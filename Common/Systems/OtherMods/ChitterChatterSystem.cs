using System;
using AdvancedChatFeatures.Helpers;
using MonoMod.RuntimeDetour;
using Terraria.GameContent.UI.Chat;
using Terraria.ModLoader;

namespace AdvancedChatFeatures.Common.Systems.OtherMods
{
    internal class ChitterChatterSystem : ModSystem
    {
        private Hook newTextHook;
        private Hook newTextMultilineHook;

        private delegate void NewTextOrig(object self, string text, byte r, byte g, byte b);
        private delegate void NewTextMultilineOrig(object self, string text, byte r, byte g, byte b);

        public override void Load()
        {
            if (ModLoader.TryGetMod("ChitterChatter", out Mod cc))
            {
                DetourCC(cc);
            }
        }

        public override void Unload()
        {
            //hook?.Dispose();
            //hook = null;
        }

        private void DetourCC(Mod cc)
        {
            var type = cc.Code.GetType("Tomat.TML.Mod.ChitterChatter.Content.Features.ChatMonitor.CustomChatMonitor");
            if (type == null) return;

            var map = type.GetInterfaceMap(typeof(IChatMonitor));

            for (int i = 0; i < map.InterfaceMethods.Length; i++)
            {
                var iface = map.InterfaceMethods[i];
                var target = map.TargetMethods[i];

                if (iface.Name == nameof(IChatMonitor.NewText))
                {
                    newTextHook = new Hook(
                        target,
                        new Action<NewTextOrig, object, string, byte, byte, byte>((orig, self, text, r, g, b) =>
                        {
                            var tagged = "[cc] " + text;
                            orig(self, tagged, r, g, b);
                            Log.Info("CC.NewText: " + tagged);
                        })
                    );
                    Log.Info("Hooked CC.IChatMonitor.NewText");
                }
                else if (iface.Name == nameof(IChatMonitor.NewTextMultiline))
                {
                    newTextMultilineHook = new Hook(
                        target,
                        new Action<NewTextMultilineOrig, object, string, bool, Color, int>((orig, self, text, force, c, widthLimit) =>
                        {
                            var tagged = "[cc] " + text;
                            orig(self, tagged, force, c, widthLimit);
                            Log.Info("CC.NewTextMultiline: " + tagged);
                        })
                    );
                    Log.Info("Hooked CC.IChatMonitor.NewTextMultiline");
                }
            }
        }

    }
}
