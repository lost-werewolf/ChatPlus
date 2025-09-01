using System.Collections.Generic;
using ChatPlus.Core.Features.ModIcons.ModInfo;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace ChatPlus.Core.Features.ModIcons;

/// <summary>
/// Draws hovered Mod info panel above every other interface layer.
/// </summary>
[Autoload(Side = ModSide.Client)]
public class TopMostModInfoOverlaySystem : ModSystem
{
    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        layers.Add(new LegacyGameInterfaceLayer(
            "ChatPlus: ModInfoOverlay_TopMost",
            () =>
            {
                var mod = HoveredModOverlay.Consume();
                if (mod != null)
                {
                    ModInfoDrawer.Draw(Main.spriteBatch, mod);
                }
                return true;
            },
            InterfaceScaleType.UI));
    }
}
