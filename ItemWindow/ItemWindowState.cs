using AdvancedChatFeatures.UI;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;

namespace AdvancedChatFeatures.ItemWindow
{
    public class ItemWindowState : UIState
    {
        public ItemPanel itemPanel;
        public DescriptionPanel<Item> itemDescPanel;

        public ItemWindowState()
        {
            itemPanel = new();
            Append(itemPanel);

            itemDescPanel = new();
            Append(itemDescPanel);

            itemPanel.ConnectedPanel = itemDescPanel;
            itemDescPanel.ConnectedPanel = itemPanel;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
