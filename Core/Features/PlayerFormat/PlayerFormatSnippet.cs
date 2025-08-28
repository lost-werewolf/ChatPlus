using ChatPlus.Common.Configs;
using Microsoft.Xna.Framework;
using Terraria.UI.Chat;

namespace ChatPlus.Core.Features.PlayerFormat;

public class PlayerFormatSnippet : TextSnippet
{
    public PlayerFormatSnippet(TextSnippet src) : base(Normalize(src.Text), src.Color, src.Scale) { }

    public static bool LooksLikeName(string t) =>
        !string.IsNullOrWhiteSpace(t) && (t.StartsWith("[n:", System.StringComparison.OrdinalIgnoreCase) || (t.Length >= 3 && t[0] == '<' && t[^1] == '>'));

    public static bool TryExtractName(string t, out string name)
    {
        t = t?.Trim() ?? "";
        if (t.StartsWith("[n:", System.StringComparison.OrdinalIgnoreCase))
        {
            int close = t.IndexOf(']');
            if (close > 3) { name = t.Substring(3, close - 3); return true; }
        }
        if (t.Length >= 3 && t[0] == '<' && t[^1] == '>') { name = t.Substring(1, t.Length - 2); return true; }
        name = null; return false;
    }

    private static string Normalize(string t)
    {
        if (!TryExtractName(t, out var name)) return t?.Trim() ?? "";
        var pref = Conf.C?.PlayerFormat ?? "<PlayerName>"; // "<PlayerName>" or "PlayerName:"
        return string.Equals(pref, "PlayerName:", System.StringComparison.Ordinal) ? name + ":" : "<" + name + ">";
    }
}
