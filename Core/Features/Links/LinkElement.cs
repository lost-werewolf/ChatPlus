using ChatPlus.Core.UI;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
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

        // Tag (iconic) - smaller
        string tag = Data.Tag;
        ChatManager.DrawColorCodedStringWithShadow(
            sb,
            FontAssets.MouseText.Value,
            tag,
            pos + new Microsoft.Xna.Framework.Vector2(4, 4),
            Microsoft.Xna.Framework.Color.White,
            0f,
            Microsoft.Xna.Framework.Vector2.Zero,
            new Microsoft.Xna.Framework.Vector2(0.9f),
            -1f,
            1f);

        // Display text
        //string txt = Data.Display;
        //TextSnippet[] snips = [new TextSnippet(txt)];
        //ChatManager.DrawColorCodedStringWithShadow(
        //    sb,
        //    FontAssets.MouseText.Value,
        //    snips,
        //    pos + new Microsoft.Xna.Framework.Vector2(150, 4),
        //    0f,
        //    Microsoft.Xna.Framework.Vector2.Zero,
        //    Microsoft.Xna.Framework.Vector2.One,
        //    out _);
    }
}
