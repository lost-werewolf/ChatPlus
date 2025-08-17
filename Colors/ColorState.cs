using AdvancedChatFeatures.UI;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;

namespace AdvancedChatFeatures.Colors
{
    public class ColorState : NavigationState<ColorItem>
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
