using Microsoft.Xna.Framework.Input;
using Terraria.ModLoader;

namespace AdvancedChatFeatures.Common.Keybinds
{
    // Acts as a container for keybinds registered by this mod.
    // See Common/Players/ExampleKeybindPlayer for usage.
    public class KeybindSystem : ModSystem
    {
        public static ModKeybind ToggleChatKeybind { get; private set; }

        public override void Load()
        {
            // Registers a new keybind
            // We localize keybinds by adding a Mods.{ModName}.Keybind.{KeybindName} entry to our localization files. The actual text displayed to English users is in en-US.hjson
            ToggleChatKeybind = KeybindLoader.RegisterKeybind(
                mod: Mod,
                name: "ToggleChat",
                defaultBinding: Keys.Enter);
        }

        // Please see ExampleMod.cs' Unload() method for a detailed explanation of the unloading process.
        public override void Unload()
        {
            // Not required if your AssemblyLoadContext is unloading properly, but nulling out static fields can help you figure out what's keeping it loaded.
            ToggleChatKeybind = null;
        }
    }
}