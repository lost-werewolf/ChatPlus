using ChatPlus.GlyphHandler;
using ChatPlus.ModIconHandler;
using ChatPlus.UI;
using Microsoft.Xna.Framework.Graphics;
using Stubble.Core.Classes;
using Terraria.GameContent;
using Terraria.UI.Chat;

namespace ChatPlus.Core.Features.ModIcons;

public class ModIconElement : BaseElement<ModIcon>
{
    public ModIconElement(ModIcon data) : base(data)
    {
        Height.Set(30, 0);
        Width.Set(0, 1);
    }

    public override void Draw(SpriteBatch sb)
    {
        base.Draw(sb);
        var dims = GetDimensions();
        Vector2 pos = dims.Position();

        // tag text
        ChatManager.DrawColorCodedStringWithShadow(
            sb,
            FontAssets.MouseText.Value,
            Data.Tag,
            pos + new Vector2(3, 2),
            Color.White,
            0f,
            Vector2.Zero,
            Vector2.One,
            -1f,
            1f
        );

        // Render raw tag in text form
        TextSnippet[] snip = [new TextSnippet(Data.DisplayName)];
        ChatManager.DrawColorCodedStringWithShadow(sb, FontAssets.MouseText.Value, snip, pos += new Vector2(32, 3), 0f, Vector2.Zero, Vector2.One, out _);
    }
}
