using System;
using System.Collections.Generic;
using ChatPlus.Core.Chat;
using ChatPlus.Core.Features.PlayerIcons.PlayerInfo;
using ChatPlus.Core.Helpers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace ChatPlus.Core.Features.Emojis
{
    [Autoload(Side = ModSide.Client)]
    public class EmojiSystem : ModSystem
    {
        public UserInterface ui;
        public EmojiState state;

        private static bool _forceOpen;
        public static bool IsForceOpen => _forceOpen;
        public static bool FilterReset { get; private set; }

        public static void OpenFromButton()
        {
            _forceOpen = true;
            FilterReset = true; // show full list when opened by button
            var sys = ModContent.GetInstance<EmojiSystem>();
            if (sys.ui.CurrentState != sys.state)
            {
                StateManager.CloseOthers(sys.ui);
                sys.ui.SetState(sys.state);
                sys.ui.CurrentState?.Recalculate();
            }
        }

        public static void CloseAfterCommit()
        {
            _forceOpen = false;
            FilterReset = false; // clear reset on close
            var sys = ModContent.GetInstance<EmojiSystem>();
            if (sys.ui.CurrentState != null)
                sys.ui.SetState(null);
        }

        public override void PostSetupContent()
        {
            ui = new UserInterface();
            state = new EmojiState();
            ui.SetState(null); // start hidden
        }

        public override void UpdateUI(GameTime gameTime)
        {
            // Close if chat isn't open
            if (!Main.drawingPlayerChat)
            {
                if (ui.CurrentState != null) ui.SetState(null);
                _forceOpen = false;
                FilterReset = false; // <- clear
                return;
            }

            string text = Main.chatText ?? string.Empty;
            int caret = Math.Clamp(HandleChatSystem.GetCaretPos(), 0, text.Length);

            // [e ... (no closing ])
            int eStart = text.LastIndexOf("[e", StringComparison.OrdinalIgnoreCase);
            bool hasOpenETag = eStart >= 0 && text.IndexOf(']', eStart + 2) == -1;

            // ':' not inside [tag:...]
            int searchStart = Math.Min(caret - 1, text.Length - 1);
            int colon = searchStart >= 0 ? text.LastIndexOf(':', searchStart) : -1;
            bool colonTrigger = false;
            if (colon >= 0)
            {
                int lb = text.LastIndexOf('[', colon);
                int rb = text.LastIndexOf(']', colon);
                bool insideTag = lb > rb;
                colonTrigger = !insideTag;
            }

            bool shouldOpen = _forceOpen || hasOpenETag || colonTrigger;

            if (shouldOpen)
            {
                bool justOpened = ui.CurrentState != state;
                if (justOpened)
                {
                    StateManager.CloseOthers(ui);
                    ui.SetState(state);
                    ui.CurrentState?.Recalculate();

                    // Reset filter when opening via button or ':'
                    FilterReset = _forceOpen || colonTrigger;
                }
                ui.Update(gameTime);
            }
            else
            {
                if (ui.CurrentState == state) ui.SetState(null);
                FilterReset = false; // <- clear when not open
            }
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int index = layers.FindIndex(l => l.Name.Equals("Vanilla: Death Text"));
            if (index == -1) return;

            layers.Insert(index, new LegacyGameInterfaceLayer(
                "ChatPlus: Emojis Panel",
                () =>
                {
                    if (ui?.CurrentState != null)
                        ui.Draw(Main.spriteBatch, new GameTime());

                    return true;
                },
                InterfaceScaleType.UI
            ));
        }
    }
}