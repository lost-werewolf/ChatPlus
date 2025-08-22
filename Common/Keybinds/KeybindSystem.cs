using ChatPlus.ColorHandler;
using ChatPlus.CommandHandler;
using ChatPlus.Common.Systems;
using ChatPlus.EmojiHandler;
using ChatPlus.GlyphHandler;
using ChatPlus.ItemHandler;
using ChatPlus.UploadHandler;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.UI;

namespace ChatPlus.Common.Keybinds
{
    public class KeybindSystem : ModSystem
    {
        public static ModKeybind OpenCommandKeybind;
        public static ModKeybind OpenEmojiKeybind;
        public static ModKeybind OpenGlyphKeybind;
        public static ModKeybind OpenItemWindowKeybind;
        public static ModKeybind OpenColorWindowKeybind;
        public static ModKeybind OpenUploadWindow;

        public override void Load()
        {
            OpenCommandKeybind = KeybindLoader.RegisterKeybind(Mod, "Open Command Window", Keys.C);
            OpenEmojiKeybind = KeybindLoader.RegisterKeybind(Mod, "Open Emoji Window", Keys.E);
            OpenGlyphKeybind = KeybindLoader.RegisterKeybind(Mod, "Open Glyph Window", Keys.G);
            OpenItemWindowKeybind = KeybindLoader.RegisterKeybind(Mod, "Open Item Window", Keys.I);
            OpenColorWindowKeybind = KeybindLoader.RegisterKeybind(Mod, "Open Color Window", Keys.O);
            OpenUploadWindow = KeybindLoader.RegisterKeybind(Mod, "Open Upload Window", Keys.U);
        }
    }

    public class KeybindPlayer : ModPlayer
    {
        public override void ProcessTriggers(TriggersSet t)
        {
            void OpenSystem<TSystem>(ModKeybind keybind, TSystem sys, string prefix) where TSystem : ModSystem
            {
                if (!keybind.JustPressed)
                    return;

                // If chat is open, keybind is ignored
                if (Main.drawingPlayerChat)
                    return;

                // Force open chat and input
                Main.drawingPlayerChat = true;
                PlayerInput.WritingText = true;
                Main.chatRelease = false;

                // Add the system prefix.
                // The system will handle opening the state by checking for the prefix.
                Main.chatText = prefix;

                // Set caret to last
                HandleChatSystem.SetCaretPos(Main.chatText.Length);
            }

            OpenSystem(KeybindSystem.OpenCommandKeybind, ModContent.GetInstance<CommandSystem>(), "/");
            OpenSystem(KeybindSystem.OpenEmojiKeybind, ModContent.GetInstance<EmojiSystem>(), "[e");
            OpenSystem(KeybindSystem.OpenGlyphKeybind, ModContent.GetInstance<GlyphSystem>(), "[g");
            OpenSystem(KeybindSystem.OpenItemWindowKeybind, ModContent.GetInstance<ItemSystem>(), "[i");
            OpenSystem(KeybindSystem.OpenColorWindowKeybind, ModContent.GetInstance<ColorSystem>(), "[c");
            OpenSystem(KeybindSystem.OpenUploadWindow, ModContent.GetInstance<UploadSystem>(), "[u");
        }
    }
}