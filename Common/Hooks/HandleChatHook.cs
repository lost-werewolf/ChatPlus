using System;
using AdvancedChatFeatures.Commands;
using AdvancedChatFeatures.Emojis;
using AdvancedChatFeatures.Helpers;
using AdvancedChatFeatures.UI.Glyphs;
using MonoMod.Cil;
using Terraria;
using Terraria.GameContent.UI.Chat;
using Terraria.ModLoader;

namespace AdvancedChatFeatures.Common.Hooks
{
    /// <summary>
    /// This system does IL edits the chat to implement and modify some features related to the chat:
    /// 1. Skips chat lines from scrolling when pressing up and down arrow keys.
    /// </summary>
    public class HandleChatHook : ModSystem
    {
        public override void Load()
        {
            IL_Main.DoUpdate_HandleChat += ModifyToggleChat;
        }

        public override void Unload()
        {
            IL_Main.DoUpdate_HandleChat -= ModifyToggleChat;
        }

        /// <summary>
        /// Skips the text lines offset changing when scrolling up and down arrow keys
        /// if the command window is open
        /// </summary>
        private void ModifyToggleChat(ILContext il)
        {
            try
            {
                var c = new ILCursor(il);

                if (!c.TryGotoNext(
                        i => i.MatchLdsfld(typeof(Main), nameof(Main.chatMonitor)),
                        i => i.MatchLdloc0(),
                        i => i.MatchCallvirt(typeof(IChatMonitor), nameof(IChatMonitor.Offset))))
                {
                    Log.Error("ToggleChatHook: pattern not found (chatMonitor.Offset).");
                    MonoModHooks.DumpIL(Mod, il);
                    return;
                }

                c.Index += 2;

                // If any system is active, we return 0 (to prevent scrolling)
                c.EmitDelegate<Func<int, int>>(v =>
                {
                    var cmd = ModContent.GetInstance<CommandSystem>();
                    var emo = ModContent.GetInstance<EmojiSystem>();
                    var gly = ModContent.GetInstance<GlyphSystem>();

                    if (cmd?.ui?.CurrentState != null ||
                        emo?.ui?.CurrentState != null ||
                        gly?.ui?.CurrentState != null)
                        return 0;

                    return v;
                });

                MonoModHooks.DumpIL(Mod, il);
                Log.Info("ToggleChatHook: injected successfully.");
            }
            catch (Exception ex)
            {
                Log.Error($"ToggleChatHook failed: {ex}");
                MonoModHooks.DumpIL(Mod, il);
            }
        }

    }
}

