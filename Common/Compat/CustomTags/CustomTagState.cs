using ChatPlus.Core.UI;

namespace ChatPlus.Common.Compat.CustomTags;

public class CustomTagState : BaseState<CustomTag>
{
    public string prefix;

    public CustomTagState(string prefix): base(new CustomTagPanel(prefix), new DescriptionPanel<CustomTag>())
    {
        this.prefix = prefix;
    }
}
