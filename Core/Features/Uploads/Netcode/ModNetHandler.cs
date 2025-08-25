using System.IO;
using ChatPlus.Core.Helpers;

namespace ChatPlus.Core.Features.Uploads.Netcode
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

                default:
                    Log.Warn("Unknown packet handler type: " + handlerType);
                    break;
            }
        }
    }

}