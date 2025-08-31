using System;
using System.Linq;
using System.Text.RegularExpressions;
using ChatPlus.Common.Configs;
using ChatPlus.Core.Features.Links;
using ChatPlus.Core.Features.ModIcons;
using ChatPlus.Core.Features.PlayerHeads;
using ChatPlus.Core.Features.Uploads;
using ChatPlus.Core.Helpers;
using Terraria;
using Terraria.GameContent.UI.Chat;
using Terraria.ModLoader;
using Terraria.UI.Chat;

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
        // 1. Set show count from config
        self._showCount = (int)Conf.C.ChatItemCount;

        // 2. Add upload padding
        if (UploadTagHandler.ContainsUploadTag(text))
            text += string.Concat(Enumerable.Repeat("\n", 6));

        string resultText = text;

        // 3. Add player icons
        if (ContainsNameTag(text) && Main.LocalPlayer != null)
        {
            string playerTag = PlayerHeadTagHandler.GenerateTag(Main.LocalPlayer.name);
            resultText = playerTag + text;
        }

        // 4. Add mod icons
        string mod = ModIconSnippet.GetModSource();
        if (Conf.C.ModIcons && mod != null && !ContainsNameTag(text))
        {
            string modTag = ModIconTagHandler.GenerateTag(mod);
            resultText = modTag + " " + text;
        }

        // 5. Create linkText
        if (LinkTagHandler.TryGetLink(text, out string linkText))
        {
            string linkTag = LinkTagHandler.GenerateTag(linkText);
            resultText = resultText.Replace(linkText, linkTag);
        }

        // 6. Add player color
        if (TryGetNameTag(text, out string playerNameText) && Main.LocalPlayer != null)
        {
            string suffix = "";
            string prefix = ":";

            // Replace [n:Penguin] with [c/323232:Penguin]
            string playerNameTextWithColor = $"[c/{Conf.C.PlayerColor}:{suffix}{Main.LocalPlayer.name}{prefix}]";
            resultText = resultText.Replace(playerNameText, playerNameTextWithColor);
        }
        
        // Send the message
        orig(self, resultText, color, widthLimitInPixels);
    }

    public static bool TryGetNameTag(string input, out string output)
    {
        var match = Regex.Match(input, @"\[n:[^\]]+\]", RegexOptions.IgnoreCase);
        if (match.Success)
        {
            output = match.Value;
            return true;
        }
        output = null;
        return false;
    }

    public static bool ContainsNameTag(string text)
    {
        return Regex.IsMatch(text, @"\[n:[^\]]+\]", RegexOptions.IgnoreCase);
    }
}
