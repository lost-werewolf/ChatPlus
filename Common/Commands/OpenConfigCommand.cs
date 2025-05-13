using AdvancedChatFeatures.Common.Configs;
using Terraria.ModLoader;

namespace AdvancedChatFeatures.Common.Commands
{
    public class OpenConfigCommand : ModCommand
    {
        public override string Command => "c";

        public override string Description => "Open Chat Config";

        public override CommandType Type => CommandType.Chat;

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            Conf.C.Open();
        }
    }
}