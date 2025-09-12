using System;
using System.IO;
using ChatPlus.Core.Netcode;

namespace ChatPlus.Core.Features.PlayerIcons.PlayerInfo.SessionTracker
{
    internal class SessionTrackerNetHandler : BasePacketHandler
    {
        public const byte HandlerId = 3;
        public static SessionTrackerNetHandler Instance { get; } = new SessionTrackerNetHandler(HandlerId);
        public SessionTrackerNetHandler(byte handlerType) : base(handlerType) { }

        public override void HandlePacket(BinaryReader reader, int fromWho)
        {
            
        }
    }
}
