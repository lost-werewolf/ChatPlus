using System;
using AdvancedChatFeatures.Commands;
using AdvancedChatFeatures.Common.Configs;
using AdvancedChatFeatures.Emojis;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace AdvancedChatFeatures.UI
{
    public abstract class DraggablePanel : UIPanel
    {
        // Dragging
        private bool dragging;
        private Vector2 dragOffset;
        private const float DragThreshold = 3f; // very low threshold for dragging
        private Vector2 mouseDownPos;

        public override void Update(GameTime gameTime)
        {
            if (IsMouseHovering)
                Main.LocalPlayer.mouseInterface = true;

            if (dragging)
            {
                float dragDistance = Vector2.Distance(new Vector2(Main.mouseX, Main.mouseY), mouseDownPos);
                if (dragDistance > DragThreshold)
                {
                    // Current screen-space rects (before moving)
                    var thisRect = GetDimensions().ToRectangle();

                    float oldX = Left.Pixels;
                    float oldY = Top.Pixels;

                    // Target based on mouse
                    float targetX = Main.mouseX - dragOffset.X;
                    float targetY = Main.mouseY - dragOffset.Y;

                    // Raw deltas (in pixels)
                    float rawDx = targetX - oldX;
                    float rawDy = targetY - oldY;

                    // Allowed movement so THIS panel stays on-screen
                    float dxMin = -thisRect.Left;
                    float dxMax = Main.screenWidth - thisRect.Right;
                    float dyMin = -thisRect.Top;
                    float dyMax = Main.screenHeight - thisRect.Bottom;

                    // Find the "other" panel (the one we move together with)
                    //var sys = ModContent.GetInstance<CommandSystem>();
                    UIElement other = null;
                    //if (sys != null)
                    //{
                    //    if (this is CommandPanel && sys.commandState?.commandDescriptionPanel != null)
                    //        other = sys.commandState.commandDescriptionPanel;
                    //    else if (this is DescriptionPanel && sys.commandState?.commandPanel != null)
                    //        other = sys.commandState.commandPanel;
                    //}

                    // Intersect constraints so the OTHER panel also stays on-screen
                    if (other != null)
                    {
                        var oRect = other.GetDimensions().ToRectangle();
                        dxMin = Math.Max(dxMin, -oRect.Left);
                        dxMax = Math.Min(dxMax, Main.screenWidth - oRect.Right);
                        dyMin = Math.Max(dyMin, -oRect.Top);
                        dyMax = Math.Min(dyMax, Main.screenHeight - oRect.Bottom);
                    }

                    // Clamp deltas so both panels remain fully within bounds
                    float dx = MathHelper.Clamp(rawDx, dxMin, dxMax);
                    float dy = MathHelper.Clamp(rawDy, dyMin, dyMax);

                    // Apply to this panel
                    float newX = oldX + dx;
                    float newY = oldY + dy;
                    Left.Set(newX, 0f);
                    Top.Set(newY, 0f);
                    Recalculate();

                    // Apply the same clamped delta to the paired panel (if any)
                    if (other != null)
                    {
                        other.Left.Set(other.Left.Pixels + dx, 0f);
                        other.Top.Set(other.Top.Pixels + dy, 0f);
                        other.Recalculate();
                    }
                }
            }
        }

        public override void LeftMouseDown(UIMouseEvent evt)
        {
            if (!Conf.C.featureStyleConfig.MakeWindowDraggable)
                return;

            // If hovering a scrollbar, skip drag
            var emojiSys = ModContent.GetInstance<EmojiSystem>();
            if (emojiSys?.emojiState?.emojiPanel?.scrollbar?.IsMouseHovering == true)
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