

// // ChatRebind, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// // ChatRebind.ChatRebind
// using System;
// using System.Linq;
// using AdvancedChatFeatures.Common.Keybinds;
// using AdvancedChatFeatures.Helpers;
// using Microsoft.Xna.Framework.Input;
// using Mono.Cecil.Cil;
// using MonoMod.Cil;
// using Terraria;
// using Terraria.ModLoader;

// namespace AdvancedChatFeatures.Common.Hooks;

// public class ChatRebindHook : ModSystem
// {
//     public override void Load()
//     {
//         IL_Main.DoUpdate_Enter_ToggleChat += IL_ModifyToggleChatKey;
//     }

//     private void IL_ModifyToggleChatKey(ILContext il)
//     {
//         //IL_0011: Unknown result type (might be due to invalid IL or missing references)
//         //IL_0017: Expected O, but got Unknown
//         ILCursor c = new ILCursor(il);
//         if (!c.TryGotoNext(new Func<Instruction, bool>[1]
//         {
//         (Instruction i) => ILPatternMatchingExt.MatchLdcI4(i, 13)
//         }))
//         {
//             Log.Error("Failed to find Enter key (13) in DoUpdate_Enter_ToggleChat");
//             return;
//         }
//         c.Remove();
//         c.EmitDelegate<Func<int>>((Func<int>)delegate
//         {
//             if (KeybindSystem.ToggleChatKeybind == null || !KeybindSystem.ToggleChatKeybind.GetAssignedKeys().Any())
//             {
//                 return 13;
//             }
//             if (Enum.TryParse<Keys>(KeybindSystem.ToggleChatKeybind.GetAssignedKeys()[0], out var result))
//             {
//                 Log.Info($"Using keybind key: {result} (code: {result})");
//                 return (int)result;
//             }
//             Log.Info("Couldn't parse key, falling back to Enter (13)");
//             return 13;
//         });
//         Log.Info("Successfully patched DoUpdate_Enter_ToggleChat");
//     }
// }
