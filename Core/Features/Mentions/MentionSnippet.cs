using System;
using System.Collections.Generic;
using System.Globalization;
using ChatPlus.Core.Features.PlayerColors;
using ChatPlus.Core.Features.Stats.Base;
using ChatPlus.Core.Features.Stats.PlayerStats;
using ChatPlus.Core.Helpers;
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
                PlayerColorSystem.PlayerColors.TryGetValue(i, out var synced) &&
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
        //Log.Info($"hex: {hex} for: {name}");
        Color textColor = Color.White;
        if (hex.Length == 6 &&
            byte.TryParse(hex[..2], NumberStyles.HexNumber, null, out var r) &&
            byte.TryParse(hex.Substring(2, 2), NumberStyles.HexNumber, null, out var g) &&
            byte.TryParse(hex.Substring(4, 2), NumberStyles.HexNumber, null, out var b))
            textColor = new Color(r, g, b);

        Vector2 p = new((float)Math.Floor(pos.X), (float)Math.Floor(pos.Y));

        // bold if this client is the mentioned player
        if (string.Equals(name, Main.LocalPlayer.name))
        {
            DrawColorCodedStringWithBOLD(sb, font,
                display, pos, textColor, 0f, Vector2.Zero, Vector2.One);
           
        }
        //Utils.DrawBorderString(sb, display, pos, textColor);

        // hover (like links)
        int width = (int)Math.Ceiling(size.X);
        int lineH = (int)Math.Ceiling(font.LineSpacing * scale);
        var hoverR = new Rectangle((int)p.X, (int)p.Y, width, Math.Max(1, lineH - 7));

        isHovered = hoverR.Contains(Main.MouseScreen.ToPoint());

        // to debug and always see hovered player overlay: comment the below line out
        if (isHovered)
        {
            Main.LocalPlayer.mouseInterface = true;

            // underline slightly darker than text color
            Color ul = new((byte)(textColor.R * 0.85f), (byte)(textColor.G * 0.85f), (byte)(textColor.B * 0.85f));
            int underlineY = (int)Math.Floor(p.Y + lineH - 10f);
            sb.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)p.X, underlineY, width, 2), ul);

            // topmost overlay + click
            HoveredPlayerOverlay.Set(_lastIndex);
            if (Main.mouseLeft && Main.mouseLeftRelease)
                OnClick();   // uses _lastIndex resolved above
        }

        return true;
    }

    public static Vector2 DrawColorCodedStringWithBOLD(SpriteBatch spriteBatch, DynamicSpriteFont font, string text, Vector2 position, Color baseColor, float rotation, Vector2 origin, Vector2 baseScale, float maxWidth = -1f, float spread = 2f)
    {
        TextSnippet[] snippets = ChatManager.ParseMessage(text, baseColor).ToArray();
        ChatManager.ConvertNormalSnippets(snippets);
        ChatManager.DrawColorCodedStringShadow(spriteBatch, font, snippets, position + Vector2.UnitX * spread, new Color(0, 0, 0, baseColor.A), rotation, origin, baseScale, maxWidth, spread);
        ChatManager.DrawColorCodedStringShadow(spriteBatch, font, snippets, position + Vector2.UnitY * spread, new Color(0, 0, 0, baseColor.A), rotation, origin, baseScale, maxWidth, spread);
        ChatManager.DrawColorCodedStringShadow(spriteBatch, font, snippets, position - Vector2.UnitX * spread, new Color(0, 0, 0, baseColor.A), rotation, origin, baseScale, maxWidth, spread);
        ChatManager.DrawColorCodedStringShadow(spriteBatch, font, snippets, position - Vector2.UnitY * spread, new Color(0, 0, 0, baseColor.A), rotation, origin, baseScale, maxWidth, spread);
        for (int i = 0; i < ChatManager.ShadowDirections.Length; i++)
        {
            //ChatManager.DrawColorCodedString(spriteBatch, font, text, position + ChatManager.ShadowDirections[i] * spread, Color.White, rotation, origin, baseScale, out int _, maxWidth, ignoreColors: false);
        }
        int hoveredSnippet;
        return ChatManager.DrawColorCodedString(spriteBatch, font, snippets, position, Color.White, rotation, origin, baseScale, out hoveredSnippet, maxWidth);
    }

    public override void OnClick()
    {
        Main.LocalPlayer.mouseInterface = true;

        int idx = _lastIndex;
        if (idx < 0 || idx >= Main.maxPlayers)
            return;

        var target = Main.player[idx];
        if (target == null || !target.active)
            return;

        if (target != null && !PlayerInfoDrawer.HasAccess(Main.LocalPlayer, target))
        {
            Main.NewText($"{target.name}'s stats is private.", Color.OrangeRed);
            return;
        }

        var state = PlayerInfoState.instance;
        if (state == null)
        {
            Main.NewText("Player info UI not available.", Color.Orange);
            return;
        }

        state.SetPlayer(idx, target.name);

        Main.drawingPlayerChat = false;
        state.OpenForCurrentContext();
    }
}