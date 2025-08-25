using System;
using System.Collections.Generic;
using ChatPlus.EmojiHandler;
using ChatPlus.UI;
using Microsoft.Xna.Framework;
using Terraria;

namespace ChatPlus.Core.Features.Emojis
{
    public class EmojiPanel : BasePanel<Emoji>
    {
        protected override BaseElement<Emoji> BuildElement(Emoji data) => new EmojiElement(data);

        protected override IEnumerable<Emoji> GetSource() => EmojiInitializer.Emojis;

        protected override string GetDescription(Emoji data) => data.Description;

        protected override string GetTag(Emoji data) => data.Tag;

        public override void Update(GameTime gt)
        {
            if (items.Count == 0)
                PopulatePanel();

            base.Update(gt);
        }
    }
}
