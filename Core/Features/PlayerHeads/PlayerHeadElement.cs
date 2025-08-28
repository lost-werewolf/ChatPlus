using ChatPlus.Core.UI;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.UI.Chat;

namespace ChatPlus.Core.Features.PlayerHeads;

public class PlayerHeadElement : BaseElement<PlayerHead>
{
    public PlayerHeadElement(PlayerHead data) : base(data)
    {
        Height.Set(30, 0); // consistent row height
        Width.Set(0, 1);
    }

    public override void Draw(SpriteBatch sb)
    {
        base.Draw(sb);
        var dims = GetDimensions();
        Vector2 pos = dims.Position();

        // mod icon tag
        string tag = Data.Tag;
        float scale = 2.0f; // 150% bigger
        ChatManager.DrawColorCodedStringWithShadow(
            sb,
            FontAssets.MouseText.Value,
            tag,
            pos + new Microsoft.Xna.Framework.Vector2(3, 4),
            Color.White,
            0f,            // rotation
            Vector2.Zero,  // origin
            new Vector2(scale), // scale
            -1f,
            1f
        );

        // mod icon display name
        TextSnippet[] snip = [new TextSnippet(Data.PlayerName)];
        ChatManager.DrawColorCodedStringWithShadow(sb, FontAssets.MouseText.Value, snip, pos += new Vector2(32, 4), 0f, Vector2.Zero, Vector2.One, out _);
    }
}
