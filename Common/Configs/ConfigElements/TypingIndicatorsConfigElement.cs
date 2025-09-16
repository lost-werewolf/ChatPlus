using ChatPlus.Common.Configs.ConfigElements.Base;
using ChatPlus.Core.Features.PlayerColors;
using ChatPlus.Core.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.UI.Chat;

namespace ChatPlus.Common.Configs.ConfigElements;

public class TypingIndicatorsConfigElement : BaseBoolConfigElement
{
    protected override void OnToggled(bool newValue)
    {
        Conf.C.TypingIndicators = newValue;
    }

    protected override void DrawPreview(SpriteBatch sb)
    {
        if (!Value) return;

        var dims = GetDimensions();
        Vector2 pos = new(dims.X + 175 - 3, dims.Y+3);


        var tex = Ass.TypingIndicator; // w: 32, h: 26, frames: 10
        int frame = 2; 
        Rectangle rect = new(32 * frame, 0, 32, 26);
        sb.Draw(Ass.TypingIndicator.Value, pos, 
            rect, Color.White, 0f, Vector2.Zero, 0.9f, 0f, 0f);

        return;

        string playerName = "a";
        var player = Main.LocalPlayer;

        // Fallback if no player found
        if (player == null || !player.active || player.name == "" || Main.gameMenu)
        {
            playerName = "Player";
        }
        else
        {
            playerName = Main.LocalPlayer.name;
        }

        // Colorized player name
        pos += new Vector2(-0, 0);
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

        if (playerName == "Player")
        {
            // Read live config value
            hex = PlayerColorConfigElement.Instance?.LiveValue ?? "FFFFFF";
            //Log.Info("live: " + hex);
        }

        var coloredSnips = ($"[c/{hex}:{playerName}]");
        ChatManager.DrawColorCodedStringWithShadow(sb, FontAssets.ItemStack.Value, 
            Loc.Get("TypingIndicators.IsTyping", coloredSnips), pos, Color.White, 0f, Vector2.Zero, new Vector2(0.85f));
    }
}