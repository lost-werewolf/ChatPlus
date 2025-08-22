using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.UI;

namespace ChatPlus.Helpers;

public static class StateHandler
{
    public static void OpenStateIfPrefixMatches(GameTime gameTime, UserInterface ui, UIState state, string prefix)
    {
        if (!Main.drawingPlayerChat)
        {
            if (ui?.CurrentState != null)
            {
                ui.SetState(null);
            }
            return;
        }

        string text = Main.chatText ?? "";

        // Look for last occurrence of the prefix
        int start = text.LastIndexOf(prefix);
        if (start == -1)
        {
            // No active prefix → close
            if (ui?.CurrentState == state)
                ui.SetState(null);
            return;
        }

        // Check if this prefix is already "closed" with a ']'
        int end = text.IndexOf(']', start + prefix.Length);
        bool isClosed = end != -1;

        if (!isClosed)
        {
            // Still typing an open prefix → keep state open
            if (ui.CurrentState != state)
                ui.SetState(state);

            ui.Update(gameTime);
        }
        else
        {
            // Found a closing bracket → close state
            if (ui?.CurrentState == state)
                ui?.SetState(null);
        }
    }
}
