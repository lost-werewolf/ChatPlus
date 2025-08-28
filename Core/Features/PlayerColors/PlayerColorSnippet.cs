using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI.Chat;
using ChatPlus.Core.Helpers;

namespace ChatPlus.Core.Features.PlayerColors;

public class PlayerColorSnippet : TextSnippet
{
    private static readonly Dictionary<int, Color> colorByPlayer = [];
    private static readonly Dictionary<int, string> hexByPlayer = [];

    public PlayerColorSnippet(TextSnippet inner) : base(inner.Text, inner.Color, inner.Scale)
    {
        CheckForHover = inner.CheckForHover;
        DeleteWhole = inner.DeleteWhole;
    }

    public override void Update() { }
    public override bool UniqueDraw(bool justCheckingString, out Vector2 size, SpriteBatch spriteBatch, Vector2 position = default, Color color = default, float scale = 1) { size = default; return true; }

    public static Color GetColor(int whoAmI)
    {
        if (colorByPlayer.TryGetValue(whoAmI, out var cached)) return cached;

        var palette = ColorHelper.PlayerColors;
        if (palette == null || palette.Length == 0) return Color.White;

        int idx = whoAmI;
        var p = (whoAmI >= 0 && whoAmI < Main.maxPlayers) ? Main.player[whoAmI] : null;
        if (p != null && p.active && !string.IsNullOrEmpty(p.name))
        {
            int h = 17;
            foreach (var ch in p.name) unchecked { h = h * 31 + ch; }
            if (h == int.MinValue) h = 0;
            idx = h & int.MaxValue;
        }
        idx %= palette.Length;

        var c = palette[idx];
        colorByPlayer[whoAmI] = c;
        return c;
    }

    public static string GetHex(int whoAmI)
    {
        if (hexByPlayer.TryGetValue(whoAmI, out var hex)) return hex;
        var c = GetColor(whoAmI);
        hex = $"{c.R:X2}{c.G:X2}{c.B:X2}".ToLowerInvariant();
        hexByPlayer[whoAmI] = hex;
        return hex;
    }
}
