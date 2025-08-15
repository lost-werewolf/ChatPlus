using System.Collections.Generic;
using Terraria.UI.Chat;

namespace AdvancedChatFeatures.Helpers
{
    public static class PlayerColorHelper
    {
        private static readonly Dictionary<string, Color> PlayerColors = new();
        private static int nextIndex;

        public static void Process(TextSnippet nameSnippet, string playerName)
        {
            if (!PlayerColors.TryGetValue(playerName, out var color))
            {
                color = ColorHelper.PlayerColors[nextIndex++ % ColorHelper.PlayerColors.Length];
                PlayerColors[playerName] = color;
            }
            nameSnippet.Color = color;
        }
    }
}
