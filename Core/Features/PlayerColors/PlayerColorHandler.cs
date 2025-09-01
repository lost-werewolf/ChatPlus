using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ChatPlus.Core.Features.PlayerColors;
public static class PlayerColorHandler 
{
    public static bool TryGetNameTag(string input, out string output)
    {
        var match = Regex.Match(input, @"\[n:[^\]]+\]", RegexOptions.IgnoreCase);
        if (match.Success)
        {
            output = match.Value;
            return true;
        }
        output = null;
        return false;
    }

    public static string ColorToHex(Color c)
    {
        var r = c.R.ToString("X2", CultureInfo.InvariantCulture);
        var g = c.G.ToString("X2", CultureInfo.InvariantCulture);
        var b = c.B.ToString("X2", CultureInfo.InvariantCulture);
        return string.Concat(r, g, b);
    }

    public static Color HexToColor(string tag)
    {
        if (string.IsNullOrWhiteSpace(tag) || tag.Length < 8 || !tag.StartsWith("[c:", StringComparison.OrdinalIgnoreCase) || !tag.EndsWith("]"))
            return new Color(0, 0, 0) * 0f;

        // Expecting format [c:FF00FF]
        string hex = tag[3..].Replace(":", " ").Trim();

        if (hex.Length == 6) // RRGGBB
        {
            byte r = Convert.ToByte(hex[..2], 16);
            byte g = Convert.ToByte(hex.Substring(2, 2), 16);
            byte b = Convert.ToByte(hex.Substring(4, 2), 16);
            return new Color(r, g, b, 255);
        }
        return new Color(0, 0, 0) * 0f;
    }
    public static string HexFromName(string name)
    {
        if (string.IsNullOrEmpty(name)) return "FFFFFF";
        unchecked
        {
            int h = 17;
            for (int i = 0; i < name.Length; i++) h = h * 31 + name[i];
            float hue = ((h & 0xFFFF) / 65535f);   // 0..1
            var c = HsvToRgb(hue, 0.85f, 0.95f);   // high saturation, bright
            return $"{c.R:X2}{c.G:X2}{c.B:X2}";
        }
    }
    private static Microsoft.Xna.Framework.Color HsvToRgb(float h, float s, float v)
    {
        h = (h % 1f + 1f) % 1f;
        float i = (float)System.Math.Floor(h * 6f);
        float f = h * 6f - i;
        float p = v * (1f - s);
        float q = v * (1f - f * s);
        float t = v * (1f - (1f - f) * s);
        switch ((int)i % 6)
        {
            case 0: return new Microsoft.Xna.Framework.Color(v, t, p);
            case 1: return new Microsoft.Xna.Framework.Color(q, v, p);
            case 2: return new Microsoft.Xna.Framework.Color(p, v, t);
            case 3: return new Microsoft.Xna.Framework.Color(p, q, v);
            case 4: return new Microsoft.Xna.Framework.Color(t, p, v);
            default: return new Microsoft.Xna.Framework.Color(v, p, q);
        }
    }

}
