using System.Collections.Generic;
using System.Reflection;
using LinksInChat.Common.Configs;
using LinksInChat.Helpers;
using LinksInChat.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.UI.Chat;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace LinksInChat.Common
{
    public class ChatSystem : ModSystem
    {
        private Mod mod;

        // Player colors
        private static readonly Dictionary<string, Color> PlayerNameColors = [];
        private static int nextColorIndex = 0;

        public override void Load()
        {
            mod = ModContent.GetInstance<LinksInChat>();
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
            var localTag = $"<{Main.LocalPlayer.name}>";
            Log.Info($"[ProcessSnippets] localTag={localTag}, totalLines={parsedList.Count}");

            foreach (var snippetArray in parsedList)
            {
                for (int i = 0; i < snippetArray.Length; i++)
                {
                    var snippet = snippetArray[i];
                    Log.Info($"[Before] idx={i}, Text='{snippet.Text}', Color={snippet.Color}");

                    // name snippet?
                    if (snippet.Text.StartsWith("<") && snippet.Text.EndsWith(">"))
                    {
                        var tag = snippet.Text;
                        var playerName = tag.Trim('<', '>');
                        Log.Info($"→ Detected name tag='{tag}', playerName='{playerName}'");

                        // pick or assign color
                        if (!PlayerNameColors.TryGetValue(tag, out var col))
                        {
                            col = ColorHelper.PlayerColors[nextColorIndex++ % ColorHelper.PlayerColors.Length];
                            PlayerNameColors[tag] = col;
                            Log.Info($"   Assigned new color {col} to '{playerName}'");
                        }
                        else
                        {
                            Log.Info($"   Reusing color {col} for '{playerName}'");
                        }
                        snippet.Color = col;

                        // also set the format.
                        // change <PlayerName> to PlayerName:
                        if (Conf.C.PlayerNameFormat == "PlayerName:")
                        {
                            snippet.Text = snippet.Text.Replace("<", "").Replace(">", ":");
                        }

                        // leave snippetArray[i] as the colored TextSnippet
                    }
                    else
                    {
                        // this is the actual message text → wrap it
                        snippetArray[i] = new MessageSnippet(snippet, mod);
                        Log.Info($"→ Wrapped message snippet: '{snippetArray[i].Text}', Color={snippetArray[i].Color}");
                    }
                }
            }
        }
    }
}