using System.Reflection;
using ChatPlus.Core.Features.Scrollbar;
using Terraria;
using Terraria.GameContent.UI.Chat;
using Terraria.ModLoader;

namespace ChatPlus.Common.ModCommands
{
    public class ChatClearCommand : ModCommand
    {
        public override string Command => "clear";
        public override string Description => "Clears all chat messages.";
        public override CommandType Type => CommandType.Chat;

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            ClearChatMonitor();

            // Clear scroll list
            var sys = ModContent.GetInstance<ChatScrollSystem>();
            sys.state.chatScrollList.Clear();
        }

        private void ClearChatMonitor()
        {
            // Clear _messages from RemadeChatMonitor
            var clearMethod = Main.chatMonitor.GetType().GetMethod("Clear", BindingFlags.Instance | BindingFlags.Public);
            clearMethod.Invoke(Main.chatMonitor, null);
            Main.NewTextMultiline("Chat cleared!", c: Color.Green);
        }
    }
}
