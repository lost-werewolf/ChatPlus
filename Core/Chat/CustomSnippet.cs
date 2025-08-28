using ChatPlus.Core.Features.Links;
using ChatPlus.Core.Features.PlayerFormat;
using Terraria.UI.Chat;

namespace ChatPlus.Core.Chat;
public static class CustomSnippet
{
    public static TextSnippet Wrap(TextSnippet s)
    {
        var t = s?.Text?.Trim() ?? "";

        if (PlayerFormatSnippet.LooksLikeName(t))
            return new PlayerFormatSnippet(s);

        if (LinkSnippet.IsLink(t))
            return new LinkSnippet(s);

        return s;
    }
}
