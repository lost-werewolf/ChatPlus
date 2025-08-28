using System.Linq;
using System.Text.RegularExpressions;
using ChatPlus.Core.Features.Links;
using Terraria.GameContent.UI.Chat;
using Terraria.ModLoader;

namespace ChatPlus.Core.Chat;

internal class AddNewMessageSystem : ModSystem
{
    public override void Load()
    {
        On_RemadeChatMonitor.AddNewMessage += AddMessageDetour;
    }
    public override void Unload()
    {
        On_RemadeChatMonitor.AddNewMessage -= AddMessageDetour;
    }

    private void AddMessageDetour(On_RemadeChatMonitor.orig_AddNewMessage orig,RemadeChatMonitor self,string text,Color color,int widthLimitInPixels)
    {
        // Add 9 extra lines for uploads sent
        if (!string.IsNullOrEmpty(text) && Regex.IsMatch(text, @"\[u:[^\]]+\]", RegexOptions.IgnoreCase))
            text += string.Concat(Enumerable.Repeat("\n", 9)); // 9 is maximum

        self._showCount = 10;

        orig(self, text, color, widthLimitInPixels);

        Intercept(self);
    }

    private void Intercept(RemadeChatMonitor self)
    {
        if (self._messages.Count == 0)
            return;

        var container = self._messages[0];
        if (container._parsedText.Count == 0)
            return;

        // Grab the last line of the newest message
        var line = container._parsedText[^1];

        for (int i = 1; i < line.Length; i++) // skip [0] because it's usually the player name
        {
            var snip = line[i];
            if (snip == null || string.IsNullOrWhiteSpace(snip.Text))
                continue;

            //Log.Info("Chat Message: " + snip.Text.Trim(), printCallerInMessage: false);

            if (LinkSnippet.IsWholeLink(snip.Text.Trim()) && snip is not LinkSnippet)
            {
                // Swap it in place with a LinkSnippet
                line[i] = new LinkSnippet(snip);
            }
        }
    }
}
