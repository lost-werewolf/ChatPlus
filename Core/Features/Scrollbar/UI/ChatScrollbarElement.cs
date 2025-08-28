using Humanizer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.UI;
using Terraria.ID;

namespace ChatPlus.Core.Features.Scrollbar.UI;

public class ChatScrollbarElement : UIElement
{
    private float _viewPosition = 0f;
    private float _viewSize = 1f;
    private float _maxViewSize = 1f;
    private bool _isDragging;
    private bool _isHoveringOverHandle;
    private float _dragYOffset;
    private Asset<Texture2D> _texture;
    private Asset<Texture2D> _innerTexture;

    public float ViewPosition
    {
        get => _viewPosition;
        set => _viewPosition = MathHelper.Clamp(value, 0f, _maxViewSize - _viewSize);
    }

    public bool CanScroll => _maxViewSize > _viewSize;
    public float ViewSize => _viewSize;
    public float MaxViewSize => _maxViewSize;

    public void GoToBottom()
    {
        ViewPosition = _maxViewSize - _viewSize;
    }

    public ChatScrollbarElement()
    {
        // Set scrollbar dimensions and load textures
        Width.Set(20f, 0f);
        MaxWidth.Set(20f, 0f);
        Height.Set(200f, 0f);
        Top.Set(Main.screenHeight - 247, 0f);
        Left.Set(60f, 0f);
        _texture = Main.Assets.Request<Texture2D>("Images/UI/Scrollbar");
        _innerTexture = Main.Assets.Request<Texture2D>("Images/UI/ScrollbarInner");
        PaddingTop = 5f;
        PaddingBottom = 5f;
    }

    public void SetView(float viewSize, float maxViewSize)
    {
        // Update the size of the scroll handle relative to content
        viewSize = MathHelper.Clamp(viewSize, 0f, maxViewSize);
        _viewSize = viewSize;
        _maxViewSize = maxViewSize;
        // Adjust view position if out of bounds after content size change
        _viewPosition = MathHelper.Clamp(_viewPosition, 0f, _maxViewSize - _viewSize);
    }

    public float GetValue() => _viewPosition;

    public Rectangle GetHandleRectangle()
    {
        CalculatedStyle innerDimensions = GetInnerDimensions();
        if (_maxViewSize <= 0f && _viewSize <= 0f)
        {
            // Prevent division by zero
            _viewSize = 1f;
            _maxViewSize = 1f;
        }
        // Calculate handle size and position within the scrollbar track
        int handleHeight = (int)(innerDimensions.Height * (_viewSize / _maxViewSize)) + 7;
        int handleY = (int)(innerDimensions.Y + innerDimensions.Height * (_viewPosition / _maxViewSize)) - 3;
        return new Rectangle((int)innerDimensions.X, handleY, 20, handleHeight);
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        // If dragging, move the view position according to mouse movement
        if (_isDragging)
        {
            CalculatedStyle inner = GetInnerDimensions();
            float mouseOffset = UserInterface.ActiveInstance.MousePosition.Y - inner.Y - _dragYOffset;
            ViewPosition = (mouseOffset / inner.Height) * _maxViewSize;
        }
        // Update hover state for visual feedback
        Rectangle handleRect = GetHandleRectangle();
        bool wasHovering = _isHoveringOverHandle;
        _isHoveringOverHandle = handleRect.Contains(UserInterface.ActiveInstance.MousePosition.ToPoint());
        if (!wasHovering && _isHoveringOverHandle && Main.hasFocus)
        {
            SoundEngine.PlaySound(SoundID.MenuTick);
        }
        // Ensure scrollbar stays aligned to the chat area each frame
        Top.Set(Main.screenHeight - 247, 0f);
        Height.Set(210f, 0f);
        Recalculate();  // update geometry if position changed

        // Draw scrollbar track and handle
        spriteBatch.Draw(_texture.Value, GetDimensions().ToRectangle(), new Rectangle(0, 6, _texture.Width(), 4), Color.White);
        spriteBatch.Draw(_innerTexture.Value, handleRect, new Rectangle(0, 6, _innerTexture.Width(), 4),
                         Color.White * (_isDragging || _isHoveringOverHandle ? 1f : 0.65f));
    }

    public override void LeftMouseDown(UIMouseEvent evt)
    {
        base.LeftMouseDown(evt);
        Rectangle handleRect = GetHandleRectangle();
        if (handleRect.Contains(evt.MousePosition.ToPoint()))
        {
            // Begin dragging the scrollbar thumb
            _isDragging = true;
            _dragYOffset = evt.MousePosition.Y - handleRect.Y;
        }
        else
        {
            // Click outside the handle: jump scroll position (handle centered on click)
            CalculatedStyle inner = GetInnerDimensions();
            float mouseY = evt.MousePosition.Y - inner.Y;
            float target = mouseY - handleRect.Height / 2f;
            ViewPosition = (target / inner.Height) * _maxViewSize;
        }
    }

    public override void LeftMouseUp(UIMouseEvent evt)
    {
        base.LeftMouseUp(evt);
        _isDragging = false; // stop dragging
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        // If hovering, block game's own mouse scroll handling to use it for our UI
        if (IsMouseHovering)
        {
            PlayerInput.LockVanillaMouseScroll("ModLoader/UIList");
            Main.LocalPlayer.mouseInterface = true;
        }
    }
}
