using System.Collections.Generic;
using System.Reflection;
using AdvancedChatFeatures.Common.Configs;
using AdvancedChatFeatures.Common.Snippets;
using AdvancedChatFeatures.Helpers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.UI.Chat;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace AdvancedChatFeatures.Common.Hooks
{
    public class ChatNewMessageHook : ModSystem
    {
        // Player colors
        private static readonly Dictionary<string, Color> PlayerNameColors = [];
        private static int nextColorIndex = 0;

        public override void Load()
        {
            On_RemadeChatMonitor.AddNewMessage += AddNewMessageDetour;
        }

        public override void Unload()
        {
            On_RemadeChatMonitor.AddNewMessage -= AddNewMessageDetour;
            PlayerNameColors.Clear();
            nextColorIndex = 0;
        }

        private void AddNewMessageDetour(On_RemadeChatMonitor.orig_AddNewMessage orig, RemadeChatMonitor self, string text, Color color, int widthLimitInPixels)
        {
            // Call original to preserve existing functionality
            orig(self, text, color, widthLimitInPixels);

            // Grab the new message via reflection
            var chatMonitor = typeof(RemadeChatMonitor);
            var messagesField = chatMonitor.GetField("_messages", BindingFlags.NonPublic | BindingFlags.Instance);
            var messages = (List<ChatMessageContainer>)messagesField.GetValue(self);

            if (messages?.Count > 0)
            {
                // Get all messages and parse the first one to get the snippets
                var msg = messages[0];
                var parsedField = typeof(ChatMessageContainer).GetField("_parsedText", BindingFlags.NonPublic | BindingFlags.Instance);
                var parsedList = (List<TextSnippet[]>)parsedField.GetValue(msg);

                // Set time left
                var timeLeftField = typeof(ChatMessageContainer).GetField("_timeLeft", BindingFlags.NonPublic | BindingFlags.Instance);
                timeLeftField?.SetValue(msg, Conf.C.ChatMessageShowTime * 60);

                // Wrap snippets in CustomSnippet to add custom behavior
                ProcessSnippets(parsedList);
            }
        }


        private void ProcessSnippets(List<TextSnippet[]> parsedList)
        {
            string myName = Main.LocalPlayer.name;

            foreach (var snippetArray in parsedList)
            {
                /* Defensive: need at least “name” + “message” */
                if (snippetArray.Length < 2)
                    continue;

                TextSnippet nameSnippet = snippetArray[0];
                TextSnippet msgSnippet = snippetArray[1];
                Log.Info($"ChatHook received a message! Name: {nameSnippet.Text}, Message: {msgSnippet.Text}");

                // -------- 1. Is the first snippet a <Name>? --------------------
                if (!(nameSnippet.Text.StartsWith("<") && nameSnippet.Text.EndsWith(">")))
                    continue;

                string playerName = nameSnippet.Text.Trim('<', '>', ' ');

                // -------- 2. Colour every name consistently --------------------
                if (!PlayerNameColors.TryGetValue(playerName, out var nameColor))
                {
                    nameColor = ColorHelper.PlayerColors[nextColorIndex++ %
                                                         ColorHelper.PlayerColors.Length];
                    PlayerNameColors[playerName] = nameColor;
                }
                nameSnippet.Color = nameColor;

                // Optional format change “<Bob>” → “Bob:”
                if (Conf.C.PlayerNameFormat == "PlayerName:")
                    nameSnippet.Text = $"{playerName}:";

                // -------- 3. Only wrap the *message* if the name == me ----------
                if (playerName == myName && msgSnippet is not MessageSnippet)
                {
                    Mod mod = ModContent.GetInstance<AdvancedChatFeatures>();
                    var custom = new MessageSnippet(msgSnippet, mod)
                    {
                        //Scale = Conf.C.TextScale              // keep your settings
                    };
                    snippetArray[1] = custom;
                }
            }
        }

    }
}