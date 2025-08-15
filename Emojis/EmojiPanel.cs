using System.Collections.Generic;
using AdvancedChatFeatures.UI;
using Microsoft.Xna.Framework;

namespace AdvancedChatFeatures.Emojis
{
    public class EmojiPanel : NavigationPanel<Emoji>
    {
        protected override NavigationElement BuildElement(Emoji data) =>
            new EmojiElement(data);

        protected override IEnumerable<Emoji> GetSource() =>
            EmojiInitializer.Emojis;

        public override void Update(GameTime gt)
        {
            base.Update(gt);
        }
    }
}
