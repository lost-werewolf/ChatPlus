using System.Collections.Generic;
using System.Linq;
using ChatPlus.Core.UI;
using Microsoft.Xna.Framework;

namespace ChatPlus.Core.Features.Items
{
    public class ItemPanel : BasePanel<ItemEntry>
    {
        protected override BaseElement<ItemEntry> BuildElement(ItemEntry data) => new ItemElement(data);

        protected override IEnumerable<ItemEntry> GetSource() => ItemManager.Items.OrderBy(i => i.ID);

        protected override string GetDescription(ItemEntry data)
        {
            return $"{data.DisplayName}";
        }

        protected override string GetTag(ItemEntry data) => data.Tag;

        public override void Update(GameTime gt)
        {
            if (items.Count == 0)
                PopulatePanel();

            base.Update(gt);
        }
    }
}
