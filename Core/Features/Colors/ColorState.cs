using ChatPlus.Core.UI;

namespace ChatPlus.Core.Features.Colors
{
    public class ColorState : BaseState<ColorItem>
    {
        public ColorState() : base(new ColorPanel(), new DescriptionPanel<ColorItem>())
        {
        }
    }
}
