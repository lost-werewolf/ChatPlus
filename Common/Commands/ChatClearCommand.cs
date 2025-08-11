using AdvancedChatFeatures.Common.Configs;
using Terraria;
using Terraria.ModLoader;

namespace AdvancedChatFeatures.Common.Commands
{
    public class ChatClearCommand : ModCommand
    {
        public override string Command => "clear";

        public override string Description => "Clears the last few messages.";

        public override CommandType Type => CommandType.Chat;

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            for (int i = 0; i < Conf.C.ShowCount; i++)
            {
                Main.NewText("");
            }
        }
    }
}