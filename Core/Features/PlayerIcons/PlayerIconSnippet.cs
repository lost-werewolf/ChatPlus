using ChatPlus.Common.Configs;
using ChatPlus.Core.Features.ModIcons.ModInfo;
using ChatPlus.Core.Features.PlayerIcons
.PlayerInfo;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace ChatPlus.Core.Features.PlayerIcons
;

/// <summary>
/// Inline snippet that draws a player's head icon.
/// </summary>
public class PlayerIconSnippet : TextSnippet
{
    private readonly int _playerIndex;

    public PlayerIconSnippet(int idx)
    {
        _playerIndex = idx;
        CheckForHover = true;
    }

    public override bool UniqueDraw(bool justCheckingString, out Vector2 size, SpriteBatch sb,
    Vector2 pos = default, Color color = default, float scale = 1f)
    {
        const float box = 26f;
        scale = 0.75f;
        pos.X -= 7f;

        size = new Vector2(box * scale - 3.5f, box * scale);

        if (justCheckingString || color == Color.Black) return true;

        if (_playerIndex < 0 || _playerIndex >= Main.maxPlayers) return true;
        Player player = Main.player[_playerIndex];
        if (player == null || !player.active) return true;

        // draw head
        sb.End();
        sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.UIScaleMatrix);
        pos = new Vector2(pos.X + 8f, pos.Y + 8f);
        MapHeadRendererHook.shouldFlipHeadDraw = player.direction == -1;
        Main.MapPlayerRenderer.DrawPlayerHead(Main.Camera, player, pos, 1f, scale, Color.White);
        MapHeadRendererHook.shouldFlipHeadDraw = false;

        //// debug
        //Rectangle debugRect = new((int)pos.X, (int)pos.Y, (int)size.X, (int)size.Y);
        //sb.Draw(TextureAssets.MagicPixel.Value, debugRect, Color.Red * 0.25f);

        // hover
        int width = (int)size.X;
        //int nameWidth = (int)FontAssets.MouseText.Value.MeasureString(player.name).X;
        //width += nameWidth + 10;
        var hoverRect = new Rectangle((int)pos.X - 10, (int)pos.Y - 6, width + 3, (int)size.Y + 3);
        
        // to debug; comment below line out!
        if (hoverRect.Contains(Main.MouseScreen.ToPoint()))
        {
            if (!Conf.C.ShowStatsWhenHovering) 
                return true;

            if (Conf.C.DisableStatsWhenBossIsAlive && Main.CurrentFrameFlags.AnyActiveBossNPC)
                return true;

            Main.LocalPlayer.mouseInterface = true;

            HoveredPlayerOverlay.Set(_playerIndex);

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

        Main.LocalPlayer.mouseInterface = true;

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
            if (!Conf.C.ShowStatsWhenHovering) 
                return;

            if (Conf.C.DisableStatsWhenBossIsAlive && Main.CurrentFrameFlags.AnyActiveBossNPC)
                return;

            HoveredPlayerOverlay.Set(_playerIndex);
        }
    }
}
