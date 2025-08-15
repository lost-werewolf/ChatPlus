using Terraria;
using Terraria.GameContent.UI.Chat;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace AdvancedChatFeatures.Common.Commands
{
    public class ChatClearCommand : ModCommand
    {
        public override string Command => "clear";

        public override string Description => "Clears all chat messages in history.";

        public override CommandType Type => CommandType.Chat;

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            // Get ChatMonitor
            var chatMonitor = typeof(RemadeChatMonitor);
            var chatMonitorInstance = Main.chatMonitor as RemadeChatMonitor;

            chatMonitorInstance._messages.Clear();

            Main.NewText("Chat cleared! ", Color.Green);
        }
    }
}