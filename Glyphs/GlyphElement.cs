using AdvancedChatFeatures.UI;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.UI.Chat;

namespace AdvancedChatFeatures.Glyphs
{
    public class GlyphElement : NavigationElement<Glyph>
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
            ChatManager.DrawColorCodedStringWithShadow(
                sb,
                FontAssets.MouseText.Value,
                tag,
                pos + new Vector2(3, 2),
                Color.White,
                0f,
                Vector2.Zero,
                new Vector2(1.0f),
                -1f,
                1.0f
            );

            // Render raw tag in text form
            var glyphSnippet = new[] { new TextSnippet(Glyph.Tag.ToString()) { Color = Color.White, CheckForHover = false } };
            ChatManager.DrawColorCodedStringWithShadow(
                sb,
                FontAssets.MouseText.Value,
                glyphSnippet,
                pos + new Vector2(32, 3),
                0f,
                Vector2.Zero,
                new Vector2(1.0f),
                out _
            );
        }
    }
}
