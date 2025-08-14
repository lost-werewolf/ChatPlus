using System;
using AdvancedChatFeatures.Common.Configs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace AdvancedChatFeatures.UI.Commands.Elements
{
    public class DraggablePanel : UIPanel
    {
        // Dragging
        public bool dragging;
        public Vector2 dragOffset;
        public const float DragThreshold = 3f; // very low threshold for dragging
        public Vector2 mouseDownPos;

        public DraggablePanel()
        {
            // Properties are implemented in children classes
        }

        public override void Update(GameTime gameTime)
        {
            DisableItemUseOnHover();

            if (dragging)
            {
                float dragDistance = Vector2.Distance(new Vector2(Main.mouseX, Main.mouseY), mouseDownPos);
                if (dragDistance > DragThreshold)
                {
                    float oldX = Left.Pixels;
                    float oldY = Top.Pixels;

                    float newX = Main.mouseX - dragOffset.X;
                    float newY = Main.mouseY - dragOffset.Y;

                    Left.Set(newX, 0f);
                    Top.Set(newY, 0f);
                    Recalculate();

                    float dx = newX - oldX;
                    float dy = newY - oldY;

                    var sys = ModContent.GetInstance<CommandSystem>();
                    if (sys != null)
                    {
                        if (this is CommandPanel && sys.commandState?.tooltipPanel != null)
                        {
                            var other = sys.commandState.tooltipPanel;
                            other.Left.Set(other.Left.Pixels + dx, 0f);
                            other.Top.Set(other.Top.Pixels + dy, 0f);
                            other.Recalculate();
                        }
                        else if (this is TooltipPanel && sys.commandState?.commandPanel != null)
                        {
                            var other = sys.commandState.commandPanel;
                            other.Left.Set(other.Left.Pixels + dx, 0f);
                            other.Top.Set(other.Top.Pixels + dy, 0f);
                            other.Recalculate();
                        }
                    }
                }
            }
        }

        public override void LeftMouseDown(UIMouseEvent evt)
        {
            if (!Conf.C.autocompleteConfig.DraggableWindow) return;
            if (IsHoveringScrollbar()) return;

            // start dragging
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

        private void DisableItemUseOnHover()
        {
            if (IsMouseHovering)
            {
                // disable item use
                Main.LocalPlayer.mouseInterface = true;
            }
        }

        private bool IsHoveringScrollbar()
        {
            var sys = ModContent.GetInstance<CommandSystem>();
            if (sys != null)
            {
                return sys.commandState.commandPanel.scrollbar.IsMouseHovering;
            }
            return false;
        }
    }
}