using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using ChatPlus.Common.Configs;
using ChatPlus.Core.Features.Links;
using ChatPlus.Core.Features.ModIcons;
using ChatPlus.Core.Features.PlayerColors;
using ChatPlus.Core.Features.PlayerFormat; // <-- add this
using ChatPlus.Core.Features.PlayerHeads;
using ChatPlus.Core.Helpers;
using Terraria;
using Terraria.GameContent.UI.Chat;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI.Chat;

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
        bool looksLikePlayer = PlayerFormatSnippet.LooksLikeName(text?.TrimStart() ?? "");
        if (!looksLikePlayer && Conf.C.ModIcons)
        {
            var mod = ModIconSnippet.GetModSource();
            if (!string.IsNullOrEmpty(mod) && mod != "Terraria" && mod != "ModLoader") 
                text = ModIconTagHandler.GenerateTag(mod) + " " + text;
        }

        if (!string.IsNullOrEmpty(text) && Regex.IsMatch(text, @"\[u:[^\]]+\]", RegexOptions.IgnoreCase)) text += string.Concat(Enumerable.Repeat("\n", 6));
        self._showCount = (int)Conf.C.ChatItemCount;
        orig(self, text, color, widthLimitInPixels);

        if (self._messages.Count == 0) return;
        var container = self._messages[0];
        if (container._parsedText.Count == 0) return;

        bool allowHeads = Conf.C.PlayerIcons;
        bool allowColors = Conf.C.PlayerColors;
        bool allowModIcons = Conf.C.ModIcons;

        var nameWrapped = false;
        for (int li = 0; li < container._parsedText.Count; li++)
        {
            var line = container._parsedText[li];
            if (line == null || line.Length == 0) continue;

            int nameIndex = -1;
            string playerName = null;

            for (int si = 0; si < line.Length; si++)
            {
                var s = line[si];
                if (s == null) continue;
                var t = s.Text;
                if (string.IsNullOrWhiteSpace(t)) continue;
                var trimmed = t.Trim();

                if (!nameWrapped && PlayerFormatSnippet.TryExtractName(trimmed, out var extracted))
                {
                    playerName = extracted;
                    line[si] = new PlayerFormatSnippet(s);
                    nameWrapped = true;
                    nameIndex = si;
                    continue;
                }

                if (LinkSnippet.IsLink(trimmed)) line[si] = new LinkSnippet(s);
            }

            if (nameIndex < 0 || string.IsNullOrEmpty(playerName)) continue;

            int whoAmI = -1;
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                var p = Main.player[i];
                if (p != null && p.active && p.name != null && string.Equals(p.name, playerName, StringComparison.OrdinalIgnoreCase)) { whoAmI = i; break; }
            }

            int skip = 0;
            if (line.Length > 0)
            {
                var first = line[0];
                bool hasModIconPrefix = first is ModIconSnippet || (first?.Text?.StartsWith("[m:", StringComparison.OrdinalIgnoreCase) == true);

                // Strip mod icon when: player message (never show both) OR mod icons disabled globally
                if (hasModIconPrefix && (!allowModIcons || playerName != null)) skip = 1;
                if (skip == 1 && line.Length > 1 && string.IsNullOrWhiteSpace(line[1]?.Text)) skip = 2;
            }

            int prefixSlots = allowHeads && whoAmI >= 0 ? 2 : 0;
            var augmented = new TextSnippet[line.Length - skip + prefixSlots];

            if (prefixSlots == 2)
            {
                augmented[0] = new PlayerHeadSnippet(whoAmI);
                augmented[1] = new TextSnippet(" ");
            }

            Array.Copy(line, skip, augmented, prefixSlots, line.Length - skip);

            int namePos = prefixSlots + (nameIndex - skip);
            if (allowColors && whoAmI >= 0 && namePos >= 0 && namePos < augmented.Length) augmented[namePos].Color = PlayerColorSnippet.GetColor(whoAmI);

            container._parsedText[li] = augmented;
        }
    }
}
