using ChatPlus.Core.UI;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.UI.Chat;

namespace ChatPlus.Core.Features.Links;

public class LinkElement : BaseElement<LinkEntry>
{
    public LinkElement(LinkEntry data) : base(data)
    {
        Height.Set(30, 0);
        Width.Set(0, 1f);
    }

    public override void Draw(SpriteBatch sb)
    {
        base.Draw(sb);
        var dims = GetDimensions();
        var pos = dims.Position();

        // Tag
        string tag = Data.Tag;
        ChatManager.DrawColorCodedStringWithShadow(
            sb,
            FontAssets.MouseText.Value,
            tag,
            pos + new Vector2(4, 4),
            Color.White,
            0f,
            Vector2.Zero,
            new Vector2(0.9f),
            -1f,
            1f);
    }
}
