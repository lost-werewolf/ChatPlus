using AdvancedChatFeatures.ColorWindow;
using AdvancedChatFeatures.Commands;
using AdvancedChatFeatures.Emojis;
using AdvancedChatFeatures.Glyphs;
using AdvancedChatFeatures.Helpers;
using AdvancedChatFeatures.ItemWindow;
using AdvancedChatFeatures.ImageWindow;
using Microsoft.Xna.Framework.Input;
using Terraria.GameInput;
using Terraria.ModLoader;

namespace AdvancedChatFeatures.Common.Keybinds
{
    public class KeybindSystem : ModSystem
    {
        public static ModKeybind OpenCommandKeybind;
        public static ModKeybind OpenEmojiKeybind;
        public static ModKeybind OpenGlyphKeybind;
        public static ModKeybind OpenItemWindowKeybind;
        public static ModKeybind OpenColorWindowKeybind;
        public static ModKeybind OpenImageWindowKeybind;

        public override void Load()
        {
            OpenCommandKeybind = KeybindLoader.RegisterKeybind(Mod, "Open Command Window", Keys.C);
            OpenEmojiKeybind = KeybindLoader.RegisterKeybind(Mod, "Open Emoji Window", Keys.E);
            OpenGlyphKeybind = KeybindLoader.RegisterKeybind(Mod, "Open Glyph Window", Keys.G);
            OpenItemWindowKeybind = KeybindLoader.RegisterKeybind(Mod, "Open Item Window", Keys.I);
            OpenColorWindowKeybind = KeybindLoader.RegisterKeybind(Mod, "Open Color Window", Keys.O);
            OpenImageWindowKeybind = KeybindLoader.RegisterKeybind(Mod, "Open Upload Window", Keys.U);
        }
    }

    public class KeybindPlayer : ModPlayer
    {
        public override void ProcessTriggers(TriggersSet t)
        {
            if (KeybindSystem.OpenCommandKeybind.JustPressed)
            {
                var sys = ModContent.GetInstance<CommandSystem>();
                if (StateHelper.IsActive(sys.ui)) StateHelper.Close(sys.ui);
                else StateHelper.OpenExclusive(sys.ui, sys.commandState);
            }

            if (KeybindSystem.OpenColorWindowKeybind.JustPressed)
            {
                var sys = ModContent.GetInstance<ColorWindowSystem>();
                if (StateHelper.IsActive(sys.ui)) StateHelper.Close(sys.ui);
                else StateHelper.OpenExclusive(sys.ui, sys.colorWindowState);
            }

            if (KeybindSystem.OpenEmojiKeybind.JustPressed)
            {
                var sys = ModContent.GetInstance<EmojiSystem>();
                if (StateHelper.IsActive(sys.ui)) StateHelper.Close(sys.ui);
                else StateHelper.OpenExclusive(sys.ui, sys.emojiState);
            }

            if (KeybindSystem.OpenGlyphKeybind.JustPressed)
            {
                var sys = ModContent.GetInstance<GlyphSystem>(); // <-- not EmojiSystem
                if (StateHelper.IsActive(sys.ui)) StateHelper.Close(sys.ui);
                else StateHelper.OpenExclusive(sys.ui, sys.glyphState);
            }

            if (KeybindSystem.OpenItemWindowKeybind.JustPressed)
            {
                var sys = ModContent.GetInstance<ItemWindowSystem>();
                if (StateHelper.IsActive(sys.ui)) StateHelper.Close(sys.ui);
                else StateHelper.OpenExclusive(sys.ui, sys.itemWindowState);
            }

            if (KeybindSystem.OpenImageWindowKeybind.JustPressed)
            {
                var sys = ModContent.GetInstance<ImageSystem>();
                if (StateHelper.IsActive(sys.ui)) StateHelper.Close(sys.ui);
                else StateHelper.OpenExclusive(sys.ui, sys.state);
            }
        }
    }
}