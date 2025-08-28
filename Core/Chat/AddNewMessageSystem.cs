using System.Linq;
using System.Text.RegularExpressions;
using ChatPlus.Common.Configs;
using ChatPlus.Core.Features.Links;
using ChatPlus.Core.Features.PlayerFormat; // <-- add this
using Terraria;
using Terraria.GameContent.UI.Chat;
using Terraria.GameContent.UI.Elements;
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

    private void AddMessageDetour(On_RemadeChatMonitor.orig_AddNewMessage orig, RemadeChatMonitor self, string text, Color color, int widthLimitInPixels)
    {
        if (!string.IsNullOrEmpty(text) && Regex.IsMatch(text, @"\[u:[^\]]+\]", RegexOptions.IgnoreCase))
            text += string.Concat(Enumerable.Repeat("\n", 9));

        self._showCount = (int) Conf.C.ShowCount;

        orig(self, text, color, widthLimitInPixels);
        Intercept(self);
    }
    private void Intercept(RemadeChatMonitor self)
    {
        if (self._messages.Count == 0) return;
        var container = self._messages[0];
        if (container._parsedText.Count == 0) return;

        var line = container._parsedText[^1];

        bool nameDone = false;
        for (int i = 0; i < line.Length; i++)
        {
            var s = line[i];
            var t = s?.Text?.Trim();
            if (string.IsNullOrEmpty(t)) continue;

            if (!nameDone && PlayerFormatSnippet.LooksLikeName(t))
            {
                line[i] = CustomSnippet.Wrap(s); // becomes PlayerFormatSnippet
                nameDone = true;
                continue;
            }

            if (LinkSnippet.IsLink(t))
            {
                line[i] = CustomSnippet.Wrap(s); // becomes LinkSnippet
            }
        }
    }
}
