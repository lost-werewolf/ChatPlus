using Microsoft.Xna.Framework;
using Terraria.UI.Chat;

namespace ChatPlus.Core.Features.PlayerFormat;

public class PlayerFormatSnippet : TextSnippet
{
    public PlayerFormatSnippet(TextSnippet src)
        : base(Normalize(src.Text), src.Color, src.Scale) { }

    public static bool LooksLikeName(string t) =>
        !string.IsNullOrWhiteSpace(t) && t.Length >= 3 && t[0] == '<' && t[^1] == '>';

    private static string Normalize(string t)
    {
        t = t?.Trim() ?? "";
        return LooksLikeName(t) ? t.Substring(1, t.Length - 2) + ":" : t;
    }
}
