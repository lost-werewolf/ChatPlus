using Microsoft.Xna.Framework;
using Terraria.UI.Chat;

namespace ChatPlus.Core.Features.Mentions;

internal class MentionTagHandler : ITagHandler
{
    public static string GenerateTag(string playerName) => $"[mention:{playerName}]";

    public TextSnippet Parse(string text, Color baseColor = default, string options = null)
    {
        return new MentionSnippet(new TextSnippet(text, baseColor == default ? Color.White : baseColor));
    }
}
