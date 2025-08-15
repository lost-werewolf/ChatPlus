using Terraria.ModLoader;

namespace AdvancedChatFeatures.Commands
{
    /// <summary>
    /// A command structure that represents a chat command.
    /// Can be used to define commands for the chat system.
    /// </summary>
    /// <param name="Name"></param>
    /// <param name="Usage"></param>
    /// <param name="Mod"></param>
    public readonly record struct Command(string Name, string Usage = null, Mod Mod = null);
}
