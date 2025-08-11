using System;
using AdvancedChatFeatures.Helpers;
using AdvancedChatFeatures.UI;
using AdvancedChatFeatures.UI.Commands;
using MonoMod.Cil;
using Terraria;
using Terraria.GameContent.UI.Chat;
using Terraria.GameInput;
using Terraria.ModLoader;

namespace AdvancedChatFeatures.Common.Hooks
{
    public class ToggleChatHook : ModSystem
    {
        public override void Load()
        {
            IL_Main.DoUpdate_HandleChat += ModifyToggleChat;
        }

        public override void Unload()
        {
            IL_Main.DoUpdate_HandleChat -= ModifyToggleChat;
        }

        public override void PreUpdatePlayers()
        {
            var sys = ModContent.GetInstance<CommandsSystem>();
            if (sys != null)
            {
                //if (sys.commandsListState.commandsPanel.dragging
            }

            // Prevent scroll wheel from moving the hotbar
            PlayerInput.ScrollWheelDelta = 0;

            // Also block hotbar +/- keybinds
            PlayerInput.Triggers.Current.HotbarPlus = false;
            PlayerInput.Triggers.Current.HotbarMinus = false;
        }

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
                c.EmitDelegate<Func<int, int>>(v =>
                {
                    var sys = ModContent.GetInstance<CommandsSystem>();
                    return sys?.ui?.CurrentState != null ? 0 : v;
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

