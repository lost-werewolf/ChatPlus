using ChatPlus.Core.UI;

namespace ChatPlus.Core.Features.Items
{
    public class ItemState : BaseState<Item>
    {
        public ItemState() : base(new ItemPanel(), new DescriptionPanel<Item>())
        {
        }
    }
}
