using System.Collections.Generic;
using AdvancedChatFeatures.UI;
using Microsoft.Xna.Framework;
using Terraria;

namespace AdvancedChatFeatures.Emojis
{
    public class EmojiPanel : NavigationPanel<Emoji>
    {
        protected override NavigationElement<Emoji> BuildElement(Emoji data) =>
            new EmojiElement(data);

        protected override IEnumerable<Emoji> GetSource() =>
            EmojiInitializer.Emojis;

        protected override string GetDescription(Emoji data)
            => data.Description;

        protected override string GetFullTag(Emoji data)
            => data.Tag;

        public override void Update(GameTime gt)
        {
            if (items.Count == 0)
                PopulatePanel();

            base.Update(gt);
        }
    }
}
