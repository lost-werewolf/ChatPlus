using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using ChatPlus.Common.Snippets;
using ChatPlus.Helpers;
using ChatPlus.UploadHandler;
using Microsoft.Xna.Framework;
using MonoMod.RuntimeDetour;
using MonoMod.RuntimeDetour.HookGen;
using Terraria;
using Terraria.GameContent.UI.Chat;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace ChatPlus.Common.Systems;

internal class AddNewMessageSystem : ModSystem
{
    public override void Load()
    {
        On_RemadeChatMonitor.AddNewMessage += RemadeChatMonitor_AddNewMessage;
    }
    public override void Unload()
    {
        On_RemadeChatMonitor.AddNewMessage -= RemadeChatMonitor_AddNewMessage;
    }

    private void RemadeChatMonitor_AddNewMessage(On_RemadeChatMonitor.orig_AddNewMessage orig, RemadeChatMonitor self, string text, Color color, int widthLimitInPixels)
    {
        if (!string.IsNullOrEmpty(text) && System.Text.RegularExpressions.Regex.IsMatch(text, @"\[u:[^\]]+\]", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
        {
            text += string.Concat(Enumerable.Repeat("\n", 9)); // 9 is MAXIMUM!
        }

        self._showCount = 15;

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

            Log.Info("Chat Message: " + snip.Text.Trim(), printCallerInMessage: false);

            if (LinkSnippet.IsWholeLink(snip.Text.Trim()) && snip is not LinkSnippet)
            {
                // Swap it in place with a LinkSnippet
                line[i] = new LinkSnippet(snip);
            }
        }
    }
}
