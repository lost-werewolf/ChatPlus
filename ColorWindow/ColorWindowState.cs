using AdvancedChatFeatures.UI;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;

namespace AdvancedChatFeatures.ColorWindow
{
    public class ColorWindowState : UIState
    {
        public ColorPanel colorPanel;
        public DescriptionPanel<Color> colorDescPanel;

        public ColorWindowState()
        {
            colorPanel = new();
            Append(colorPanel);

            colorDescPanel = new("List of colors");
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
