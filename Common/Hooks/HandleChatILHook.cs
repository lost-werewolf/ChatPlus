using System;
using AdvancedChatFeatures.Colors;
using AdvancedChatFeatures.Commands;
using AdvancedChatFeatures.Emojis;
using AdvancedChatFeatures.Glyphs;
using AdvancedChatFeatures.Helpers;
using AdvancedChatFeatures.ItemWindow;
using AdvancedChatFeatures.UI;
using AdvancedChatFeatures.Uploads;
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
    public class HandleChatILHook : ModSystem
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
        /// Does 2 things:
        /// 1. Skip scrolling with up/down arrow keys when any of my states are open
        /// 2. Skip closing the chat when escape is pressed when any of my states are open
        /// </summary>
        private void ModifyToggleChat(ILContext il)
        {
            try
            {
                var cursor = new ILCursor(il);
                if (cursor.TryGotoNext(
                        i => i.MatchLdsfld(typeof(Main), nameof(Main.chatMonitor)),
                        i => i.MatchLdloc0(),
                        i => i.MatchCallvirt(typeof(IChatMonitor), nameof(IChatMonitor.Offset))))
                {
                    cursor.Index += 2;

                    cursor.EmitDelegate<Func<int, int>>(v => IsAnyStateActive() ? 0 : v);
                }
                else
                {
                    Log.Error("Failed to patch linesOffset in HandleChat.");
                    MonoModHooks.DumpIL(Mod, il);
                }

            }
            catch (Exception ex)
            {
                Log.Error($"HandleChatILHook failed: {ex}");
                MonoModHooks.DumpIL(Mod, il);
            }
        }

        public static bool IsAnyStateActive()
        {
            var cmdSys = ModContent.GetInstance<CommandSystem>();
            var colorSys = ModContent.GetInstance<ColorSystem>();
            var emojiSys = ModContent.GetInstance<EmojiSystem>();
            var glyphSys = ModContent.GetInstance<GlyphSystem>();
            var itemSys = ModContent.GetInstance<ItemSystem>();
            var uploadSys = ModContent.GetInstance<UploadSystem>();

            if (cmdSys.ui != null || colorSys.ui != null || emojiSys.ui != null || glyphSys.ui != null || itemSys.ui != null || uploadSys.ui != null)
            {
                return true;
            }

            return false;
        }
    }
}

