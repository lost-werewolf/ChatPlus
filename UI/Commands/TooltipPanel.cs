using AdvancedChatFeatures.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;

namespace AdvancedChatFeatures.UI.Commands
{
    public class TooltipPanel : UIPanel
    {
        public UIText text;
        public TooltipPanel(int width)
        {
            Width.Set(width, 0);
            Height.Set(30, 0);

            text = new("", 0.9f, false)
            {
            };
            Append(text);
        }

        public override void Update(GameTime gameTime)
        {
            // Set index
            var sys = ModContent.GetInstance<CommandSystem>();
            CommandPanel commandPanel = sys.commandState.commandPanel;

            VAlign = 1f;
            BackgroundColor = ColorHelper.DarkBlue * 0.8f;
            Left.Set(80, 0);
            Height.Set(60, 0);
            Top.Set(-30 * commandPanel.itemCount -30 -6, 0);

            text.VAlign = 0f;
            text.HAlign = 0f;

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
