using ChatPlus.Core.UI;

namespace ChatPlus.Core.Features.Mentions;

public class MentionState : BaseState<Mention>
{
    public MentionState() : base(new MentionPanel(), new DescriptionPanel<Mention>())
    {

    }
}