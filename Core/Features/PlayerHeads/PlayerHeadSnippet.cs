using ChatPlus.Common.Configs;
using ChatPlus.Core.Features.ModIcons.ModInfo;
using ChatPlus.Core.Features.PlayerHeads.PlayerInfo;
using ChatPlus.Core.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader.UI;
using Terraria.UI;
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

        //PlayerInfoDrawer.Draw(Main.spriteBatch, player); // debug

        return true;
    }

    public override void OnClick()
    {
        base.OnClick();

        if (_playerIndex < 0 || _playerIndex >= Main.maxPlayers) return;
        var plr = Main.player[_playerIndex];
        if (plr == null || !plr.active) return;

        var state = PlayerInfoState.instance;
        if (state == null)
        {
            Main.NewText("Player info UI not available.", Color.Orange);
            return;
        }

        // Snapshot current chat so the info UI can restore it later
        var snap = ChatSession.Capture();                

        state.SetPlayer(_playerIndex, plr.name);          // tell the UI which player to show
        state.SetReturnSnapshot(snap);                    // so Back can restore chat/session

        Main.drawingPlayerChat = false;                   // hide chat while the modal is open (optional)
        IngameFancyUI.OpenUIState(state);                 // open the "view more" UI
    }

    public override void OnHover()
    {
        if (_playerIndex >= 0 && _playerIndex < Main.maxPlayers)
        {
            Player player = Main.player[_playerIndex];
            if (player?.active == true && Conf.C.ShowPlayerPreviewWhenHovering)
            {
                PlayerInfoDrawer.Draw(Main.spriteBatch, player);
            }
        }
    }
}
