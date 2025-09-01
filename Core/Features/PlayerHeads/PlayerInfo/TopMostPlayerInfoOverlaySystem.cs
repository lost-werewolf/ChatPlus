using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace ChatPlus.Core.Features.PlayerHeads.PlayerInfo;

/// <summary>
/// Draws the hovered player info panel absolutely last (above every other interface layer).
/// </summary>
[Autoload(Side = ModSide.Client)]
public class TopMostPlayerInfoOverlaySystem : ModSystem
{
    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        // Insert at the very end to be above everything.
        layers.Add(new LegacyGameInterfaceLayer(
            "ChatPlus: PlayerInfoOverlay_TopMost",
            () =>
            {
                int idx = HoveredPlayerOverlay.Consume();
                if (idx >= 0 && idx < Main.maxPlayers)
                {
                    var player = Main.player[idx];
                    if (player?.active == true)
                    {
                        PlayerInfoDrawer.Draw(Main.spriteBatch, player);
                    }
                }
                return true;
            },
            InterfaceScaleType.UI));
    }
}
