using System.Reflection;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.UI.Chat;
using Terraria.ModLoader;

namespace ChatPlus.Common.ModCommands
{
    public class ChatClearCommand : ModCommand
    {
        public override string Command => "clear";
        public override string Description => "Clears all chat messages in history.";
        public override CommandType Type => CommandType.Chat;

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            var monitor = Main.chatMonitor;

            // If monitor doesn't exist, create a fresh one and bail.
            if (monitor == null)
            {
                try
                {
                    Main.chatMonitor = new RemadeChatMonitor();
                }
                catch
                {
                    // Fall back to doing nothing if the type isn't available
                }

                Main.NewText("Chat cleared!", Color.Green);
                return;
            }

            // 1) Try a Clear() method if the implementation provides one
            var clearMethod = monitor.GetType().GetMethod("Clear", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (clearMethod != null)
            {
                clearMethod.Invoke(monitor, null);
                Main.NewText("Chat cleared!", Color.Green);
                return;
            }

            // 2) Otherwise, reflect the internal list and reset counters if present
            try
            {
                var messagesField = monitor.GetType().GetField("_messages", BindingFlags.Instance | BindingFlags.NonPublic);
                if (messagesField?.GetValue(monitor) is System.Collections.IList list)
                    list.Clear();

                var showCountField = monitor.GetType().GetField("_showCount", BindingFlags.Instance | BindingFlags.NonPublic);
                var scrollOffsetField = monitor.GetType().GetField("_scrollOffset", BindingFlags.Instance | BindingFlags.NonPublic);

                if (showCountField != null) showCountField.SetValue(monitor, 0);
                if (scrollOffsetField != null) scrollOffsetField.SetValue(monitor, 0f);

                Main.NewText("Chat cleared!", Color.Green);
            }
            catch
            {
                // 3) As a last resort, reinitialize the monitor
                try
                {
                    Main.chatMonitor = new RemadeChatMonitor();
                    Main.NewText("Chat cleared!", Color.Green);
                }
                catch
                {
                    Main.NewText("Couldn't clear chat (unsupported chat monitor).", Color.Red);
                }
            }
        }
    }
}
