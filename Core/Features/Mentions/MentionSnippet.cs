using System;
using System.Collections.Generic;
using ChatPlus.Core.Features.ModIcons.ModInfo;
using ChatPlus.Core.Features.PlayerColors;
using ChatPlus.Core.Features.PlayerIcons.PlayerInfo;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.UI;
using Terraria.UI.Chat;

namespace ChatPlus.Core.Features.Mentions;

public sealed class MentionSnippet : TextSnippet
{
    private bool isHovered;

    // cache maps (case-insensitive)
    private static readonly Dictionary<string, string> _nameToHex = new(StringComparer.OrdinalIgnoreCase);
    private static readonly Dictionary<string, int> _nameToIndex = new(StringComparer.OrdinalIgnoreCase);

    // per-snippet last resolved info for click handling
    private int _lastIndex = -1;
    private string _lastName = string.Empty;

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
            if ((uint)idx < Main.maxPlayers &&
                Main.player[idx]?.active == true &&
                string.Equals(Main.player[idx].name, name, StringComparison.OrdinalIgnoreCase))
                return idx;
        }

        for (int i = 0; i < Main.maxPlayers; i++)
        {
            var p = Main.player[i];
            if (p?.active == true && string.Equals(p.name, name, StringComparison.OrdinalIgnoreCase))
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

        // prefer synced table
        for (int i = 0; i < Main.maxPlayers; i++)
        {
            var p = Main.player[i];
            if (p?.active == true &&
                string.Equals(p.name, name, StringComparison.OrdinalIgnoreCase) &&
                AssignPlayerColorsSystem.PlayerColors.TryGetValue(i, out var synced) &&
                !string.IsNullOrWhiteSpace(synced))
            {
                _nameToHex[name] = synced;
                return synced;
            }
        }

        var fallback = PlayerColorHandler.HexFromName(name);
        _nameToHex[name] = fallback;
        return fallback;
    }

    public override bool UniqueDraw(
        bool justCheckingString,
        out Vector2 size,
        SpriteBatch sb,
        Vector2 pos = default,
        Color passColor = default,
        float scale = 1f)
    {
        string name = Text ?? string.Empty;
        _lastName = name;
        _lastIndex = ResolveIndex(name);

        string display = "@" + name;

        var font = FontAssets.MouseText.Value;
        size = font.MeasureString(display) * scale;
        if (justCheckingString) return true;

        // skip shadow pass
        if (passColor.R + passColor.G + passColor.B <= 5)
            return true;

        // text color from cached hex
        string hex = ResolveHex(name);
        Color textColor = Color.White;
        if (hex.Length == 6 &&
            byte.TryParse(hex[..2], System.Globalization.NumberStyles.HexNumber, null, out var r) &&
            byte.TryParse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber, null, out var g) &&
            byte.TryParse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber, null, out var b))
            textColor = new Color(r, g, b);

        Vector2 p = new((float)Math.Floor(pos.X), (float)Math.Floor(pos.Y));

        // outline
        sb.DrawString(font, display, p + new Vector2(1, 1), new Color(0, 0, 0, 160), 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

        // bold if this client is the mentioned player
        bool isLocalTarget =
            !string.IsNullOrWhiteSpace(Main.LocalPlayer?.name) &&
            string.Equals(name, Main.LocalPlayer.name, StringComparison.OrdinalIgnoreCase);

        if (isLocalTarget)
            sb.DrawString(font, display, p + new Vector2(1, 0), textColor, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

        // base text
        sb.DrawString(font, display, p, textColor, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

        // hover (like links)
        int width = (int)Math.Ceiling(size.X);
        int lineH = (int)Math.Ceiling(font.LineSpacing * scale);
        var hoverR = new Rectangle((int)p.X, (int)p.Y, width, Math.Max(1, lineH - 7));

        isHovered = hoverR.Contains(Main.MouseScreen.ToPoint());
        if (isHovered)
        {
            Main.LocalPlayer.mouseInterface = true;

            // underline slightly darker than text color
            Color ul = new((byte)(textColor.R * 0.85f), (byte)(textColor.G * 0.85f), (byte)(textColor.B * 0.85f));
            int underlineY = (int)Math.Floor(p.Y + lineH - 9f);
            sb.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)p.X, underlineY, width, 1), ul);

            // topmost overlay + click
            HoveredPlayerOverlay.Set(_lastIndex);
            if (Main.mouseLeft && Main.mouseLeftRelease)
                OnClick();   // uses _lastIndex resolved above
        }

        return true;
    }

    public override void OnClick()
    {
        Main.LocalPlayer.mouseInterface = true;

        int idx = _lastIndex;
        if (idx < 0 || idx >= Main.maxPlayers) return;

        var plr = Main.player[idx];
        if (plr == null || !plr.active) return;

        var state = PlayerInfoState.instance;
        if (state == null)
        {
            Main.NewText("Player info UI not available.", Color.Orange);
            return;
        }

        // snapshot chat so the UI can restore it later
        var snap = ChatSession.Capture();

        state.SetPlayer(idx, plr.name);
        state.SetReturnSnapshot(snap);

        Main.drawingPlayerChat = false;       
        IngameFancyUI.OpenUIState(state);
    }
}
