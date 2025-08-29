using ChatPlus.Core.Features.PlayerHeads.PlayerInfo;
using ChatPlus.Core.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader.UI;
using Terraria.UI.Chat;

namespace ChatPlus.Core.Features.PlayerHeads;

/// <summary>
/// Inline snippet that draws a player's head icon.
/// </summary>
public class PlayerHeadSnippet : TextSnippet
{
    private readonly int _playerIndex;

    public PlayerHeadSnippet(int idx)
    {
        _playerIndex = idx;
        CheckForHover = true;
    }

    public override bool UniqueDraw(bool justCheckingString, out Vector2 size, SpriteBatch spriteBatch, Vector2 position = default, Color color = default, float scale = 1f)
    {
        const float box = 26f;
        const float line = 20f;
        const float headScale = 0.75f;

        size = new Vector2(box * scale, box * scale);
        if (justCheckingString || color == Color.Black) return true;

        if (_playerIndex < 0 || _playerIndex >= Main.maxPlayers) return true;
        var player = Main.player[_playerIndex];
        if (player == null || !player.active) return true;

        float headPx = line * headScale * scale;
        float x = position.X + 8;
        float y = position.Y + 8; // same baseline lift as mod icon, centered in box

        PlayerHeadFlipHook.shouldFlipHeadDraw = player.direction == -1;
        Main.MapPlayerRenderer.DrawPlayerHead(Main.Camera, player, new Vector2(x, y), 1f, headScale * scale, Color.White);
        PlayerHeadFlipHook.shouldFlipHeadDraw = false;
        return true;
    }

    public override void OnHover()
    {
        if (_playerIndex >= 0 && _playerIndex < Main.maxPlayers)
        {
            Player player = Main.player[_playerIndex];
            if (player?.active == true)
            {
                UICommon.TooltipMouseText(player.name);
                PlayerInfoDrawer.Draw(Main.spriteBatch, player);
                //Main.instance.MouseText(player.name);
            }
        }
    }
}
