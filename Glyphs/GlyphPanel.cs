using System;
using System.Collections.Generic;
using AdvancedChatFeatures.UI;
using Microsoft.Xna.Framework;

namespace AdvancedChatFeatures.Glyphs
{
    public class GlyphPanel : NavigationPanel<Glyph>
    {
        protected override NavigationElement<Glyph> BuildElement(Glyph data) =>
            new GlyphElement(data);

        protected override IEnumerable<Glyph> GetSource() =>
            GlyphInitializer.Glyphs;

        protected override string GetDescription(Glyph data)
            => data.Description;

        protected override string GetFullTag(Glyph data)
            => data.Tag;
        protected override string Prefix => "[g";


        public override void Update(GameTime gt)
        {
            if (items.Count == 0)
                PopulatePanel();

            base.Update(gt);
        }

        protected override bool MatchesFilter(Glyph data, string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return true;
            string tag = GetFullTag(data) ?? string.Empty;
            return tag.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}
