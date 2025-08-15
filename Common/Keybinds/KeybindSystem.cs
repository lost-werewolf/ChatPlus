using AdvancedChatFeatures.UI.Commands;
using AdvancedChatFeatures.UI.Emojis;
using Microsoft.Xna.Framework.Input;
using Terraria.GameInput;
using Terraria.ModLoader;

namespace AdvancedChatFeatures.Common.Keybinds
{
    public class KeybindSystem : ModSystem
    {
        public static ModKeybind OpenCommandKeybind;
        public static ModKeybind OpenEmojiKeybind;

        public override void Load()
        {
            OpenCommandKeybind = KeybindLoader.RegisterKeybind(Mod, "Open Command Window", Keys.Y);
            OpenEmojiKeybind = KeybindLoader.RegisterKeybind(Mod, "Open Emoji Window", Keys.U);
        }
    }

    public class KeybindPlayer : ModPlayer
    {
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (KeybindSystem.OpenCommandKeybind.JustPressed)
            {
                var sys = ModContent.GetInstance<CommandSystem>();
                sys.ToggleState();
            }

            if (KeybindSystem.OpenEmojiKeybind.JustPressed)
            {
                var sys = ModContent.GetInstance<EmojiSystem>();
                sys.ToggleState();
            }
        }
    }
}