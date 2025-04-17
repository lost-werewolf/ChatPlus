using LinksInChat.Common.Configs;
using Terraria.ModLoader;

namespace LinksInChat.Common.Commands
{
    public class ClearChatCommand : ModCommand
    {
        public override string Command => "conf";

        public override string Description => "Open Chat Config";

        public override CommandType Type => CommandType.Chat;

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            Conf.C.Open();
        }
    }
}