using ChatPlus.ItemHandler;
using ChatPlus.UI;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;

namespace ChatPlus.Core.Features.Items
{
    public class ItemState : BaseState<Item>
    {
        public ItemPanel itemPanel;
        public DescriptionPanel<Item> itemDescPanel;

        public ItemState()
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
