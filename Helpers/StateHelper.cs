using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.UI;

namespace AdvancedChatFeatures.Helpers
{
    public static class StateHelper
    {
        private static readonly HashSet<UserInterface> _all = new();
        private static UserInterface _active;

        public static void Register(UserInterface ui)
        {
            if (ui != null) _all.Add(ui);
        }

        public static void Unregister(UserInterface ui)
        {
            if (ui == null) return;
            _all.Remove(ui);
            if (_active == ui) _active = null;
        }

        public static bool IsActive(UserInterface ui) => _active == ui;

        public static void Close(UserInterface ui)
        {
            if (ui?.CurrentState != null) ui.SetState(null);
            if (_active == ui) _active = null;
        }

        public static void CloseAll()
        {
            foreach (var ui in _all)
                ui?.SetState(null);
            _active = null;
        }

        public static void OpenExclusive(UserInterface ui, UIState state)
        {
            if (ui == null || state == null) return;

            // Close everyone else first
            foreach (var other in _all)
                if (other != null && other != ui && other.CurrentState != null)
                    other.SetState(null);

            // Now open this one
            _active = ui;
            if (ui.CurrentState != state)
                ui.SetState(state);
        }

        public static void ToggleForPrefixExclusive(UserInterface ui, UIState target, GameTime gameTime, string prefix)
        {
            if (ui == null || target == null) return;

            // Don’t run this in main menu.
            if (Main.gameMenu) { Close(ui); return; }

            string text = Main.chatText ?? string.Empty;
            bool want = Main.drawingPlayerChat && text.StartsWith(prefix, System.StringComparison.OrdinalIgnoreCase);

            if (want)
            {
                OpenExclusive(ui, target);
                ui.Update(gameTime);
            }
            else
            {
                // Close this UI even if it's not the currently active one.
                Close(ui);
            }
        }
    }
}
