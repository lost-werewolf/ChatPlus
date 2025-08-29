using ChatPlus.Core.UI;

namespace ChatPlus.Core.Features.ModIcons;

public class ModIconState : BaseState<ModIcon>
{
    public ModIconState() : base(new ModIconPanel(), new DescriptionPanel<ModIcon>())
    {
    }
}
