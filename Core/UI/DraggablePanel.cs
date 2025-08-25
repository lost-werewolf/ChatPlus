using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace ChatPlus.Core.UI
{
    public abstract class DraggablePanel : UIPanel
    {
        // Dragging
        private bool dragging;
        private Vector2 dragOffset;
        private const float DragThreshold = 3f; // very low threshold for dragging
        private Vector2 mouseDownPos;

        /// <summary>
        /// If set, this panel will drag together with this one.
        /// </summary>
        public DraggablePanel ConnectedPanel { get; set; }

        public override void Update(GameTime gameTime)
        {
            if (IsMouseHovering)
                Main.LocalPlayer.mouseInterface = true;

            if (dragging)
            {
                float dragDistance = Vector2.Distance(new Vector2(Main.mouseX, Main.mouseY), mouseDownPos);
                if (dragDistance > DragThreshold)
                {
                    // Current rect
                    var thisRect = GetDimensions().ToRectangle();
                    float oldX = Left.Pixels;

                    // Only allow X dragging
                    float targetX = Main.mouseX - dragOffset.X;
                    float rawDx = targetX - oldX;

                    float dxMin = -thisRect.Left;
                    float dxMax = Main.screenWidth - thisRect.Right;

                    // If there’s a connected panel, constrain so it stays onscreen too
                    if (ConnectedPanel != null)
                    {
                        var otherRect = ConnectedPanel.GetDimensions().ToRectangle();
                        dxMin = Math.Max(dxMin, -otherRect.Left);
                        dxMax = Math.Min(dxMax, Main.screenWidth - otherRect.Right);
                    }

                    // Clamp X delta
                    float dx = MathHelper.Clamp(rawDx, dxMin, dxMax);

                    // Apply to this panel
                    float newX = oldX + dx;
                    Left.Set(newX, 0f);
                    Recalculate();

                    // Apply to connected panel
                    if (ConnectedPanel != null)
                    {
                        ConnectedPanel.Left.Set(ConnectedPanel.Left.Pixels + dx, 0f);
                        ConnectedPanel.Recalculate();
                    }
                }
            }
        }

        public override void LeftMouseDown(UIMouseEvent evt)
        {
            // If hovering any UIScrollbar, skip drag
            var cmdSys = ModContent.GetInstance<CommandSystem>();
            var colorSys = ModContent.GetInstance<ColorSystem>();
            var emojiSys = ModContent.GetInstance<EmojiSystem>();
            var glyphSys = ModContent.GetInstance<GlyphSystem>();
            var itemSys = ModContent.GetInstance<ItemSystem>();
            var uploadSys = ModContent.GetInstance<UploadSystem>();

            if (cmdSys.commandState.commandPanel.scrollbar.IsMouseHovering
            || colorSys.colorState.colorPanel.scrollbar.IsMouseHovering
            || emojiSys.emojiState.emojiPanel.scrollbar.IsMouseHovering
            || glyphSys.glyphState.glyphPanel.scrollbar.IsMouseHovering
            || itemSys.itemWindowState.itemPanel.scrollbar.IsMouseHovering
            || uploadSys.state.panel.scrollbar.IsMouseHovering)
                return;

            // Start dragging
            mouseDownPos = evt.MousePosition;
            base.LeftMouseDown(evt);
            dragging = true;
            dragOffset = evt.MousePosition - new Vector2(Left.Pixels, Top.Pixels);
        }

        public override void LeftMouseUp(UIMouseEvent evt)
        {
            base.LeftMouseUp(evt);
            dragging = false; // stop dragging
            Recalculate();
        }
    }
}