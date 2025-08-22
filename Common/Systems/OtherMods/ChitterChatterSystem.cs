using System;
using System.Collections.Generic;
using System.Reflection;
using AdvancedChatFeatures.Common.Snippets;
using AdvancedChatFeatures.Helpers;
using MonoMod.RuntimeDetour;
using Terraria.GameContent.UI.Chat;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace AdvancedChatFeatures.Common.Systems.OtherMods
{
    internal class ChitterChatterSystem : ModSystem
    {
        private Hook newTextHook;

        private delegate void newTextOrig(object self, string text, bool force, Color c, int widthLimit);

        public override void Load()
        {
            if (ModLoader.TryGetMod("ChitterChatter", out Mod cc))
            {
                InitializeHook(cc);
            }
        }

        public override void Unload()
        {
            newTextHook?.Dispose();
            newTextHook = null;
        }

        /// <summary>
        /// The action to perform when the hook is hooking
        /// </summary>
        private void ActionOnHook(object self, string text, bool force, Color c, int widthLimit)
        {
            // Get vanillaChatRoom
            var vanillaField = self.GetType().GetField("vanillaChatRoom", BindingFlags.NonPublic | BindingFlags.Instance);
            var vanilla = vanillaField.GetValue(self);
            
            // Get messages
            var msgsField = vanilla.GetType().GetField("messages", BindingFlags.NonPublic | BindingFlags.Instance);
            var msgs = msgsField.GetValue(vanilla) as System.Collections.IList;

            var container = msgs[0]; // newest message is inserted at index 0

            // Get parsedText
            var parsedField = container.GetType().GetField("parsedText", BindingFlags.NonPublic | BindingFlags.Instance);
            var parsedList = parsedField.GetValue(container) as List<TextSnippet[]>;

            var line = parsedList[^1];

            for (int i = 0; i < line.Length; i++)
            {
                var snip = line[i];
                if (snip == null) continue;
                var t = snip.Text?.Trim();
                if (string.IsNullOrEmpty(t)) continue;

                if (LinkSnippet.IsWholeLink(t) && snip is not LinkSnippet)
                {
                    line[i] = new LinkSnippet(snip);
                }
            }
        }

        private void InitializeHook(Mod cc)
        {
            var newTextMethod = GetMethod(cc);
            if (newTextMethod != null)
            {
                newTextHook = new Hook(
                    newTextMethod,
                    new Action<newTextOrig, object, string, bool, Color, int>((orig, self, text, force, c, widthLimit) =>
                    {
                        orig(self, text, force, c, widthLimit); // call vanilla behaviour
                        ActionOnHook(self, text, force, c, widthLimit); // call our action
                    }
                ));
            }
        }

        private MethodInfo GetMethod(Mod cc)
        {
            var type = cc.Code.GetType("Tomat.TML.Mod.ChitterChatter.Content.Features.ChatMonitor.CustomChatMonitor");
            if (type == null)
            {
                Log.Error($"CC not found, exiting...");
                return null;
            }
            string iface = typeof(IChatMonitor).FullName;// "Terraria.GameContent.UI.Chat.IChatMonitor"

            var method = type.GetMethod(
                iface + ".NewTextMultiline",
                BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                [typeof(string), typeof(bool), typeof(Color), typeof(int)],
                null
            );

            if (method == null)
            {
                Log.Error($"NewText not found, exiting...");
                return null;
            }
            return method;
        }
    }
}
