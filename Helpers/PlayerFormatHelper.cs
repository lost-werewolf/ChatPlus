using Terraria.UI.Chat;

namespace AdvancedChatFeatures.Helpers
{
    public static class PlayerFormatHelper
    {
        public static string Strip(string rawName)
        {
            return rawName.Trim('<', '>', ' ');
        }

        public static void Apply(ref TextSnippet nameSnippet, string playerName, string format)
        {
            if (format == "<PlayerName>")
                nameSnippet.Text = $"<{playerName}>";
            else if (format == "PlayerName:")
                nameSnippet.Text = $"{playerName}:";
            else if (format == "(PlayerName)")
                nameSnippet.Text = $"({playerName})";
        }

        public static string GetFormattedName(string playerName, string format)
        {
            return format switch
            {
                "<PlayerName>" => $"<{playerName}>",
                "PlayerName:" => $"{playerName}:",
                "(PlayerName)" => $"({playerName})",
                _ => $"<{playerName}>"
            };
        }
    }

}
