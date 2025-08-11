using AdvancedChatFeatures.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;

namespace AdvancedChatFeatures.UI.Commands.Elements
{
    public class HeaderPanel : UIPanel
    {
        public ModFilterButton modFilterButton;
        private UIColoredImageButton closeButton;

        public HeaderPanel(string text)
        {
            // Dimensions
            Width.Set(0, 1);
            Height.Set(30, 0);
            SetPadding(0);

            // Add header text centered
            Append(new UIText(text, 0.5f, true) { HAlign = 0.5f, VAlign = 0.5f });

            // Add mod filter button
            modFilterButton = new();
            Append(modFilterButton);

            // Add close button
            Asset<Texture2D> empty = TextureAssets.MagicPixel;
            closeButton = new(empty, isSmall: true) { HAlign = 1f, VAlign = 0f, Width = { Pixels = 30 }, Height = { Pixels = 30 } };
            closeButton.SetColor(Color.Transparent);
            var xText = new UIText("X", 0.5f, large: true) { HAlign = 0.5f, VAlign = 0.5f };
            closeButton.Append(xText);
            closeButton.OnLeftClick += (_, _) =>
            {
                // Closes command window
                var sys = ModContent.GetInstance<CommandsSystem>();
                if (sys?.ui != null) { 
                    sys._snoozed = true; 
                    sys.ui.SetState(null); 
                }
            };
            Append(closeButton);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
        }
    }
}
