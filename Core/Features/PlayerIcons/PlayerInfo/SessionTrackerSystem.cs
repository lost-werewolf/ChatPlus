using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ChatPlus.Core.Features.PlayerIcons.PlayerInfo
{
    public partial class SessionTrackerSystem : ModSystem
    {
        // Server-authoritative timestamps (MP only; set on server and replicated)
        private static readonly Dictionary<int, DateTime> serverSessions = new();

        // Local timestamps (used in SP and as a fallback on clients until server sync arrives)
        private static readonly Dictionary<int, DateTime> localSessions = new();

        internal enum SessionMessage : byte
        {
            SyncAll,
            PlayerJoin,
            PlayerLeave
        }

        public override void OnWorldUnload()
        {
            serverSessions.Clear();
            localSessions.Clear();
        }

        // SERVER: detect joins/leaves and broadcast
        public override void PostUpdatePlayers()
        {
            if (Main.netMode != NetmodeID.Server)
                return;

            for (int i = 0; i < Main.maxPlayers; i++)
            {
                var plr = Main.player[i];

                if (plr.active)
                {
                    if (!serverSessions.ContainsKey(i))
                    {
                        var joinedAt = DateTime.UtcNow;
                        serverSessions[i] = joinedAt;

                        // tell everyone this player joined
                        SendPlayerJoin(i, joinedAt);

                        // tell the newcomer about everyone currently tracked
                        SendFullSync(i);
                    }
                }
                else
                {
                    if (serverSessions.Remove(i))
                        SendPlayerLeave(i);
                }
            }
        }
        public static void MarkLocalJoin(int index, DateTime start) => localSessions[index] = start;
        public static void MarkLocalLeave(int index) => localSessions.Remove(index);

        public static void SetServerJoinTime(int index, DateTime start)
        {
            serverSessions[index] = start;
            // When server tells us, prefer server time, and drop any local placeholder
            localSessions.Remove(index);
        }

        public static void RemoveServer(int index) => serverSessions.Remove(index);

        public static void ClearAll()
        {
            serverSessions.Clear();
            localSessions.Clear();
        }

        public static string GetSessionDurationIngameDays(int playerIndex)
        {
            // Prefer server-authoritative start, else fall back to local start
            DateTime start;
            if (!serverSessions.TryGetValue(playerIndex, out start))
            {
                if (!localSessions.TryGetValue(playerIndex, out start))
                    return string.Empty;
            }

            TimeSpan elapsed = DateTime.UtcNow - start;

            // 1 in-game day = 24 real minutes = 1440 real seconds
            const double RealSecondsPerIngameDay = 24 * 60; // 1440
            double days = elapsed.TotalSeconds / RealSecondsPerIngameDay;

            // Show with two decimals (e.g., "Days: 3.42")
            return $"{days:0.00}";
        }

        // Prefer server → else local → else empty
        public static string GetSessionDuration(int playerIndex)
        {
            DateTime start;
            if (!serverSessions.TryGetValue(playerIndex, out start))
            {
                if (!localSessions.TryGetValue(playerIndex, out start))
                    return string.Empty;
            }

            var span = DateTime.UtcNow - start;

            if (span.TotalHours >= 1)
                return $"{(int)span.TotalHours:D2}:{span.Minutes:D2}:{span.Seconds:D2}";

            return $"{(int)span.TotalMinutes:D2}:{span.Seconds:D2}";
        }

        private static void SendFullSync(int toClient)
        {
            var packet = ModContent.GetInstance<ChatPlus>().GetPacket();
            packet.Write((byte)SessionMessage.SyncAll);
            packet.Write((byte)serverSessions.Count);
            foreach (var kv in serverSessions)
            {
                packet.Write((byte)kv.Key);
                packet.Write(kv.Value.Ticks);
            }
            packet.Send(toClient: toClient);
        }
        private static void SendPlayerJoin(int index, DateTime time)
        {
            var packet = ModContent.GetInstance<ChatPlus>().GetPacket();
            packet.Write(SessionTrackerNetHandler.HandlerId);     
            packet.Write((byte)SessionMessage.PlayerJoin);
            packet.Write((byte)index);
            packet.Write(time.Ticks);
            packet.Send();
        }

        private static void SendPlayerLeave(int index)
        {
            var packet = ModContent.GetInstance<ChatPlus>().GetPacket();
            packet.Write(SessionTrackerNetHandler.HandlerId);   
            packet.Write((byte)SessionMessage.PlayerLeave);
            packet.Write((byte)index);
            packet.Send();
        }

        public class SessionTrackerPlayer : ModPlayer
        {
            public override void OnEnterWorld()
            {
                // On clients & single-player, store a local start time
                if (Main.netMode != NetmodeID.Server)
                    MarkLocalJoin(Player.whoAmI, DateTime.UtcNow);
            }
        }
    }
}
