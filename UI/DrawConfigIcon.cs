using AdvancedChatFeatures.Common.Configs;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace AdvancedChatFeatures.UI
{
    public class DrawConfigIcon : UIImageButton
    {
        public DrawConfigIcon(Asset<Texture2D> texture) : base(texture)
        {
            Width.Set(32, 0);
            Height.Set(32, 0);
            HAlign = 0.02f;
            VAlign = 0.99f;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!Main.drawingPlayerChat || !Conf.C.ShowConfigIcon)
            {
                return;
            }

            base.Draw(spriteBatch);

            if (IsMouseHovering)
            {
                Main.hoverItemName = "Chat Settings";
            }
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            if (!Main.drawingPlayerChat || !Conf.C.ShowConfigIcon)
            {
                return;
            }

            Conf.C.Open();
        }
    }
}