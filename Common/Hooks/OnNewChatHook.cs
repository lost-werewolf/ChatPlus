using System.Linq;
using AdvancedChatFeatures.Common.Configs;
using AdvancedChatFeatures.Helpers;
using Terraria.GameContent.UI.Chat;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace AdvancedChatFeatures.Common.Hooks
{
    /// <summary>
    /// Detours into a new chat message and customizes it
    /// </summary>
    public class OnNewChatHook : ModSystem
    {
        public override void Load()
        {
            On_RemadeChatMonitor.AddNewMessage += OnNewChat;
        }

        public override void Unload()
        {
            On_RemadeChatMonitor.AddNewMessage -= OnNewChat;
        }

        private void OnNewChat(On_RemadeChatMonitor.orig_AddNewMessage orig, RemadeChatMonitor self, string text, Color color, int widthLimitInPixels)
        {
            orig(self, text, color, widthLimitInPixels);

            var parsedSnippets = ChatManager.ParseMessage(text, color).ToArray();
            if (parsedSnippets.Length < 2)
                return;

            string rawName = parsedSnippets[0].Text;
            string playerName = PlayerFormatHelper.Strip(rawName);
            


            // Player format
            if (!string.IsNullOrEmpty(Conf.C.styleConfig.ShowPlayerFormat))
                PlayerFormatHelper.Apply(ref parsedSnippets[0], playerName, Conf.C.styleConfig.ShowPlayerFormat);

            // Player colors
            if (Conf.C.styleConfig.ShowPlayerColors)
                PlayerColorHelper.Process(parsedSnippets[0], playerName);

            // Links
            if (Conf.C.styleConfig.ShowLinks)
                LinkHelper.ProcessLink(parsedSnippets, self);

            // Player icons
            if (Conf.C.styleConfig.ShowPlayerIcons)
            {
                // Insert icon snippet at start
                var iconSnippet = new PlayerIconSnippet(playerName);

                var list = parsedSnippets.ToList();
                list.Insert(0, iconSnippet);
                parsedSnippets = list.ToArray();

                var container = self._messages[0];
                container._parsedText[0] = parsedSnippets;
            }

            // Mod icons
            if (Conf.C.styleConfig.ShowModIcons)
            {
                //string modName = ModIconSnippet.GetModName();
                //Log.Info(parsedSnippets[1].Text.Trim() + ", mod: " + modName, printCallerInMessage: false);

                // Insert mod icon snippet at start
                var modIconSnippet = new ModIconSnippet("modName");
                var list = parsedSnippets.ToList();
                list.Insert(0, modIconSnippet);
                parsedSnippets = list.ToArray();
                var container = self._messages[0];
                container._parsedText[0] = parsedSnippets;
            }
        }
    }
}