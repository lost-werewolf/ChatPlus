using ChatPlus.Core.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
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
        const int dim = 20;
        size = new Vector2(dim * scale, dim * scale);
        if (justCheckingString) return true;

        if (_playerIndex >= 0 && _playerIndex < Main.maxPlayers)
        {
            position += new Vector2(9, 9);
            DrawPlayerHead(position);
        }
        return true;
    }

    private void DrawPlayerHead(Vector2 position)
    {
        if (Main.LocalPlayer == null || Main.Camera == null || _playerIndex < 0 || _playerIndex >= Main.maxPlayers) { 
            Log.Error("Oof invalid draw for player head"); 
            return; 
        }

        var player = Main.player[_playerIndex];
        if (player == null || !player.active) { 
            Log.Error("Oof invalid player"); 
            return; 
        }

        PlayerHeadFlipHook.shouldFlipHeadDraw = player.direction == -1;
        Main.MapPlayerRenderer.DrawPlayerHead(Main.Camera, player, position, 1f, 0.8f, Color.White);
        PlayerHeadFlipHook.shouldFlipHeadDraw = false;
    }

    public override void OnHover()
    {
        if (_playerIndex >= 0 && _playerIndex < Main.maxPlayers)
        {
            var p = Main.player[_playerIndex];
            if (p?.active == true)
                Main.instance.MouseText(p.name);
        }
    }
}
