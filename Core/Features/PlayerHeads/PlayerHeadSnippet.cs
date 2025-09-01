using ChatPlus.Common.Configs;
using ChatPlus.Core.Features.ModIcons.ModInfo;
using ChatPlus.Core.Features.PlayerHeads.PlayerInfo;
using ChatPlus.Core.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Chat;
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

    public override bool UniqueDraw(bool justCheckingString, out Vector2 size, SpriteBatch sb,
    Vector2 position = default, Color color = default, float scale = 1f)
    {
        const float box = 26f;
        scale = 0.75f;

        size = new Vector2(box * scale + 9f, box * scale);

        if (justCheckingString || color == Color.Black) return true;

        if (_playerIndex < 0 || _playerIndex >= Main.maxPlayers) return true;
        Player player = Main.player[_playerIndex];
        if (player == null || !player.active) return true;

        // draw head
        sb.End();
        sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.UIScaleMatrix);
        var pos = new Vector2(position.X + 8f, position.Y + 8f);
        PlayerHeadFlipHook.shouldFlipHeadDraw = player.direction == -1;
        Main.MapPlayerRenderer.DrawPlayerHead(Main.Camera, player, pos, 1f, scale, Color.White);
        PlayerHeadFlipHook.shouldFlipHeadDraw = false;

        // hover
        int width = (int)size.X;
        int nameWidth = (int)FontAssets.MouseText.Value.MeasureString(player.name).X;
        width += nameWidth + 10;
        var hoverRect = new Rectangle((int)position.X, (int)position.Y, width, (int)size.Y);
        if (hoverRect.Contains(Main.MouseScreen.ToPoint()))
        {
            //if (Conf.C.ShowPlayerPreviewWhenHovering)
                //PlayerInfoDrawer.Draw(sb, player);

            if (Main.mouseLeft && Main.mouseLeftRelease)
                OnClick();
        }

        // debug
        //hoverRect.X = 110;
        //hoverRect.Width = nameWidth;
        //sb.Draw(TextureAssets.MagicPixel.Value, hoverRect, Color.Red * 0.25f);

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
        Main.LocalPlayer.mouseInterface = true;

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
