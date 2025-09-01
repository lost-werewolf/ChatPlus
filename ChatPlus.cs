using System.IO;
using ChatPlus.Core.Netcode;
using Terraria.ModLoader;

namespace ChatPlus;
public sealed class ChatPlus : Mod
{
    public override void HandlePacket(BinaryReader reader, int whoAmI)
    {
        ModNetHandler.HandlePacket(reader, whoAmI);
    }
}