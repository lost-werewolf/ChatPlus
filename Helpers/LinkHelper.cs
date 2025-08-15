using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using AdvancedChatFeatures.Common.Snippets;
using Terraria;
using Terraria.GameContent.UI.Chat;
using Terraria.UI.Chat;

namespace AdvancedChatFeatures.Helpers
{
    public static class LinkHelper
    {
        public static void ProcessLink(TextSnippet[] snippets, RemadeChatMonitor self)
        {
            for (int i = 1; i < snippets.Length; i++) // skip player name
            {
                var snip = snippets[i];
                if (IsWholeLink(snip.Text.Trim()) && snip is not LinkSnippet)
                {
                    snippets[i] = new LinkSnippet(snip);
                }
            }

            // update parsed text
            var container = self._messages[0];
            container._parsedText[0] = snippets;
        }

        /// <summary>
        /// Returns true if the string is a valid link.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool IsWholeLink(string text)
        {
            return Regex.IsMatch(text, @"^(https?://|www\.)\S+\.\S+$", RegexOptions.IgnoreCase);
        }

        public static void OpenURL(string url)
        {
            if (string.IsNullOrEmpty(url))
                return;

            try
            {
                // Start a process to open the URL in a web browser
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
            catch (Exception e)
            {
                Main.NewText($"Failed to open URL: {url}" + e.Message, Color.Red);
                Log.Error($"Failed to open URL: {url}" + e);
            }
        }
    }
}
