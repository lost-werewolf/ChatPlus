using System.IO;
using ChatPlus.Core.Netcode;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ChatPlus.Core.Features.TypingIndicators;
internal class TypingIndicatorNetHandler : BasePacketHandler
{
    public const byte HandlerId = 5;
    public static TypingIndicatorNetHandler Instance { get; } = new();

    private TypingIndicatorNetHandler() : base(HandlerId) { }

    public override void HandlePacket(BinaryReader reader, int fromWho)
    {
        int playerId = reader.ReadInt32();
        bool isTyping = reader.ReadBoolean();

        TypingIndicatorSystem.TypingPlayers[playerId] = isTyping;

        if (Main.netMode == NetmodeID.Server)
        {
            ModPacket packet = ModContent.GetInstance<ChatPlus>().GetPacket();
            packet.Write(HandlerId);
            packet.Write(playerId);
            packet.Write(isTyping);
            packet.Send(-1, fromWho);
        }
    }

    public static void SendTypingState(bool isTyping)
    {
        if (Main.netMode == NetmodeID.SinglePlayer)
            return;

        ModPacket packet = ModContent.GetInstance<ChatPlus>().GetPacket();
        packet.Write(HandlerId);
        packet.Write(Main.myPlayer);
        packet.Write(isTyping);

        if (Main.netMode == NetmodeID.MultiplayerClient)
            packet.Send();
        else if (Main.netMode == NetmodeID.Server)
            packet.Send(-1, Main.myPlayer);
    }
}
