using Terraria;
using Terraria.ModLoader;

namespace LinksInChat.Common.Commands
{
    public class ClearChatCommand : ModCommand
    {
        public override string Command => "clear";

        public override string Description => "Open Chat Config";

        public override CommandType Type => CommandType.Chat;

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            for(int i = 0; i < 10; i++)
            {
                Main.NewText("");
            }
        }
    }
}