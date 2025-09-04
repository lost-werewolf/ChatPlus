using ChatPlus.Core.UI;

namespace ChatPlus.Core.Features.Mentions;

public class MentionState : BaseState<MentionEntry>
{
    public MentionState() : base(new MentionPanel(), new DescriptionPanel<MentionEntry>()) { }
}