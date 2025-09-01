using ChatPlus.Core.UI;

namespace ChatPlus.Core.Features.Links;

public class LinkState : BaseState<LinkEntry>
{
    public LinkState() : base(new LinkPanel(), new DescriptionPanel<LinkEntry>()) { }
}
