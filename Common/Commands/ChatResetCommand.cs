using AdvancedChatFeatures.Common.Configs;
using AdvancedChatFeatures.Common.Hooks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace AdvancedChatFeatures.Common.Commands
{
    public class ChatResetCommand : ModCommand
    {
        public override string Command => "chatreset";

        public override string Description => "Reset chat position to default.";

        public override CommandType Type => CommandType.Chat;

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            // Set chatposhook.OffsetX and OffsetY to 0
            ChatPosHook.OffsetX = 0;
            ChatPosHook.OffsetY = 0;

            // Update config
            Conf.C.ChatOffsetX = 0;
            Conf.C.ChatOffsetY = 0;
            ConfigManager.Save(Conf.C);
        }
    }
}