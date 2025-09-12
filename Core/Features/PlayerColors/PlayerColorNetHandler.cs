using System.IO;
using ChatPlus.Core.Features.Mentions;
using ChatPlus.Core.Netcode;
using Terraria;
using Terraria.ID;

namespace ChatPlus.Core.Features.PlayerColors
{
    internal sealed class PlayerColorNetHandler : BasePacketHandler
    {
        public const byte HandlerId = 2;
        public static PlayerColorNetHandler Instance { get; } = new PlayerColorNetHandler();

        private enum Msg : byte 
        { 
            Hello = 1, 
            SyncSingle = 2, 
            SyncAll = 3 
        }

        private PlayerColorNetHandler() : base(HandlerId) { }

        public static void ClientHello(int who, string hex)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient) return;
            if (string.IsNullOrWhiteSpace(hex)) hex = "FFFFFF";

            var p = Instance.GetPacket((byte)Msg.Hello);
            p.Write((byte)who);
            p.Write(hex);
            p.Send();
        }

        public override void HandlePacket(BinaryReader reader, int fromWho)
        {
            var msg = (Msg)reader.ReadByte();

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                switch (msg)
                {
                    case Msg.SyncSingle:
                        {
                            byte who = reader.ReadByte();
                            string hex = reader.ReadString();

                            AssignPlayerColorsSystem.PlayerColors[who] = SanHex(hex);

                            var name = Main.player[who]?.name;
                            if (!string.IsNullOrWhiteSpace(name))
                            {
                                MentionSnippet.InvalidateCachesFor(name);
                            }
                            else
                            {
                                MentionSnippet.ClearAllCaches();
                            }
                            break;
                        }

                    case Msg.SyncAll:
                        {
                            AssignPlayerColorsSystem.PlayerColors.Clear();
                            int count = reader.ReadByte();
                            for (int i = 0; i < count; i++)
                            {
                                byte who = reader.ReadByte();
                                string hex = reader.ReadString();
                                AssignPlayerColorsSystem.PlayerColors[who] = SanHex(hex);
                            }
                            MentionSnippet.ClearAllCaches();
                            break;
                        }
                }
                return;
            }

            if (Main.netMode == NetmodeID.Server)
            {
                switch (msg)
                {
                    case Msg.Hello:
                        {
                            byte who = reader.ReadByte();
                            string requested = SanHex(reader.ReadString());
                            string assigned = requested;

                            if (assigned == "FFFFFF")
                            {
                                AssignPlayerColorsSystem.PlayerColors.Remove(who);
                            }
                            else
                            {
                                AssignPlayerColorsSystem.PlayerColors[who] = assigned;
                            }

                            // Send full table back to the joiner
                            var all = Instance.GetPacket((byte)Msg.SyncAll);
                            var map = AssignPlayerColorsSystem.PlayerColors;
                            all.Write((byte)map.Count);
                            foreach (var kv in map)
                            {
                                all.Write((byte)kv.Key);
                                all.Write(SanHex(kv.Value));
                            }
                            all.Send(toClient: who);

                            // Broadcast this player's color to everyone
                            var one = Instance.GetPacket((byte)Msg.SyncSingle);
                            one.Write(who);
                            one.Write(assigned);
                            one.Send();

                            break;
                        }
                }
            }
        }

        private static string SanHex(string hex)
        {
            if (string.IsNullOrWhiteSpace(hex)) return "FFFFFF";
            hex = hex.Trim().TrimStart('#');
            if (hex.Length != 6) return "FFFFFF";
            // basic validation: hex chars only
            for (int i = 0; i < 6; i++)
            {
                char c = hex[i];
                bool ok = c >= '0' && c <= '9' || c >= 'a' && c <= 'f' || c >= 'A' && c <= 'F';
                if (!ok) return "FFFFFF";
            }
            return hex;
        }
    }
}
