using ChatPlus.Common.Configs;
using Terraria;
using Terraria.ModLoader;

namespace ChatPlus.Core.Features.TypingIndicators;
internal class TypingIndicatorPlayer : ModPlayer
{
    private bool lastTyping;

    public override void PostUpdate()
    {
        if (Player.whoAmI != Main.myPlayer) return;
        if (!Conf.C.TypingIndicators) return;

        bool isTyping = Main.hasFocus && 
            Main.instance.IsActive
            && Main.drawingPlayerChat
            && Main.chatText.Length > 0
            && !Main.blockInput;

        if (isTyping != lastTyping)
        {
            lastTyping = isTyping;
            TypingIndicatorSystem.TypingPlayers[Main.myPlayer] = isTyping;
            TypingIndicatorNetHandler.SendTypingState(isTyping);
        }
    }

}
