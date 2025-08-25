using ChatPlus.GlyphHandler;
using ChatPlus.UI;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.UI.Chat;
using static System.Net.Mime.MediaTypeNames;

namespace ChatPlus.Core.Features.Glyphs
{
    public class GlyphElement : BaseElement<Glyph>
    {
        public Glyph Glyph;
        public GlyphElement(Glyph glyph) : base(glyph)
        {
            Glyph = glyph;
            Height.Set(30, 0);
            Width.Set(0, 1);
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);

            var dims = GetDimensions();
            Vector2 pos = dims.Position();

            // Render glyph
            string tag = Glyph.Tag;
            ChatManager.DrawColorCodedStringWithShadow(sb,FontAssets.MouseText.Value,tag,pos + new Vector2(3, 2),Color.White, 0f,Vector2.Zero,Vector2.One, -1f, 1.0f);

            // Render raw tag in text form
            TextSnippet[] snip = [new TextSnippet(Glyph.Tag)];
            ChatManager.DrawColorCodedStringWithShadow(sb, FontAssets.MouseText.Value, snip, pos += new Vector2(32, 3), 0f, Vector2.Zero, Vector2.One, out _);
        }
    }
}
