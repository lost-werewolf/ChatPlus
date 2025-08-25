using ChatPlus.ColorHandler;
using ChatPlus.UI;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;

namespace ChatPlus.Core.Features.Colors
{
    public class ColorState : BaseState<ColorItem>
    {
        public ColorPanel colorPanel;
        public DescriptionPanel<ColorItem> colorDescPanel;

        public ColorState()
        {
            colorPanel = new();
            Append(colorPanel);

            colorDescPanel = new();
            Append(colorDescPanel);

            colorPanel.ConnectedPanel = colorDescPanel;
            colorDescPanel.ConnectedPanel = colorPanel;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
