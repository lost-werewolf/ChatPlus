using System.IO;
using ChatPlus.Common.Configs;
using ChatPlus.Core.Netcode;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ChatPlus.Core.Features.PlayerIcons.PlayerInfo.StatsPrivacy
{
    internal sealed class StatsPrivacyNetHandler : BasePacketHandler
    {
        public const byte HandlerId = 4;

        public static StatsPrivacyNetHandler Instance { get; } = new StatsPrivacyNetHandler();

        private enum Op : byte
        {
            PrivacyUpdate = 1
        }

        private StatsPrivacyNetHandler() : base(HandlerId) { }

        public void SendLocalPrivacy()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient) return;

            var privacy = ModContent.GetInstance<Config>().StatsPrivacy;

            var packet = GetPacket((byte)Op.PrivacyUpdate);
            packet.Write((byte)Main.myPlayer);
            packet.Write((byte)privacy);
            packet.Send();
        }

        public void BroadcastSingle(int who, Config.UserStatsPrivacy privacy)
        {
            if (Main.netMode != NetmodeID.Server) return;

            PrivacyCache.Set(who, privacy);

            var packet = GetPacket((byte)1); // Op.PrivacyUpdate
            packet.Write((byte)who);
            packet.Write((byte)privacy);
            packet.Send(); // broadcast to all
        }

        public void ServerSyncTo(int toClient)
        {
            if (Main.netMode != NetmodeID.Server) return;

            for (int i = 0; i < Main.maxPlayers; i++)
            {
                if (!Main.player[i].active) continue;

                var privacy = PrivacyCache.Get(i);

                var packet = GetPacket((byte)Op.PrivacyUpdate);
                packet.Write((byte)i);
                packet.Write((byte)privacy);
                packet.Send(toClient);
            }
        }

        public override void HandlePacket(BinaryReader reader, int fromWho)
        {
            var op = (Op)reader.ReadByte();
            if (op != Op.PrivacyUpdate) return;

            int playerId = reader.ReadByte();
            var privacy = (Config.UserStatsPrivacy)reader.ReadByte();

            PrivacyCache.Set(playerId, privacy);

            if (Main.netMode == NetmodeID.Server)
            {
                var packet = GetPacket((byte)Op.PrivacyUpdate);
                packet.Write((byte)playerId);
                packet.Write((byte)privacy);
                packet.Send(-1, fromWho);
            }
        }
    }
}
