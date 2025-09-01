using ChatPlus.Core.Chat;
using ChatPlus.Core.Features.Colors;
using ChatPlus.Core.Features.Commands;
using ChatPlus.Core.Features.Emojis;
using ChatPlus.Core.Features.Glyphs;
using ChatPlus.Core.Features.Items;
using ChatPlus.Core.Features.ModIcons;
using ChatPlus.Core.Features.PlayerIcons;
using ChatPlus.Core.Features.Scrollbar;
using ChatPlus.Core.Features.Uploads;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;

namespace ChatPlus.Common.Keybinds
{
    public class KeybindSystem : ModSystem
    {
        public static ModKeybind OpenCommandKeybind;
        public static ModKeybind OpenColorWindowKeybind;
        public static ModKeybind OpenEmojiKeybind;
        public static ModKeybind OpenGlyphKeybind;
        public static ModKeybind OpenItemWindowKeybind;
        public static ModKeybind OpenModWindowKeybind;
        public static ModKeybind OpenPlayerWindowKeybind;
        public static ModKeybind OpenUploadWindow;

        // Debug
        public static ModKeybind WriteLineCount;

        public override void Load()
        {
            OpenCommandKeybind = KeybindLoader.RegisterKeybind(Mod, "Open Command Window", Keys.C);
            OpenColorWindowKeybind = KeybindLoader.RegisterKeybind(Mod, "Open Color Window", Keys.O);
            OpenEmojiKeybind = KeybindLoader.RegisterKeybind(Mod, "Open Emoji Window", Keys.E);
            OpenGlyphKeybind = KeybindLoader.RegisterKeybind(Mod, "Open Glyph Window", Keys.G);
            OpenItemWindowKeybind = KeybindLoader.RegisterKeybind(Mod, "Open Item Window", Keys.I);
            OpenModWindowKeybind = KeybindLoader.RegisterKeybind(Mod, "Open Mod Window", Keys.M);
            OpenPlayerWindowKeybind = KeybindLoader.RegisterKeybind(Mod, "Open Player Window", Keys.P);
            OpenUploadWindow = KeybindLoader.RegisterKeybind(Mod, "Open Upload Window", Keys.U);

#if DEBUG
            WriteLineCount = KeybindLoader.RegisterKeybind(Mod, "DEBUG: Write Chat Line Count", Keys.L);
#endif
        }
    }

    public class KeybindPlayer : ModPlayer
    {
        public override void ProcessTriggers(TriggersSet t)
        {
#if DEBUG
            if (KeybindSystem.WriteLineCount.JustPressed)
            {
                int count = 10;
                for (int i = 0; i < count; i++)
                {
                    Main.NewText($"Line count: {ChatScrollList.GetTotalLines()}");
                }
            }
#endif

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
                // The system will handle opening the chatScrollState by checking for the prefix.
                Main.chatText = prefix;

                // Set caret to last
                HandleChatSystem.SetCaretPos(Main.chatText.Length);
            }

            OpenSystem(KeybindSystem.OpenCommandKeybind, ModContent.GetInstance<CommandSystem>(), "/");
            OpenSystem(KeybindSystem.OpenColorWindowKeybind, ModContent.GetInstance<ColorSystem>(), "[c");
            OpenSystem(KeybindSystem.OpenEmojiKeybind, ModContent.GetInstance<EmojiSystem>(), "[e");
            OpenSystem(KeybindSystem.OpenGlyphKeybind, ModContent.GetInstance<GlyphSystem>(), "[g");
            OpenSystem(KeybindSystem.OpenItemWindowKeybind, ModContent.GetInstance<ItemSystem>(), "[i");
            OpenSystem(KeybindSystem.OpenModWindowKeybind, ModContent.GetInstance<ModIconSystem>(), "[m");
            OpenSystem(KeybindSystem.OpenPlayerWindowKeybind, ModContent.GetInstance<PlayerIconSystem>(), "[p");
            OpenSystem(KeybindSystem.OpenUploadWindow, ModContent.GetInstance<UploadSystem>(), "[u");
        }
    }
}