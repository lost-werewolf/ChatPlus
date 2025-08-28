using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using ChatPlus.Core.Features.Links;
using ChatPlus.Core.Features.Scrollbar;
using ChatPlus.Core.Helpers;
using Terraria;
using Terraria.Chat;
using Terraria.GameContent.UI.Chat;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
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

        orig(self, text, color, widthLimitInPixels);

        AddLineCounters(self);
        Intercept(self);
    }

    private void AddLineCounters(RemadeChatMonitor self)
    {
        var scrollSystem = ModContent.GetInstance<ChatScrollSystem>();
        var list = scrollSystem?.state?.chatScrollList;
        var container = self._messages[0];

        int linesInMessage = 1;
        try
        {
            var lineCountProp = container.GetType().GetProperty("LineCount", BindingFlags.Instance | BindingFlags.Public);
            linesInMessage = Math.Max(1, lineCountProp?.GetValue(container) as int? ?? container._parsedText?.Count ?? 1);
        }
        catch { }

        int startNumber = Math.Max(1, ScrollHelper.GetTotalLineCount() - linesInMessage + 1);
        bool wasAtBottom = list.ViewPosition >= list.GetTotalHeight() - list.GetInnerDimensions().Height - 1f;

        for (int i = 0; i < linesInMessage; i++)
        {
            var text = new UIText((startNumber + i).ToString())
            {
                HAlign = 0f,
                VAlign = 0f,
                Width = { Pixels = Main.screenWidth - 300 },
                Height = { Pixels = 21f }
            };
            list.Add(text);
        }

        list.Recalculate();
        if (wasAtBottom)
            list.ViewPosition = list.GetTotalHeight();
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
