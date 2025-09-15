using System;
using ChatPlus.Core.UI;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.UI.Chat;

namespace ChatPlus.Common.Compat.CustomTags;
public class CustomTagElement : BaseElement<CustomTag>
{
    public CustomTag customTag;

    public CustomTagElement(CustomTag tag) : base(tag)
    {
        customTag = tag;
        Height.Set(30, 0);
        Width.Set(0, 1);
    }

    public override void Draw(SpriteBatch sb)
    {
        base.Draw(sb);

        // Get position
        var dims = GetDimensions();
        Vector2 pos = dims.Position();

        // Draw tag
        string tag = customTag.ActualTag;
        ChatManager.DrawColorCodedStringWithShadow(sb, FontAssets.MouseText.Value, tag, pos + new Vector2(3, 2), Color.White, 0f, Vector2.Zero, Vector2.One, -1f, 1.0f);

        // Draw tag text, e.g [customTag:example]
        TextSnippet[] snip = [new TextSnippet(tag)];
        ChatManager.DrawColorCodedStringWithShadow(sb, FontAssets.MouseText.Value, snip, pos += new Vector2(32, 3), 0f, Vector2.Zero, Vector2.One, out _);
    }
}
