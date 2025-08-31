using ChatPlus.Common.Configs;
using ChatPlus.Core.Features.PlayerHeads.PlayerInfo;
using ChatPlus.Core.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader.UI;
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
        float scale = 1.05f; // 150% bigger
        ChatManager.DrawColorCodedStringWithShadow(
            sb,
            FontAssets.MouseText.Value,
            tag,
            pos + new Vector2(5, 5),
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

        Rectangle bounds = new((int)pos.X - 34, (int)pos.Y, (int)26, (int)26);
        if (bounds.Contains(Main.MouseScreen.ToPoint()))
        {
            Player player = Main.player[Data.PlayerIndex];

            if (player?.active == true && Conf.C.ShowPlayerPreviewWhenHovering)
            {
                PlayerInfoDrawer.Draw(Main.spriteBatch, player);
            }
        }
    }
}
