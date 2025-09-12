﻿using System;
using System.Diagnostics;
using ChatPlus.Core.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.UI.Chat;

namespace ChatPlus.Core.Features.Links;

public class LinkSnippet : TextSnippet
{
    private bool isHovered;
    private int lastUnderlineDrawFrame;
    public LinkSnippet(TextSnippet src) : base(src.Text, src.Color, src.Scale)
    {
        CheckForHover = true;
    }

    public override Color GetVisibleColor()
    {
        if (isHovered) return new Color(6, 69, 173);

        return new Color(0, 125, 255);
    }

    public override void OnHover()
    {
        // No-op; we compute hover ourselves each draw.
    }

    public override bool UniqueDraw(bool justCheckingString, out Vector2 size,
        SpriteBatch sb, Vector2 pos = default, Color passColor = default, float scale = 1f)
    {
        string text = Text ?? string.Empty;
        var font = FontAssets.MouseText.Value;

        // Use ChatManager to match vanilla layout width
        size = ChatManager.GetStringSize(font, text, new Vector2(scale));
        if (justCheckingString)
        {
            return false;
        }

        // Shadow passes are drawn in near-black; skip them to avoid multiple underlines
        bool isShadowPass = passColor.R + passColor.G + passColor.B <= 5;
        if (isShadowPass)
        {
            return false;
        }

        // Snap to pixels to avoid drift
        Vector2 p = new Vector2((float)Math.Floor(pos.X), (float)Math.Floor(pos.Y));

        int width = (int)System.Math.Ceiling(size.X);
        int lineHeight = (int)System.Math.Ceiling(font.LineSpacing * scale);

        var hoverRect = new Rectangle((int)p.X, (int)p.Y-0, width, lineHeight-7);
        isHovered = hoverRect.Contains(Main.MouseScreen.ToPoint());

        // debug draw
        //sb.Draw(TextureAssets.MagicPixel.Value, hoverRect, Color.Red * 0.1f);

        if (isHovered)
        {
            Main.LocalPlayer.mouseInterface = true;

            // Draw a single 1px underline for this visible segment
            int underlineY = (int)System.Math.Floor(p.Y + lineHeight - 9f);
            var underlineRect = new Rectangle((int)p.X, underlineY, width, 1);
            sb.Draw(TextureAssets.MagicPixel.Value, underlineRect, GetVisibleColor());

            if (Main.mouseLeft && Main.mouseLeftRelease)
            {
                Main.mouseLeftRelease = false;
                OpenLink(Text);
            }
        }

        // Let vanilla draw glyphs using our GetVisibleColor()
        return false;
    }

    public override void OnClick()
    {
        base.OnClick();
    }

    public void OpenLink(string url)
    {
        try
        {
            Process.Start(new ProcessStartInfo($@"{url}")
            {
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            Main.NewText("Failed to open link: " + ex.Message, Color.Red);
            Log.Error("Failed to open link: " + ex.Message);
        }
    }
}
