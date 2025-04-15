using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using log4net;
using Microsoft.Xna.Framework;
using Terraria.GameContent.UI.Chat;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace LinksInChat.Common
{
    public class ChatSystem : ModSystem
    {
        private Mod mod;

        public override void OnWorldLoad()
        {
            mod = ModContent.GetInstance<LinksInChat>();
            On_RemadeChatMonitor.AddNewMessage += AddNewMessageDetour;
        }

        private void AddNewMessageDetour(On_RemadeChatMonitor.orig_AddNewMessage orig, RemadeChatMonitor self, string text, Color color, int widthLimitInPixels)
        {
            // Call original
            orig(self, text, color, widthLimitInPixels);

            // Grab the new message via reflection
            var monType = typeof(RemadeChatMonitor);
            var field = monType.GetField("_messages", BindingFlags.NonPublic | BindingFlags.Instance);
            var list = (List<ChatMessageContainer>)field.GetValue(self);

            if (list?.Count > 0)
            {
                var msg = list[0];
                var parsedField = typeof(ChatMessageContainer).GetField("_parsedText", BindingFlags.NonPublic | BindingFlags.Instance);
                var parsedList = (List<TextSnippet[]>)parsedField.GetValue(msg);

                // Wrap snippets in LoggingSnippet
                foreach (var snippetArray in parsedList)
                {
                    for (int i = 0; i < snippetArray.Length; i++)
                    {
                        snippetArray[i] = new CustomSnippet(snippetArray[i], mod);
                    }
                }
            }
        }
    }
}