using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader.UI;
using Terraria.UI;
using static System.Net.Mime.MediaTypeNames;

namespace ChatPlus.Core.Features.Scrollbar;

/// <summary>
/// A scrollbar paired with a <see cref="ChatScrollList"/>
/// </summary>
public class ChatScrollbar : UIElement
{
    private float viewPosition;     // top line index
    private float viewSize = 1f;    // visible lines
    private float maxViewSize = 1f; // total lines

    private bool isDragging;
    private bool isHoveringHandle;
    private float dragYOffset;
    public bool IsDragging => isDragging;

    private Asset<Texture2D> trackTexture;
    private Asset<Texture2D> handleTexture;

    public float ViewPosition
    {
        get => viewPosition;
        set => viewPosition = MathHelper.Clamp(value, 0f, Math.Max(0f, maxViewSize - viewSize));
    }

    public float ViewSize => viewSize;
    public float MaxViewSize => maxViewSize;
    public bool CanScroll => maxViewSize > viewSize;

    public void GoToBottom() => ViewPosition = Math.Max(0f, maxViewSize - viewSize);

    public ChatScrollbar()
    {
        Width.Set(20f, 0f);
        MaxWidth.Set(20f, 0f);
        Height.Set(210f, 0f);
        Left.Set(60f, 0f);

        trackTexture = Main.Assets.Request<Texture2D>("Images/UI/Scrollbar");
        handleTexture = Main.Assets.Request<Texture2D>("Images/UI/ScrollbarInner");

        PaddingTop = 5f;
        PaddingBottom = 5f;
    }

    public void SetView(float newViewSize, float newMaxViewSize)
    {
        newViewSize = MathHelper.Clamp(newViewSize, 0f, newMaxViewSize);
        viewSize = newViewSize;
        maxViewSize = Math.Max(0f, newMaxViewSize);
        viewPosition = MathHelper.Clamp(viewPosition, 0f, Math.Max(0f, maxViewSize - viewSize));
    }

    public float GetValue() => viewPosition;

    private Rectangle GetHandleRectangle()
    {
        var inner = GetInnerDimensions();

        // Avoid division by zero
        float safeMax = maxViewSize <= 0f ? 1f : maxViewSize;
        float safeView = viewSize <= 0f ? 1f : viewSize;

        int handleHeight = (int)(inner.Height * (safeView / safeMax)) + 7;
        int handleY = (int)(inner.Y + inner.Height * (viewPosition / safeMax)) - 3;

        return new Rectangle((int)inner.X, handleY, 20, handleHeight);
    }

    public override void ScrollWheel(UIScrollWheelEvent evt)
    {
        base.ScrollWheel(evt);
    }

    protected override void DrawSelf(SpriteBatch sb)
    {
        if (isDragging)
        {
            var inner = GetInnerDimensions();
            float mouseOffset = UserInterface.ActiveInstance.MousePosition.Y - inner.Y - dragYOffset;
            ViewPosition = mouseOffset / inner.Height * (maxViewSize <= 0f ? 1f : maxViewSize);
        }

        isHoveringHandle = GetHandleRectangle().Contains(Main.MouseScreen.ToPoint());
        
        // draw track and handle
        DrawBar(sb, trackTexture.Value, GetDimensions().ToRectangle(), Color.White*0.7f);
        DrawBar(sb, handleTexture.Value, GetHandleRectangle(),Color.White * (isDragging || isHoveringHandle ? 1f : 0.65f));
    }

    private void DrawBar(SpriteBatch spriteBatch, Texture2D texture, Rectangle area, Color color)
    {
        // 9-slice style draw that draws top, middle, and lower part of the bar.
        const int cap = 6;

        spriteBatch.Draw(texture,new Rectangle(area.X, area.Y, area.Width, cap),new Rectangle(0, 0, texture.Width, cap),color);
        spriteBatch.Draw(texture,new Rectangle(area.X, area.Y + cap, area.Width, area.Height - cap * 2),new Rectangle(0, cap, texture.Width, 4),color);
        spriteBatch.Draw(texture,new Rectangle(area.X, area.Bottom - cap, area.Width, cap),new Rectangle(0, texture.Height - cap, texture.Width, cap),color);
    }

    public override void LeftMouseDown(UIMouseEvent evt)
    {
        Rectangle handleRect = GetHandleRectangle();

        if (handleRect.Contains(evt.MousePosition.ToPoint()))
        {
            isDragging = true;
            dragYOffset = evt.MousePosition.Y - handleRect.Y;
        }
        else
        {
            var inner = GetInnerDimensions();
            float mouseY = evt.MousePosition.Y - inner.Y;
            float target = mouseY - handleRect.Height / 2f;
            ViewPosition = target / inner.Height * (maxViewSize <= 0f ? 1f : maxViewSize);
        }
    }

    public override void LeftMouseUp(UIMouseEvent evt) => isDragging = false;

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        // Set position
        Top.Set(Main.screenHeight - 247, 0f);
        Height.Set(210f, 0f);

        // If hovering the scrollbar, also consume vanilla scroll to prevent hotbar changes.
        if (IsMouseHovering)
        {
            Main.LocalPlayer.mouseInterface = true;
        }
    }
}
