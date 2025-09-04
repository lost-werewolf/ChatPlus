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

        public static void OpenFromButton()
        {
            _forceOpen = true;
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
                return;
            }

            // Button override keeps it open regardless of text
            if (_forceOpen)
            {
                if (ui.CurrentState != state)
                {
                    StateManager.CloseOthers(ui);
                    ui.SetState(state);
                    ui.CurrentState?.Recalculate();
                }
                ui.Update(gameTime);
                return;
            }

            string text = Main.chatText ?? string.Empty;
            int caret = Math.Clamp(HandleChatSystem.GetCaretPos(), 0, text.Length);

            // Case 1: actively typing an [e ...] tag (no closing bracket yet)
            bool hasOpenETag = false;
            {
                int eStart = text.LastIndexOf("[e", StringComparison.OrdinalIgnoreCase);
                hasOpenETag = eStart >= 0 && text.IndexOf(']', eStart + 2) == -1;
            }

            // Case 2: a ':' that is NOT inside any bracketed tag like [c:...], [e:...], [g:...], [n:...]
            bool colonTrigger = false;
            {
                int searchStart = Math.Min(caret - 1, text.Length - 1);
                int colon = searchStart >= 0 ? text.LastIndexOf(':', searchStart) : -1;
                if (colon >= 0)
                {
                    // If the nearest '[' to the left is after the nearest ']' to the left,
                    // the ':' lives inside a tag ⇒ don't trigger.
                    int lb = text.LastIndexOf('[', colon);
                    int rb = text.LastIndexOf(']', colon);
                    bool insideTag = lb > rb;
                    colonTrigger = !insideTag;
                }
            }

            bool shouldOpen = hasOpenETag || colonTrigger;

            if (shouldOpen)
            {
                if (ui.CurrentState != state)
                {
                    StateManager.CloseOthers(ui);
                    ui.SetState(state);
                    ui.CurrentState?.Recalculate();
                }
                ui.Update(gameTime);
            }
            else
            {
                if (ui.CurrentState == state)
                    ui.SetState(null);
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