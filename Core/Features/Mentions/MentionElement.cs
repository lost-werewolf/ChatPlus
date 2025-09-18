using ChatPlus.Common.Configs;
using ChatPlus.Core.Features.Glyphs;
using ChatPlus.Core.Features.PlayerColors;
using ChatPlus.Core.Features.PlayerIcons;
using ChatPlus.Core.UI;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.UI.Chat;
using static ChatPlus.Common.Configs.Config;

namespace ChatPlus.Core.Features.Mentions;

public class MentionElement : BaseElement<Mention>
{
    public MentionElement(Mention data) : base(data)
    {
        Height.Set(30, 0);
        Width.Set(0, 1);
    }

    public override void Draw(SpriteBatch sb)
    {
        base.Draw(sb);

        if (GetViewmode() == Viewmode.ListView)
            DrawListElement(sb);
        else
            DrawGridElement(sb);
    }

    private void DrawGridElement(SpriteBatch sb)
    {
        var dims = GetDimensions();
        Vector2 pos = dims.Position();
        string playerName = Data.Tag;

        // Player head
        string headTag = PlayerIconTagHandler.GenerateTag(playerName);
        ChatManager.DrawColorCodedStringWithShadow(sb, FontAssets.MouseText.Value, headTag,
            pos + new Vector2(0, 3), Color.White, 0f, Vector2.Zero, new Vector2(1.05f), -1f, 1f);
    }

    private void DrawListElement(SpriteBatch sb)
    {
        var dims = GetDimensions();
        Vector2 pos = dims.Position();
        string playerName = Data.Tag;

        // Player head
        string headTag = PlayerIconTagHandler.GenerateTag(playerName);
        ChatManager.DrawColorCodedStringWithShadow(sb, FontAssets.MouseText.Value, headTag,
            pos + new Vector2(4, 3), Color.White, 0f, Vector2.Zero, new Vector2(1.05f), -1f, 1f);

        // Colorized player name
        pos += new Vector2(35, 4);
        string hex = "FFFFFF";
        // Try resolve active player index for synced color
        for (int i = 0; i < Main.maxPlayers; i++)
        {
            var p = Main.player[i];
            if (p?.active == true && p.name == playerName)
            {
                if (PlayerColorSystem.PlayerColors.TryGetValue(i, out var syncedHex) && !string.IsNullOrWhiteSpace(syncedHex))
                    hex = syncedHex;
                else
                    hex = PlayerColorHandler.HexFromName(playerName);
                break;
            }
        }

        var coloredSnips = ChatManager.ParseMessage($"[c/{hex}:{playerName}]", Color.White).ToArray();
        ChatManager.DrawColorCodedStringWithShadow(sb, FontAssets.MouseText.Value, coloredSnips,
            pos, 0f, Vector2.Zero, Vector2.One, out _, 500f);
    }
}
