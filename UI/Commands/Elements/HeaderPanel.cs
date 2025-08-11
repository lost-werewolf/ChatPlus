using Microsoft.Xna.Framework;
using Terraria.GameContent.UI.Elements;

namespace AdvancedChatFeatures.UI.Commands.Elements
{
    public class HeaderPanel : UIPanel
    {
        public HeaderPanel(string text)
        {
            Width.Set(0, 1);
            Height.Set(30, 0);
            SetPadding(0);

            UIText headerText = new(text, 0.5f, true)
            {
                HAlign = 0.5f,
                VAlign = 0.5f
            };
            Append(headerText);

            ClosePanel x = new();
            Append(x);
        }

        public override void Update(GameTime gameTime)
        {
            Top.Set(-6, 0);

            base.Update(gameTime);
        }
    }
}
