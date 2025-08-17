using System.Collections.Generic;
using System.Linq;
using AdvancedChatFeatures.UI;
using Microsoft.Xna.Framework;

namespace AdvancedChatFeatures.ItemWindow
{
    public class ItemPanel : NavigationPanel<Item>
    {
        protected override NavigationElement<Item> BuildElement(Item data) => new ItemElement(data);

        protected override IEnumerable<Item> GetSource() => ItemInitializer.Items.OrderBy(i => i.ID);

        protected override string GetDescription(Item data)
        {
            return $"{data.DisplayName}";
        }

        protected override string GetTag(Item data) => data.Tag;

        public override void Update(GameTime gt)
        {
            if (items.Count == 0)
                PopulatePanel();

            base.Update(gt);
        }
    }
}
