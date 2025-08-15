using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace AdvancedChatFeatures.UI.Glyphs
{
    public class GlyphPanel : NavigationPanel<Glyph>
    {
        protected override NavigationElement BuildElement(Glyph data) =>
            new GlyphElement(data);

        protected override IEnumerable<Glyph> GetSource() =>
            GlyphInitializer.Glyphs;

        public override void Update(GameTime gt)
        {
            base.Update(gt);
        }
    }
}
