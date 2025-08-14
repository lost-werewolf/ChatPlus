using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace AdvancedChatFeatures.UI.Commands
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="Name"> The name of the command, e.g /help</param>
    /// <param name="Usage">A description of how the command should be used, e.g "Displays a list of commands"</param>
    /// <param name="Mod">The mod that this command belongs to. May be null</param>
    public readonly record struct Command(
        string Name,
        string Usage = null,
        Mod Mod = null
    );
}
