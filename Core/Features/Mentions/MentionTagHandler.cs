using System.Text.RegularExpressions;
using ChatPlus.Core.Features.Links;
using Terraria.UI.Chat;

namespace ChatPlus.Core.Features.Mentions;
internal class MentionTagHandler : ITagHandler
{
    public static string GenerateTag(string linkText)
    { 
        return $"[mention:{linkText}]";
    }

    public static bool ContainsLink(string text)
    {
        return Regex.IsMatch(text, @"(https?://|www\.)\S+\.\S+", RegexOptions.IgnoreCase);
    }

    public static bool TryGetLink(string input, out string link)
    {
        var match = Regex.Match(input, @"(https?://|www\.)\S+\.\S+", RegexOptions.IgnoreCase);
        if (match.Success)
        {
            link = match.Value;
            return true;
        }
        link = null;
        return false;
    }

    public TextSnippet Parse(string text, Color baseColor = default, string options = null)
    {
        if (ContainsLink(text))
        {
            return new LinkSnippet(new TextSnippet(text, baseColor));
        }
        else
        {
            //return new LinkSnippet(new TextSnippet(text, baseColor));
            return new TextSnippet(text);
        }
    }
}
