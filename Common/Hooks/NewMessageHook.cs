using System;
using System.Linq;
using AdvancedChatFeatures.Common.Configs;
using AdvancedChatFeatures.Helpers;
using ReLogic.Graphics;
using Terraria;
using Terraria.Chat;
using Terraria.GameContent.UI.Chat;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;
using Terraria.UI.Chat;
using static System.Net.Mime.MediaTypeNames;

namespace AdvancedChatFeatures.Common.Hooks
{
    /// <summary>
    /// Detours into a new chat message and customizes it
    /// </summary>
    public class NewMessageHook : ModSystem
    {
        public override void Load()
        {
            On_RemadeChatMonitor.AddNewMessage += AddNewMessage;
            On_RemadeChatMonitor.NewText += NewText;
        }

        public override void Unload()
        {
            On_RemadeChatMonitor.AddNewMessage -= AddNewMessage;
            On_RemadeChatMonitor.NewText -= NewText;
        }

        private void NewText(On_RemadeChatMonitor.orig_NewText orig, RemadeChatMonitor self, string newText, byte R, byte G, byte B)
        {
            orig(self, newText, R, G, B);

            Log.Info(newText);

            if (!Conf.C.styleConfig.ShowModIcons) return;
            if (self._messages.Count == 0) return;

            var container = self._messages[0];
            if (container._parsedText == null || container._parsedText.Count == 0) return;

            var line = container._parsedText[0];
            if (line == null || line.Length == 0) return;

            var mod = ModIconSnippet.GetCallingMod();
            Log.Info("calling mod: " + mod.Name);

            var list = line.ToList();
            list.Insert(0, new ModIconSnippet(mod));
            container._parsedText[0] = list.ToArray();
        }

        private void AddNewMessage(On_RemadeChatMonitor.orig_AddNewMessage orig, RemadeChatMonitor self, string text, Color color, int widthLimitInPixels)
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

            // Re-insert mod icon (NewText insertion was overwritten by our reparse above)
            if (Conf.C.styleConfig.ShowModIcons && self._messages.Count > 0)
            {
                var container2 = self._messages[0];
                if (container2._parsedText != null && container2._parsedText.Count > 0 && container2._parsedText[0] != null)
                {
                    var mod = ModIconSnippet.GetCallingMod();
                    Log.Info("Content: + ");

                    var line2 = container2._parsedText[0].ToList();
                    line2.Insert(0, new ModIconSnippet(mod));
                    container2._parsedText[0] = line2.ToArray();
                }
            }
        }
    }
}