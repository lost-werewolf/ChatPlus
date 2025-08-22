using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using AdvancedChatFeatures.Helpers;
using AdvancedChatFeatures.UploadHandler;
using Microsoft.Xna.Framework;
using MonoMod.RuntimeDetour;
using MonoMod.RuntimeDetour.HookGen;
using Terraria;
using Terraria.GameContent.UI.Chat;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace AdvancedChatFeatures.Common.Systems.OtherMods;

internal class AddNewMessageHook : ModSystem
{
    public override void Load()
    {
        //if (!ModLoader.TryGetMod("ChitterChatter", out Mod mod))
        //{
            On_RemadeChatMonitor.AddNewMessage += RemadeChatMonitor_AddNewMessage;
        //}
    }
    public override void Unload()
    {
        //if (!ModLoader.TryGetMod("ChitterChatter", out Mod mod))
        //{
            On_RemadeChatMonitor.AddNewMessage -= RemadeChatMonitor_AddNewMessage;
        //}
    }

    private void RemadeChatMonitor_AddNewMessage(On_RemadeChatMonitor.orig_AddNewMessage orig, RemadeChatMonitor self, string text, Color color, int widthLimitInPixels)
    {
        orig(self, text, color, widthLimitInPixels);

        InterceptMessage(self);
    }

    private void InterceptMessage(RemadeChatMonitor self)
    {
        if (self._messages.Count == 0) return;

        var container = self._messages[0];
        var parsedList = container?._parsedText;
        if (parsedList is null) return;

        const float spacerHeight = 12f;

        for (int i = 0; i < parsedList.Count; i++)
        {
            var line = parsedList[i];
            var rebuilt = new List<TextSnippet>(line.Length + 4);

            LinkHelper.ProcessLink(line, self);

            int maxExtraLinesForThisRow = 0;

            foreach (var snip in line)
            {
                rebuilt.Add(snip);

                Log.Info(snip.Text);

                if (!string.IsNullOrEmpty(snip.Text) &&
    snip.Text.StartsWith("[u:", StringComparison.OrdinalIgnoreCase))
                {
                    maxExtraLinesForThisRow = Math.Max(maxExtraLinesForThisRow, ComputeExtraUploadLines(snip.Text));
                }
            }

            parsedList[i] = rebuilt.ToArray();

            for (int n = 0; n < maxExtraLinesForThisRow; n++)
            {
                parsedList.Insert(i + 1, [new SpacerSnippet(1f, spacerHeight)]);
                i++; // skip over the spacer we just inserted
            }
        }
    }

    private static int ComputeExtraUploadLines(string tagText)
    {
        float size = 20f;

        // Prefer explicit size=...
        int idx = tagText.IndexOf("size=", StringComparison.OrdinalIgnoreCase);
        if (idx >= 0)
        {
            int start = idx + 5;
            int end = tagText.IndexOfAny(new[] { ']', '|', ',', ' ' }, start);
            if (end < 0) end = tagText.Length;

            var num = tagText.Substring(start, end - start).Trim();
            if (float.TryParse(num, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed))
                size = parsed;
        }
        else
        {
            // Fallback: [u:key|300]
            int bar = tagText.IndexOf('|');
            if (bar >= 0)
            {
                int start = bar + 1;
                int end = tagText.IndexOfAny(new[] { ']', '|', ',', ' ' }, start);
                if (end < 0) end = tagText.Length;

                var num = tagText.Substring(start, end - start).Trim();
                if (float.TryParse(num, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed))
                    size = parsed;
            }
        }

        // Apply same cap here to be safe
        size = Math.Clamp(size, 0f, 100f);

        // 0–20 => 0, 20–40 => 1, 40–60 => 2, 60–80 => 3, 80–100 => 4
        int extra = (int)Math.Ceiling(size / 20f) - 1;
        return Math.Clamp(extra, 0, 4);
    }
}
