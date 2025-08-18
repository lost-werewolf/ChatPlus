using System;
using System.Collections.Generic;
using System.Reflection;
using AdvancedChatFeatures.Helpers;
using Microsoft.Xna.Framework;
using MonoMod.RuntimeDetour;
using MonoMod.RuntimeDetour.HookGen;
using Terraria;
using Terraria.GameContent.UI.Chat;
using Terraria.ModLoader;
using Terraria.UI.Chat;
using static System.Net.Mime.MediaTypeNames;

namespace AdvancedChatFeatures.Common.Hooks;

internal class AddNewMessageHook : ModSystem
{
    public override void Load() 
    {
        if (!ModLoader.TryGetMod("ChitterChatter", out Mod mod))
        {
            On_RemadeChatMonitor.AddNewMessage += RemadeChatMonitor_AddNewMessage;
        }
        else
        {
            DetourCC();
        }
    }
    public override void Unload() 
    {
        if (!ModLoader.TryGetMod("ChitterChatter", out Mod mod))
        {
            On_RemadeChatMonitor.AddNewMessage -= RemadeChatMonitor_AddNewMessage;
        }
    }

    private void DetourCC()
    {
        if (ModLoader.TryGetMod("ChitterChatter", out Mod cc))
        {
            Log.Info("cc running");

            var type = cc.Code.GetType("Tomat.TML.Mod.ChitterChatter.Content.Features.ChatMonitor.CustomChatMonitor");
            if (type == null) return;

            var iface = typeof(IChatMonitor);
            var map = type.GetInterfaceMap(iface);

            for (int i = 0; i < map.InterfaceMethods.Length; i++)
            {
                if (map.InterfaceMethods[i].Name == "NewText")
                {
                    var target = map.TargetMethods[i];

                    new Hook(target,
                        (Action<object, string, byte, byte, byte>)((self, text, r, g, b) =>
                        {
                            // Call original via reflection manually
                            target.Invoke(self, new object[] { text, r, g, b });

                            Log.Info("New message (after cc): " + text);

                            if (Main.chatMonitor is RemadeChatMonitor remade)
                                InterceptMessage(remade);
                        }));

                    Log.Info("Hooked CC.NewText!");
                    break;
                }
            }
        }
    }

    private void RemadeChatMonitor_AddNewMessage(On_RemadeChatMonitor.orig_AddNewMessage orig, RemadeChatMonitor self, string text, Color color, int widthLimitInPixels)
    {
        orig(self, text, color, widthLimitInPixels);

        InterceptMessage(self);
    }

    private void InterceptMessage(RemadeChatMonitor self)
    {
        Log.Info("new message!");

        if (self._messages.Count > 0)
        {
            var container = self._messages[0];
            var parsedList = container?._parsedText;

            if (parsedList != null)
            {
                foreach (var snippetArray in parsedList.ToArray())
                {
                    LinkHelper.ProcessLink(snippetArray, self);
                }
            }
        }
    }
}
