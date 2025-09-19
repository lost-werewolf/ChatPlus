using ChatPlus.Core.UI;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.UI.Chat;

namespace ChatPlus.Core.Features.Glyphs;
public class GlyphElement : BaseElement<Glyph>
{
    public Glyph Glyph { get; }

    public GlyphElement(Glyph glyph) : base(glyph)
    {
        Glyph = glyph;
    }

    protected override void DrawGridElement(SpriteBatch sb)
    {
        var pos = GetDimensions().Position();
        ChatManager.DrawColorCodedStringWithShadow(
            sb, FontAssets.MouseText.Value, Glyph.Tag,
            pos + new Vector2(2, 2), Color.White, 0f, Vector2.Zero, Vector2.One
        );
    }

    protected override void DrawListElement(SpriteBatch sb)
    {
        var pos = GetDimensions().Position();
        ChatManager.DrawColorCodedStringWithShadow(
            sb, FontAssets.MouseText.Value, Glyph.Tag,
            pos + new Vector2(3, 2), Color.White, 0f, Vector2.Zero, Vector2.One
        );

        TextSnippet[] snip = [new TextSnippet(Glyph.Description)];
        ChatManager.DrawColorCodedStringWithShadow(
            sb, FontAssets.MouseText.Value, snip,
            pos + new Vector2(32, 3), 0f, Vector2.Zero, Vector2.One, out _
        );
    }
}
