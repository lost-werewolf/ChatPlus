using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.UI;

namespace AdvancedChatFeatures.Helpers
{
    public static class StateHelper
    {
        private static readonly HashSet<UserInterface> all = new();
        private static UserInterface active;

        public static bool AnyActive()
        {
            return active != null && active.CurrentState != null;
        }

        public static void Register(UserInterface ui)
        {
            if (ui != null) all.Add(ui);
        }

        public static void Unregister(UserInterface ui)
        {
            if (ui == null) return;
            all.Remove(ui);
            if (active == ui) active = null;
        }

        public static bool IsActive(UserInterface ui) => active == ui;

        public static void Close(UserInterface ui)
        {
            if (ui?.CurrentState != null) ui.SetState(null);
            if (active == ui) active = null;
        }

        public static void CloseAll()
        {
            foreach (var ui in all)
                ui?.SetState(null);
            active = null;
        }

        public static void OpenExclusive(UserInterface ui, UIState state)
        {
            if (ui == null || state == null) return;

            // Close everyone else first
            foreach (var other in all)
                if (other != null && other != ui && other.CurrentState != null)
                    other.SetState(null);

            // Now open this one
            active = ui;
            if (ui.CurrentState != state)
                ui.SetState(state);
        }

        public static void ToggleForPrefixExclusive(UserInterface ui, UIState target, GameTime gameTime, string prefix)
        {
            if (ui == null || target == null) return;

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
                Close(ui);
            }
        }
    }
}
