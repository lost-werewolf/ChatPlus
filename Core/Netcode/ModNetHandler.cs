using System.IO;
using ChatPlus.Core.Features.PlayerColors;
using ChatPlus.Core.Features.PlayerIcons.PlayerInfo.SessionTracker;
using ChatPlus.Core.Features.PlayerIcons.PlayerInfo.StatsPrivacy;
using ChatPlus.Core.Features.TypingIndicators;
using ChatPlus.Core.Features.Uploads;
using ChatPlus.Core.Helpers;

namespace ChatPlus.Core.Netcode
{
    internal class ModNetHandler
    {
        public static void HandlePacket(BinaryReader r, int fromWho)
        {
            byte handlerType = r.ReadByte();

            switch (handlerType)
            {
                case UploadNetHandler.HandlerId:
                    UploadNetHandler.Instance.HandlePacket(r, fromWho);
                    break;

                case PlayerColorNetHandler.HandlerId:
                    PlayerColorNetHandler.Instance.HandlePacket(r, fromWho);
                    break;

                case SessionTrackerNetHandler.HandlerId:
                    SessionTrackerNetHandler.Instance.HandlePacket(r, fromWho);
                    break;

                case StatsPrivacyNetHandler.HandlerId:
                    StatsPrivacyNetHandler.Instance.HandlePacket(r, fromWho);
                    break;

                case TypingIndicatorNetHandler.HandlerId:
                    TypingIndicatorNetHandler.Instance.HandlePacket(r, fromWho);
                    break;

                default:
                    Log.Warn("Unknown packet handler type: " + handlerType);
                    break;
            }
        }
    }
}
