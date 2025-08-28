using Humanizer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.UI;
using Terraria.ID;

namespace ChatPlus.Core.Features.Scrollbar.UI
{
    public class ChatScrollbarElement : UIElement
    {
        public float _viewPosition;

        public float _viewSize = 1f;

        public float _maxViewSize = 20f;

        public bool _isDragging;

        public bool _isHoveringOverHandle;

        public float _dragYOffset;

        public Asset<Texture2D> _texture;

        public Asset<Texture2D> _innerTexture;

        public float ViewPosition
        {
            get
            {
                return _viewPosition;
            }
            set
            {
                _viewPosition = MathHelper.Clamp(value, 0f, _maxViewSize - _viewSize);
            }
        }

        public bool CanScroll => _maxViewSize != _viewSize;

        public float ViewSize => _viewSize;

        public float MaxViewSize => _maxViewSize;

        public void GoToBottom()
        {
            ViewPosition = _maxViewSize - _viewSize;
        }
        public ChatScrollbarElement()
        {
            Width.Set(20f, 0f);
            MaxWidth.Set(20f, 0f);
            _texture = Main.Assets.Request<Texture2D>("Images/UI/Scrollbar");
            _innerTexture = Main.Assets.Request<Texture2D>("Images/UI/ScrollbarInner");
            PaddingTop = 5f;
            PaddingBottom = 5f;

            Width.Set(20, 0);
            Height.Set(200, 0);
            Top.Set(Main.screenHeight - 243, 0);
            Left.Set(60, 0);
        }

        public void SetView(float viewSize, float maxViewSize)
        {
            viewSize = MathHelper.Clamp(viewSize, 0f, maxViewSize);
            _viewPosition = MathHelper.Clamp(_viewPosition, 0f, maxViewSize - viewSize);
            _viewSize = viewSize;
            _maxViewSize = maxViewSize;
        }

        public float GetValue()
        {
            return _viewPosition;
        }

        public Rectangle GetHandleRectangle()
        {
            CalculatedStyle innerDimensions = GetInnerDimensions();
            if (_maxViewSize == 0f && _viewSize == 0f)
            {
                _viewSize = 1f;
                _maxViewSize = 1f;
            }

            return new Rectangle((int)innerDimensions.X, (int)(innerDimensions.Y + innerDimensions.Height * (_viewPosition / _maxViewSize)) - 3, 20, (int)(innerDimensions.Height * (_viewSize / _maxViewSize)) + 7);
        }

        public void DrawBar(SpriteBatch spriteBatch, Texture2D texture, Rectangle dimensions, Color color)
        {
            Top.Set(Main.screenHeight-247, 0);
            Height.Set(210, 0);

            spriteBatch.Draw(texture, new Rectangle(dimensions.X, dimensions.Y - 6, dimensions.Width, 6), new Rectangle(0, 0, texture.Width, 6), color);
            spriteBatch.Draw(texture, new Rectangle(dimensions.X, dimensions.Y, dimensions.Width, dimensions.Height), new Rectangle(0, 6, texture.Width, 4), color);
            spriteBatch.Draw(texture, new Rectangle(dimensions.X, dimensions.Y + dimensions.Height, dimensions.Width, 6), new Rectangle(0, texture.Height - 6, texture.Width, 6), color);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            CalculatedStyle dimensions = GetDimensions();
            CalculatedStyle innerDimensions = GetInnerDimensions();
            if (_isDragging)
            {
                float num = UserInterface.ActiveInstance.MousePosition.Y - innerDimensions.Y - _dragYOffset;
                _viewPosition = MathHelper.Clamp(num / innerDimensions.Height * _maxViewSize, 0f, _maxViewSize - _viewSize);
            }

            Rectangle handleRectangle = GetHandleRectangle();
            Vector2 mousePosition = UserInterface.ActiveInstance.MousePosition;
            bool isHoveringOverHandle = _isHoveringOverHandle;
            _isHoveringOverHandle = handleRectangle.Contains(new Point((int)mousePosition.X, (int)mousePosition.Y));
            if (!isHoveringOverHandle && _isHoveringOverHandle && Main.hasFocus)
            {
                SoundEngine.PlaySound(SoundID.MenuTick);
            }

            DrawBar(spriteBatch, _texture.Value, dimensions.ToRectangle(), Color.White);
            DrawBar(spriteBatch, _innerTexture.Value, handleRectangle, Color.White * (_isDragging || _isHoveringOverHandle ? 1f : 0.65f));
        }

        public override void LeftMouseDown(UIMouseEvent evt)
        {
            base.LeftMouseDown(evt);
            if (evt.Target == this)
            {
                Rectangle handleRectangle = GetHandleRectangle();
                if (handleRectangle.Contains(new Point((int)evt.MousePosition.X, (int)evt.MousePosition.Y)))
                {
                    _isDragging = true;
                    _dragYOffset = evt.MousePosition.Y - handleRectangle.Y;
                }
                else
                {
                    CalculatedStyle innerDimensions = GetInnerDimensions();
                    float num = UserInterface.ActiveInstance.MousePosition.Y - innerDimensions.Y - (handleRectangle.Height >> 1);
                    _viewPosition = MathHelper.Clamp(num / innerDimensions.Height * _maxViewSize, 0f, _maxViewSize - _viewSize);
                }
            }
        }

        public override void LeftMouseUp(UIMouseEvent evt)
        {
            base.LeftMouseUp(evt);
            _isDragging = false;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (IsMouseHovering)
            {
                PlayerInput.LockVanillaMouseScroll("ModLoader/UIList");
                Main.LocalPlayer.mouseInterface = true;
            }
        }
    }
}