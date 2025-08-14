using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace AdvancedChatFeatures.UI.Commands
{
    /// <summary>
    /// A structure representing a command entry in the command list.
    /// </summary>
    public readonly record struct Command(
        string Name,
        string Usage = null,
        Mod Mod = null
    );
}
