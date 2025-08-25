using System.IO;
using ChatPlus.Core.Features.Uploads.Netcode;
using Terraria.ModLoader;

namespace ChatPlus;
public sealed class ChatPlus : Mod
{
    public override void HandlePacket(BinaryReader reader, int whoAmI)
    {
        ModNetHandler.HandlePacket(reader, whoAmI);
    }
}