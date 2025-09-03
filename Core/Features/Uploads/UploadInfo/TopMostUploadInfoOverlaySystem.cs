using System;
using System.Collections.Generic;
using ChatPlus.Core.Features.Uploads.UploadInfo;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;

namespace ChatPlus.Core.Features.Uploads.UploadInfo;

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
            () =>
            {
                var upload = HoveredUploadOverlay.Consume();
                if (upload.HasValue && upload.Value.Texture != null)
                {
                    DrawUploadPreview(Main.spriteBatch, upload.Value);
                }
                return true;
            },
            InterfaceScaleType.UI));
    }

    private static void DrawUploadPreview(SpriteBatch sb, Upload upload)
    {
        var tex = upload.Texture;
        if (tex == null) return;

        // Position near mouse, clamp on screen
        Vector2 pos = Main.MouseScreen + new Vector2(20, 20);
        int maxDim = 180; // preview box
        float scale = Math.Min(maxDim / (float)tex.Width, maxDim / (float)tex.Height);
        int w = (int)(tex.Width * scale);
        int h = (int)(tex.Height * scale);

        // Panel rect (some padding + room for filename at bottom)
        int pad = 8;
        int textH = 22;
        Rectangle panelRect = new((int)pos.X, (int)pos.Y, w + pad * 2, h + pad * 2 + textH);
        // Clamp
        panelRect.X = Math.Clamp(panelRect.X, 0, Main.screenWidth - panelRect.Width);
        panelRect.Y = Math.Clamp(panelRect.Y, 0, Main.screenHeight - panelRect.Height);

        // Draw background (reuse vanilla panel textures via nine-slice simplified)
        DrawPanelBackground(sb, panelRect, new Color(30, 40, 90, 230));

        // Draw texture centered
        Vector2 imgPos = new(panelRect.X + pad + (panelRect.Width - pad * 2 - w) / 2f, panelRect.Y + pad);
        sb.Draw(tex, new Rectangle((int)imgPos.X, (int)imgPos.Y, w, h), Color.White);

        // Filename (truncate)
        string name = upload.FileName;
        if (name.Length > 24) name = name[..21] + "...";
        var snippets = Terraria.UI.Chat.ChatManager.ParseMessage(name, Color.White).ToArray();
        Vector2 textSize = Terraria.UI.Chat.ChatManager.GetStringSize(FontAssets.MouseText.Value, name, Vector2.One);
        Vector2 textPos = new(panelRect.X + (panelRect.Width - textSize.X) / 2f, panelRect.Bottom - pad - textSize.Y);
        Terraria.UI.Chat.ChatManager.DrawColorCodedStringWithShadow(sb, FontAssets.MouseText.Value, snippets, textPos, 0f, Vector2.Zero, Vector2.One, out _);
    }

    private static void DrawPanelBackground(SpriteBatch sb, Rectangle rect, Color bg)
    {
        var tex = TextureAssets.MagicPixel.Value;
        sb.Draw(tex, rect, bg);
        // simple border
        var border = new Rectangle(rect.X - 1, rect.Y - 1, rect.Width + 2, rect.Height + 2);
        DrawBorder(sb, border, new Color(89, 116, 213, 255));
    }

    private static void DrawBorder(SpriteBatch sb, Rectangle rect, Color col)
    {
        var px = TextureAssets.MagicPixel.Value;
        sb.Draw(px, new Rectangle(rect.X, rect.Y, rect.Width, 2), col);
        sb.Draw(px, new Rectangle(rect.X, rect.Bottom - 2, rect.Width, 2), col);
        sb.Draw(px, new Rectangle(rect.X, rect.Y, 2, rect.Height), col);
        sb.Draw(px, new Rectangle(rect.Right - 2, rect.Y, 2, rect.Height), col);
    }
}
