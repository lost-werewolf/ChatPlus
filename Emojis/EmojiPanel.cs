using System;
using System.Collections.Generic;
using AdvancedChatFeatures.UI;
using Microsoft.Xna.Framework;
using Terraria;

namespace AdvancedChatFeatures.Emojis
{
    public class EmojiPanel : NavigationPanel<Emoji>
    {
        protected override NavigationElement<Emoji> BuildElement(Emoji data) => new EmojiElement(data);

        protected override IEnumerable<Emoji> GetSource() =>EmojiInitializer.Emojis;

        protected override string GetDescription(Emoji data)=> data.Description;

        protected override string GetFullTag(Emoji data)=> data.Tag;

        protected override string Prefix => ";";

        public override void Update(GameTime gt)
        {
            if (items.Count == 0)
                PopulatePanel();

            base.Update(gt);
        }

        protected override string ExtractQuery(string text)
        {
            text = text ?? string.Empty;

            // Take everything after the last colon
            int idx = text.LastIndexOf(';');
            if (idx >= 0 && idx < text.Length - 1)
            {
                string span = text.Substring(idx + 1);
                return span.Trim().Trim('"');
            }

            return string.Empty;
        }

        protected override bool MatchesFilter(Emoji data, string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return true;

            string q = query;
            string tag = GetFullTag(data) ?? string.Empty;
            if (tag.IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0)
                return true;

            IEnumerable<string> syn = data.Synonyms ?? [];
            foreach (var s in syn)
            {
                if (!string.IsNullOrEmpty(s) &&
                    s.IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0)
                    return true;
            }

            return false;
        }
    }
}
