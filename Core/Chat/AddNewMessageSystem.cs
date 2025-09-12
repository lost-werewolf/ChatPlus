using System;
using System.Linq;
using System.Text.RegularExpressions;
using ChatPlus.Common.Configs;
using ChatPlus.Core.Features.Links;
using ChatPlus.Core.Features.Mentions;
using ChatPlus.Core.Features.ModIcons;
using ChatPlus.Core.Features.PlayerColors;
using ChatPlus.Core.Features.PlayerIcons;
using ChatPlus.Core.Features.Uploads;
using ChatPlus.Core.Helpers;
using Terraria;
using Terraria.GameContent.UI.Chat;
using Terraria.ModLoader;
using static Terraria.Localization.NetworkText;

namespace ChatPlus.Core.Chat;

internal class AddNewMessageSystem : ModSystem
{
    public override void Load()
    {
        On_RemadeChatMonitor.AddNewMessage += ModifyNewMessage;
    }
    public override void Unload()
    {
        On_RemadeChatMonitor.AddNewMessage -= ModifyNewMessage;
    }

    /// <summary>
    /// Modifies an incoming chat message with the following changes:
    /// 1. Set show count from config
    /// 2. Adds upload padding
    /// 3. Adds a player icon before player sent messages
    /// 4. Adds a mod icon tag before mod sent messages
    /// 5. Adds a playerNameText tag for links
    /// 6. Adds player color
    /// </summary>
    private void ModifyNewMessage(On_RemadeChatMonitor.orig_AddNewMessage orig, RemadeChatMonitor self, string text, Color color, int widthLimitInPixels)
    {
        // Modify original text!
        string resultText = text;

        /// 1. Set show count from config
        self._showCount = (int)Conf.C.ChatsVisible;

        /// 2. Adds upload padding
        if (UploadTagHandler.ContainsUploadTag(text))
            resultText += string.Concat(Enumerable.Repeat("\n", 6));

        // Extract sender name from [n:Name]
        string senderName = null;
        string nameTagFull = null;

        if (PlayerColorHandler.TryGetNameTag(text, out string nameTag))
        {
            nameTagFull = nameTag;
            var m = Regex.Match(nameTag, @"\[n:(?<name>[^\]]+)\]", RegexOptions.IgnoreCase);
            if (m.Success) senderName = m.Groups["name"].Value;
#if DEBUG
            //Log.Info("nameTagFull:" + nameTagFull + ", senderName: " + senderName);
#endif
        }

        // 3. Add player icon
        if (Conf.C.PlayerIcons && !string.IsNullOrEmpty(senderName))
        {
            string playerTag = PlayerIconTagHandler.GenerateTag(senderName);
            resultText = playerTag + " " + resultText;
        }

        // 4. Add mod icon
        string mod = "";
        if (Conf.C.ModIcons && string.IsNullOrEmpty(senderName) && !text.StartsWith("[m:"))
        {
            //if (!string.IsNullOrEmpty(nameTagFull)) return;

            mod = ModIconSnippet.GetModSource();
#if DEBUG
            Log.Info("mod source: " + mod);
#endif
            string modTag = ModIconTagHandler.GenerateTag(mod);
            resultText = modTag + " " + resultText;
        }

        // 5. Link tag
        if (LinkTagHandler.TryGetLink(text, out string linkText))
        {
            string linkTag = LinkTagHandler.GenerateTag(linkText);
            resultText = resultText.Replace(linkText, linkTag);
        }

        // Mentions: convert @LocalPlayerName only
        resultText = TransformMentions(resultText);

        // 6. Name color formatting
        if (!string.IsNullOrEmpty(senderName) && !string.IsNullOrEmpty(nameTagFull))
        {
            string hex = "FFFFFF";

            int who = Array.FindIndex(Main.player, p => p?.active == true && p.name == senderName);
            if (who >= 0 && AssignPlayerColorsSystem.PlayerColors.TryGetValue(who, out var syncedHex)
                && !string.IsNullOrWhiteSpace(syncedHex))
            {
                hex = syncedHex.ToUpperInvariant();
            }
            //Log.Info($"found {hex} for {who}");

            resultText = resultText.Replace(nameTagFull, $"[c/{hex}:{senderName}:]");
        }
#if DEBUG
        Log.Info(resultText);
#endif

        orig(self, resultText, color, widthLimitInPixels);
    }

    /// <summary>
    /// Transforms every @x to [mention:x]
    /// </summary>
    private static string TransformMentions(string input)
    {
        if (string.IsNullOrEmpty(input) || input.IndexOf('@') == -1)
            return input;

        var sb = new System.Text.StringBuilder(input.Length + 16);
        bool insideTag = false;

        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];

            // Track [ ... ] tag sections so we don't transform inside tags
            if (c == '[') insideTag = true;
            if (c == ']') insideTag = false;

            if (!insideTag && c == '@')
            {
                int start = i + 1;
                int j = start;

                // simple “mention word” scan: stop at whitespace or tag delimiters
                while (j < input.Length && !char.IsWhiteSpace(input[j]) && input[j] != ':' && input[j] != '[' && input[j] != ']')
                    j++;

                if (j > start)
                {
                    string name = input.Substring(start, j - start);
                    sb.Append(MentionTagHandler.GenerateTag(name)); // [mention:Name]
                    i = j - 1;
                    continue;
                }
            }

            sb.Append(c);
        }
#if DEBUG
        //Log.Info($"Mention: {input} -> {sb.ToString()}");
#endif
        return sb.ToString();
    }
}
