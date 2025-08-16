using System.Collections.Generic;
using AdvancedChatFeatures.Commands;
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

        public override void Update(GameTime gt)
        {
            if (items.Count == 0)
                PopulatePanel();

            base.Update(gt);
        }
    }
}
