using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.UI.Chat;
using ChatPlus.Core.Features.PlayerColors;
using ChatPlus.Core.Features.PlayerIcons.PlayerInfo;

namespace ChatPlus.Core.Features.Mentions;
public sealed class MentionSnippet : TextSnippet
{
    private bool isHovered;
    private static readonly Dictionary<string, string> _nameToHex = new(StringComparer.Ordinal);
    private static readonly Dictionary<string, int> _nameToIndex = new(StringComparer.Ordinal);

    public MentionSnippet(TextSnippet src) : base(src.Text, src.Color, src.Scale)
    {
        CheckForHover = true;
    }

    public static void InvalidateCachesFor(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return;
        _nameToHex.Remove(name);
        _nameToIndex.Remove(name);
    }
    public static void ClearAllCaches()
    {
        _nameToHex.Clear();
        _nameToIndex.Clear();
    }

    private static int ResolveIndex(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return -1;
        if (_nameToIndex.TryGetValue(name, out int idx))
        {
            if ((uint)idx < Main.maxPlayers && Main.player[idx]?.active == true && Main.player[idx].name == name)
                return idx;
        }
        for (int i = 0; i < Main.maxPlayers; i++)
        {
            var p = Main.player[i];
            if (p?.active == true && p.name == name)
            {
                _nameToIndex[name] = i;
                return i;
            }
        }
        _nameToIndex[name] = -1;
        return -1;
    }

    private static string ResolveHex(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return "FFFFFF";
        if (_nameToHex.TryGetValue(name, out var hex)) return hex;

        // Try synced table first
        for (int i = 0; i < Main.maxPlayers; i++)
        {
            var p = Main.player[i];
            if (p?.active == true && p.name == name &&
                AssignPlayerColorsSystem.PlayerColors.TryGetValue(i, out var synced) &&
                !string.IsNullOrWhiteSpace(synced))
            {
                _nameToHex[name] = synced;
                return synced;
            }
        }

        // Fallback deterministic
        var fallback = PlayerColorHandler.HexFromName(name);
        _nameToHex[name] = fallback;
        return fallback;
    }

    public override bool UniqueDraw(bool justCheckingString, out Vector2 size, SpriteBatch sb, Vector2 pos = default, Color passColor = default, float scale = 1f)
    {
        string name = Text ?? string.Empty;
        string display = "@" + name;

        var font = FontAssets.MouseText.Value;
        size = font.MeasureString(display) * scale;
        if (justCheckingString) return true;
        if (passColor.R + passColor.G + passColor.B <= 5) return true; // skip shadow pass

        // color by cached hex
        string hex = ResolveHex(name);
        Color textColor = Color.White;
        if (hex.Length == 6 &&
            byte.TryParse(hex[..2], System.Globalization.NumberStyles.HexNumber, null, out var r) &&
            byte.TryParse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber, null, out var g) &&
            byte.TryParse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber, null, out var b))
            textColor = new Color(r, g, b);

        Vector2 p = new((float)System.Math.Floor(pos.X), (float)System.Math.Floor(pos.Y));
        sb.DrawString(font, display, p + new Vector2(1, 1), new Color(0, 0, 0, 160), 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        sb.DrawString(font, display, p, textColor, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

        // hover rect like links
        int width = (int)System.Math.Ceiling(size.X);
        int lineH = (int)System.Math.Ceiling(font.LineSpacing * scale);
        var hoverRect = new Rectangle((int)p.X, (int)p.Y, width, System.Math.Max(1, lineH - 7));
        isHovered = hoverRect.Contains(Main.MouseScreen.ToPoint());

        if (isHovered)
        {
            Main.LocalPlayer.mouseInterface = true;
            int underlineY = (int)System.Math.Floor(p.Y + lineH - 9f);
            // slightly darker underline
            Color ul = new Color((byte)(textColor.R * 0.85f), (byte)(textColor.G * 0.85f), (byte)(textColor.B * 0.85f));
            sb.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)p.X, underlineY, width, 1), ul);

            // drive the topmost overlay
            HoveredPlayerOverlay.Set(ResolveIndex(name));
        }

        return true;
    }
}

