using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;

namespace ChatPlus.Core.Features.Stats.UploadStats;

/// <summary>
/// Draws hovered upload preview (image + filename) above every other interface layer.
/// </summary>
[Autoload(Side = ModSide.Client)]
public class TopMostUploadInfoOverlaySystem : ModSystem
{
    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        layers.Add(new LegacyGameInterfaceLayer(
            "ChatPlus: UploadHoverOverlay_TopMost",
            Draw,
            InterfaceScaleType.UI));
    }

    public bool Draw()
    {
        var tex = HoveredUploadOverlay.Consume();
        if (tex != null)
        {
            //DrawUploadPreview(Main.spriteBatch, tex);
        }
        return true;
    }

    private static void DrawUploadPreview(SpriteBatch sb, Texture2D tex)
    {
        if (tex == null) return;

        Vector2 pos = Main.MouseScreen + new Vector2(20, 20);

        // start with base scale (fit to 180px box), then double it
        int maxDim = 180;
        float baseScale = Math.Min(maxDim / (float)tex.Width, maxDim / (float)tex.Height);
        float scale = baseScale * 1.5f;

        int w = (int)(tex.Width * scale);
        int h = (int)(tex.Height * scale);

        // shrink further if it would overflow screen bounds
        if (w > Main.screenWidth - 40 || h > Main.screenHeight - 40)
        {
            float clampScale = Math.Min(
                (Main.screenWidth - 40f) / tex.Width,
                (Main.screenHeight - 40f) / tex.Height
            );
            scale = clampScale;
            w = (int)(tex.Width * scale);
            h = (int)(tex.Height * scale);
        }

        int pad = 8;
        int textH = 22;
        Rectangle panelRect = new((int)pos.X, (int)pos.Y, w + pad * 2, h + pad * 2 + textH);

        // clamp the top-left corner so panel fits entirely
        panelRect.X = Math.Clamp(panelRect.X, 0, Main.screenWidth - panelRect.Width);
        panelRect.Y = Math.Clamp(panelRect.Y, 0, Main.screenHeight - panelRect.Height);

        Vector2 imgPos = new(panelRect.X + pad + (panelRect.Width - pad * 2 - w) / 2f, panelRect.Y + pad);
        sb.Draw(tex, new Rectangle((int)imgPos.X, (int)imgPos.Y, w, h), Color.White);
    }
}
