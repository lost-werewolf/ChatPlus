using ChatPlus.Core.UI;

namespace ChatPlus.Core.Features.Emojis
{
    public class EmojiState : BaseState<Emoji>
    {
        public static bool WasOpenedByButton; 

        public EmojiState() : base(new EmojiPanel(), new DescriptionPanel<Emoji>())
        {
        }
    }
}
