using ChatPlus.Common.Configs;
using ChatPlus.Core.Features.Colors;
using ChatPlus.Core.Features.PlayerColors;
using ChatPlus.Core.Features.PlayerIcons
.PlayerInfo;
using ChatPlus.Core.Helpers;
using ChatPlus.Core.UI;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace ChatPlus.Core.Features.PlayerIcons
;

public class PlayerIconElement : BaseElement<PlayerIcon>
{
    public PlayerIconElement(PlayerIcon data) : base(data)
    {
        Height.Set(30, 0); // consistent row height
        Width.Set(0, 1);
    }

    public override void Draw(SpriteBatch sb)
    {
        base.Draw(sb);
        var dims = GetDimensions();
        Vector2 pos = dims.Position();
        TextSnippet[] snip = [new TextSnippet(Data.PlayerName)];
        string tag = Data.Tag;

        // Draw player head
        ChatManager.DrawColorCodedStringWithShadow(sb, FontAssets.MouseText.Value, tag,
            pos + new Vector2(5, 5), Color.White, 0f, Vector2.Zero, new Vector2(1.05f), -1f, 1f);

        // Draw outline
        pos += new Vector2(32, 4);
        ChatManager.DrawColorCodedStringShadow(sb, FontAssets.MouseText.Value, snip, pos, Color.Black, 0f, Vector2.Zero, Vector2.One);

        // Draw player name in synced color
        string hex = "FFFFFF";
        if (AssignPlayerColorsSystem.PlayerColors.TryGetValue(Data.PlayerIndex, out var syncedHex))
            hex = syncedHex ?? "FFFFFF";

        var coloredSnips = ChatManager.ParseMessage($"[c/{hex}:{Data.PlayerName}:]", Color.White).ToArray();
        ChatManager.DrawColorCodedStringWithShadow(sb, FontAssets.MouseText.Value, coloredSnips,
            pos, 0f, Vector2.Zero, Vector2.One, out _, 500f);

        // hover click action handler
        Rectangle bounds = new((int)pos.X - 34, (int)pos.Y, (int)26, (int)26);
        if (bounds.Contains(Main.MouseScreen.ToPoint()))
        {
            Player player = Main.player[Data.PlayerIndex];

            if (player?.active == true && Conf.C.ShowPlayerPreviewWhenHovering)
            {
            }
        }
    }
}
