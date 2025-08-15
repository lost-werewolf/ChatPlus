using Terraria.ModLoader;

namespace AdvancedChatFeatures.UI.Commands
{
    public readonly record struct Command(string Name, string Usage = null, Mod Mod = null);
}
