using System;
using System.IO;
using ChatPlus.Core.Netcode;
using static ChatPlus.Core.Features.PlayerIcons.PlayerInfo.SessionTrackerSystem;

namespace ChatPlus.Core.Features.PlayerIcons.PlayerInfo
{
    internal class SessionTrackerNetHandler : BasePacketHandler
    {
        public const byte HandlerId = 3;
        public static SessionTrackerNetHandler Instance { get; } = new SessionTrackerNetHandler(HandlerId);
        public SessionTrackerNetHandler(byte handlerType) : base(handlerType) { }

        public override void HandlePacket(BinaryReader reader, int fromWho)
        {
            var msgType = (SessionMessage)reader.ReadByte();

            switch (msgType)
            {
                case SessionMessage.SyncAll:
                    {
                        ClearAll();
                        int count = reader.ReadByte();
                        for (int i = 0; i < count; i++)
                        {
                            int index = reader.ReadByte();
                            long ticks = reader.ReadInt64();
                            SetServerJoinTime(index, new DateTime(ticks));
                        }
                        break;
                    }
                case SessionMessage.PlayerJoin:
                    {
                        int index = reader.ReadByte();
                        long ticks = reader.ReadInt64();
                        SetServerJoinTime(index, new DateTime(ticks));
                        break;
                    }
                case SessionMessage.PlayerLeave:
                    {
                        int index = reader.ReadByte();
                        RemoveServer(index);
                        break;
                    }
            }
        }
    }
}
