using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
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

        }

        public override void Update(GameTime gameTime)
        {
            DisableItemUseOnHover();

            if (dragging)
            {

                float dragDistance = Vector2.Distance(new Vector2(Main.mouseX, Main.mouseY), mouseDownPos);
                if (dragDistance > DragThreshold)
                {
                    Left.Set(Main.mouseX - dragOffset.X, 0f);
                    Top.Set(Main.mouseY - dragOffset.Y, 0f);
                    Recalculate();
                }
            }
        }

        public override void LeftMouseDown(UIMouseEvent evt)
        {
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
            var sys = ModContent.GetInstance<CommandsSystem>();
            if (sys != null)
            {
                return sys.commandsListState.commandsPanel.scrollbar.IsMouseHovering;
            }
            return false;
        }
    }
}
