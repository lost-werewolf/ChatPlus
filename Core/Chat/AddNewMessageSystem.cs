using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.UI.Chat;
using Terraria.ModLoader;
using ChatPlus.Common.Configs;
using ChatPlus.Core.Features.Links;
using ChatPlus.Core.Features.Mentions;
using ChatPlus.Core.Features.ModIcons;
using ChatPlus.Core.Features.PlayerColors;
using ChatPlus.Core.Features.PlayerIcons;
using ChatPlus.Core.Features.Uploads;

namespace ChatPlus.Core.Chat
{
    internal class AddNewMessageSystem : ModSystem
    {
        // Strip both [p:Name] and [playericon:Name] tags when this client has PlayerIcons disabled
        private static readonly Regex _playerIconTagRx =
            new(@"\[(?:p|playericon):[^\]]+\]", RegexOptions.IgnoreCase | RegexOptions.Compiled);

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
        /// 3. Adds a player icon before player-sent messages (client-only toggle)
        /// 4. Adds a mod icon tag before mod-sent messages
        /// 5. Adds a link tag for URLs
        /// 6. Transforms mentions
        /// 7. Applies player name color
        /// </summary>
        private void ModifyNewMessage(On_RemadeChatMonitor.orig_AddNewMessage orig,
                                      RemadeChatMonitor self, string text, Color color, int widthLimitInPixels)
        {
            // Start from original text
            string resultText = text;

            // 1) show count from config
            self._showCount = (int)Conf.C.ChatsVisible;

            // 2) Upload padding (client-side visual)
            if (UploadTagHandler.ContainsUploadTag(resultText))
                resultText += string.Concat(Enumerable.Repeat("\n", 6));

            // Client-only visibility: if this client has PlayerIcons disabled,
            // strip any existing player-icon tags already in the message.
            if (!Conf.C.PlayerIcons)
                resultText = _playerIconTagRx.Replace(resultText, string.Empty);

            // Extract sender name from [n:Name]
            string senderName = null;
            string nameTagFull = null;
            if (PlayerColorHandler.TryGetNameTag(resultText, out string nameTag))
            {
                nameTagFull = nameTag;
                var m = Regex.Match(nameTag, @"\[n:(?<name>[^\]]+)\]", RegexOptions.IgnoreCase);
                if (m.Success) senderName = m.Groups["name"].Value;
            }

            // 3) Add player icon (only when this client wants them)
            if (Conf.C.PlayerIcons && !string.IsNullOrEmpty(senderName))
            {
                string playerTag = PlayerIconTagHandler.GenerateTag(senderName);
                resultText = playerTag + " " + resultText;
            }

            // 4) Add mod icon for system/mod messages (no sender)
            if (Conf.C.ModIcons && string.IsNullOrEmpty(senderName) && !resultText.StartsWith("[m:"))
            {
                string modTag = ModIconTagHandler.GenerateTag(ModIconSnippet.GetModSource());
                resultText = " " + modTag + " " + resultText;
            }

            // 5) Link tag
            if (LinkTagHandler.TryGetLink(resultText, out string linkText))
            {
                string linkTag = LinkTagHandler.GenerateTag(linkText);
                resultText = resultText.Replace(linkText, linkTag);
            }

            // Add timestamp if enabled
            if (Conf.C.timestampSettings != Config.TimestampSettings.Off)
            {
                string format = GetTimeFormat(Conf.C.timestampSettings);
                string timestamp = DateTime.Now.ToString(format);

                // Always wrap in [] and color dim
                resultText = $"[c/FFFFFF:[{timestamp}]] {resultText}";
            }

            // 6) Mentions: transform @name -> [mention:name]
            resultText = TransformMentions(resultText);

            // 7) Name color formatting
            if (!string.IsNullOrEmpty(senderName) && !string.IsNullOrEmpty(nameTagFull))
            {
                string hex = "FFFFFF";
                int who = Array.FindIndex(Main.player, p => p?.active == true && p.name == senderName);
                if (who >= 0 &&
                    AssignPlayerColorsSystem.PlayerColors.TryGetValue(who, out var syncedHex) &&
                    !string.IsNullOrWhiteSpace(syncedHex))
                {
                    hex = syncedHex.ToUpperInvariant();
                }

                resultText = resultText.Replace(nameTagFull, $"[c/{hex}:{senderName}:]");
            }

            // Hand off to vanilla
            orig(self, resultText, color, widthLimitInPixels);
        }

        private static string GetTimeFormat(Config.TimestampSettings setting)
        {
            if (setting == Config.TimestampSettings.HourAndMinute12Hours)
            {
                return "h:mm tt";
            }
            if (setting == Config.TimestampSettings.HourAndMinuteAndSeconds12Hours)
            {
                return "h:mm:ss tt";
            }
            if (setting == Config.TimestampSettings.HourAndMinute24Hours)
            {
                return "HH:mm";
            }
            if (setting == Config.TimestampSettings.HourAndMinuteAndSeconds24Hours)
            {
                return "HH:mm:ss";
            }
            return "HH:mm";
        }

        /// <summary>
        /// Transforms every @x to [mention:x], skipping inside [ ... ] tags.
        /// </summary>
        private static string TransformMentions(string input)
        {
            if (string.IsNullOrEmpty(input) || input.IndexOf('@') == -1)
                return input;

            var sb = new StringBuilder(input.Length + 16);
            bool insideTag = false;

            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];

                if (c == '[') insideTag = true;
                if (c == ']') insideTag = false;

                if (!insideTag && c == '@')
                {
                    int start = i + 1;
                    int j = start;

                    // Stop at whitespace or tag delimiters
                    while (j < input.Length &&
                           !char.IsWhiteSpace(input[j]) &&
                           input[j] != ':' && input[j] != '[' && input[j] != ']')
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

            return sb.ToString();
        }
    }
}
