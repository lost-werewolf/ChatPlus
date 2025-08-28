using System;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using ChatPlus.Common.Configs;
using ChatPlus.Core.Features.Links;
using ChatPlus.Core.Features.ModIcons;
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
        Log.Info(looksLikePlayer.ToString());
        var mod = looksLikePlayer ? null : GetModSource();

        // prepend icon only if this is a mod message
        if (!looksLikePlayer && !string.IsNullOrEmpty(mod) && mod != "Terraria" && mod != "ModLoader")
            text = ModIconTagHandler.GenerateTag(mod) + " " + text;

        if (!string.IsNullOrEmpty(text) && Regex.IsMatch(text, @"\[u:[^\]]+\]", RegexOptions.IgnoreCase))
            text += string.Concat(Enumerable.Repeat("\n", 9));

        self._showCount = (int)Conf.C.ChatItemCount;
        orig(self, text, color, widthLimitInPixels);
        Intercept(self);
    }

    private void Intercept(RemadeChatMonitor self)
    {
        if (self._messages.Count == 0) return;
        var container = self._messages[0];
        if (container._parsedText.Count == 0) return;

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
                if (!nameWrapped && PlayerFormatSnippet.LooksLikeName(trimmed))
                {
                    playerName = trimmed.Substring(1, trimmed.Length - 2);
                    line[si] = new PlayerFormatSnippet(s);
                    nameWrapped = true;
                    nameIndex = si;
                    continue;
                }
                if (LinkSnippet.IsLink(trimmed)) line[si] = new LinkSnippet(s);
            }

            if (nameIndex < 0 || string.IsNullOrEmpty(playerName)) continue;

            var whoAmI = -1;
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                var p = Main.player[i];
                if (p != null && p.active && p.name != null && string.Equals(p.name, playerName, StringComparison.OrdinalIgnoreCase)) { whoAmI = i; break; }
            }
            if (whoAmI < 0) continue;

            var skip = 0;
            if (line.Length > 0)
            {
                var first = line[0];
                if (first is ModIconSnippet || (first?.Text?.StartsWith("[m:", StringComparison.OrdinalIgnoreCase) == true)) skip = 1;
                if (skip == 1 && line.Length > 1 && string.IsNullOrWhiteSpace(line[1]?.Text)) skip = 2;
            }

            var augmented = new TextSnippet[line.Length - skip + 1];
            augmented[0] = new PlayerHeadSnippet(whoAmI);
            Array.Copy(line, skip, augmented, 1, line.Length - skip);
            container._parsedText[li] = augmented;
        }
    }

    static string? GetModSource()
    {
        try
        {
            var trace = new StackTrace();
            var frames = trace.GetFrames();
            if (frames == null) return null;
            var terrariaAssembly = typeof(Main).Assembly;
            var loaderAssembly = typeof(ModLoader).Assembly;
            var pivot = -1;
            for (int k = 0; k < frames.Length; k++)
            {
                var method = frames[k].GetMethod();
                if (method == null) continue;
                var name = method.Name;
                if (name.IndexOf("NewText", StringComparison.Ordinal) >= 0 || name.IndexOf("AddNewMessage", StringComparison.Ordinal) >= 0) { pivot = k; break; }
            }
            if (pivot < 0) return null;
            Type chosenType = null;
            for (int k = pivot + 1; k < frames.Length; k++)
            {
                var method = frames[k].GetMethod();
                if (method == null) continue;
                var type = method.DeclaringType;
                if (type == null || type.Namespace == null) continue;
                var asm = type.Assembly;
                if (asm == terrariaAssembly || asm == loaderAssembly) continue;
                chosenType = type;
                break;
            }
            if (chosenType == null) return "Terraria";
            var mod = ModLoader.Mods.FirstOrDefault(z => z.Name != "ModLoader" && z.Code == chosenType.Assembly);
            if (mod == null) return "Terraria";
            return mod.Name;
        }
        catch
        {
            return null;
        }
    }
}
