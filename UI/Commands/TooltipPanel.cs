using AdvancedChatFeatures.Common.Configs;
using AdvancedChatFeatures.Helpers;
using AdvancedChatFeatures.UI.Commands.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;

namespace AdvancedChatFeatures.UI.Commands
{
    public class TooltipPanel : DraggablePanel
    {
        public UIText text;
        public int width;
        public TooltipPanel(int width)
        {
            this.width = width;
            ResetDimensions();

            // Position
            int itemCount = Conf.C == null ? 10 : Conf.C.autocompleteConfig.CommandsVisible;
            Top.Set(-30 * itemCount - 30 - 6, 0);
            Left.Set(80, 0);
            VAlign = 1f;

            text = new("", 0.9f, false)
            {
            };
            Append(text);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public void ResetDimensions()
        {
            // Size
            Width.Set(220, 0);
            Height.Set(60, 0);

            //Style
            BackgroundColor = ColorHelper.DarkBlue * 0.8f;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
