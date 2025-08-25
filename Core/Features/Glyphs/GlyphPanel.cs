using System;
using System.Collections.Generic;
using ChatPlus.GlyphHandler;
using ChatPlus.UI;
using Microsoft.Xna.Framework;

namespace ChatPlus.Core.Features.Glyphs
{
    public class GlyphPanel : BasePanel<Glyph>
    {
        protected override BaseElement<Glyph> BuildElement(Glyph data) =>
            new GlyphElement(data);

        protected override IEnumerable<Glyph> GetSource() =>
            GlyphInitializer.Glyphs;

        protected override string GetDescription(Glyph data)
            => data.Description;

        protected override string GetTag(Glyph data)
            => data.Tag;


        public override void Update(GameTime gt)
        {
            if (items.Count == 0)
                PopulatePanel();

            base.Update(gt);
        }
    }
}
