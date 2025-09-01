using System.IO;
using ChatPlus.Core.Features.PlayerColors;
using ChatPlus.Core.Features.Uploads;
using ChatPlus.Core.Helpers;

namespace ChatPlus.Core.Netcode;

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

            default:
                Log.Warn("Unknown packet handler type: " + handlerType);
                break;
        }
    }
}
