using System;
using System.Linq;
using System.Text.RegularExpressions;
using ChatPlus.Common.Configs;
using ChatPlus.Core.Features.Links;
using ChatPlus.Core.Features.ModIcons;
using ChatPlus.Core.Features.PlayerColors;
using ChatPlus.Core.Features.PlayerIcons;
using ChatPlus.Core.Features.Uploads;
using ChatPlus.Core.Helpers;
using Terraria;
using Terraria.GameContent.UI.Chat;
using Terraria.ModLoader;

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
        self._showCount = (int)Conf.C.ChatItemCount;

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
        }

        // 3. Add player icon
        if (!string.IsNullOrEmpty(senderName))
        {
            string playerTag = PlayerIconTagHandler.GenerateTag(senderName);
            resultText = playerTag + " " + resultText;
        }

        // 4. Add mod icon
        string mod = ModIconSnippet.GetModSource();
        if (Conf.C.ModIcons && mod != null && string.IsNullOrEmpty(senderName) && !text.StartsWith("[m:"))
        {
            string modTag = ModIconTagHandler.GenerateTag(mod);
            resultText = modTag + " " + resultText;
        }

        // 5. Link tag
        if (LinkTagHandler.TryGetLink(text, out string linkText))
        {
            string linkTag = LinkTagHandler.GenerateTag(linkText);
            resultText = resultText.Replace(linkText, linkTag);
        }

        // 6. Replace the name tag with color tag using synced table; fallback to white
        if (!string.IsNullOrEmpty(senderName) && !string.IsNullOrEmpty(nameTagFull))
        {
            string hex = "FFFFFF";
            int who = -1;

            for (int i = 0; i < Main.maxPlayers; i++)
            {
                var p = Main.player[i];
                if (p?.active == true && p.name == senderName) { who = i; break; }
            }

            if (who >= 0 && AssignPlayerColorsSystem.PlayerColors.TryGetValue(who, out var syncedHex))
            {
                hex = string.IsNullOrWhiteSpace(syncedHex) ? "FFFFFF" : syncedHex.ToUpperInvariant();
            }
            else
            {
                // Fallback
                hex = PlayerColorHandler.HexFromName(senderName);
            }

            string colored = $"[c/{hex}:{senderName}:]";
            resultText = resultText.Replace(nameTagFull, colored);
        }

        orig(self, resultText, color, widthLimitInPixels);
    }
}
