using System.IO;
using ChatPlus.Common.Configs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ChatPlus.Core.Features.PlayerIcons.PlayerInfo.StatsPrivacy
{
    public sealed class StatsPrivacyNetHandler
    {
        public const byte HandlerId = 4;

        public static StatsPrivacyNetHandler Instance { get; } = new StatsPrivacyNetHandler();

        enum Op : byte
        {
            PrivacyUpdate = 1
        }

        // Client -> Server: announce my privacy
        public void SendLocalPrivacy()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient) return;

            var privacy = ModContent.GetInstance<Config>().StatsPrivacy;

            var packet = ModContent.GetInstance<ChatPlus>().GetPacket();
            packet.Write(HandlerId);
            packet.Write((byte)Op.PrivacyUpdate);
            packet.Write((byte)Main.myPlayer);
            packet.Write((byte)privacy);
            packet.Send();
        }

        // Server -> Specific client: send everyone’s current privacy to the newly joined client
        public void ServerSyncTo(int toClient)
        {
            if (Main.netMode != NetmodeID.Server) return;

            for (int i = 0; i < Main.maxPlayers; i++)
            {
                if (!Main.player[i].active) continue;

                var privacy = PrivacyCache.Get(i);

                var packet = ModContent.GetInstance<ChatPlus>().GetPacket();
                packet.Write(HandlerId);
                packet.Write((byte)Op.PrivacyUpdate);
                packet.Write((byte)i);
                packet.Write((byte)privacy);
                packet.Send(toClient);
            }
        }

        public void HandlePacket(BinaryReader reader, int fromWho)
        {
            var op = (Op)reader.ReadByte();

            if (op != Op.PrivacyUpdate) return;

            int playerId = reader.ReadByte();
            var privacy = (Config.UserStatsPrivacy)reader.ReadByte();

            PrivacyCache.Set(playerId, privacy);

            if (Main.netMode == NetmodeID.Server)
            {
                var packet = ModContent.GetInstance<ChatPlus>().GetPacket();
                packet.Write(HandlerId);
                packet.Write((byte)Op.PrivacyUpdate);
                packet.Write((byte)playerId);
                packet.Write((byte)privacy);
                packet.Send(-1, fromWho);
            }
        }
    }
}
