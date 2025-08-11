using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace AdvancedChatFeatures.UI.Commands.Elements
{
    public class ClosePanel : UIPanel
    {
        public ClosePanel()
        {
            Width.Set(30, 0);
            Height.Set(30, 0);
            HAlign = 1f;
            VAlign = 0f;
            SetPadding(0);

            UIText x = new("X", 0.35f, true)
            {
                HAlign = 0.5f,
                VAlign = 0.5f
            };
            Append(x);
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);

            Main.drawingPlayerChat = false;
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            base.MouseOver(evt);
            BorderColor = Color.Yellow;
        }

        public override void MouseOut(UIMouseEvent evt)
        {
            base.MouseOut(evt);
            BorderColor = Color.Black;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
