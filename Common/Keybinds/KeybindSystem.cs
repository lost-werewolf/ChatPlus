using AdvancedChatFeatures.ColorWindow;
using AdvancedChatFeatures.Commands;
using AdvancedChatFeatures.Emojis;
using AdvancedChatFeatures.Glyphs;
using AdvancedChatFeatures.Helpers;
using AdvancedChatFeatures.ItemWindow;
using AdvancedChatFeatures.Uploads;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.UI;

namespace AdvancedChatFeatures.Common.Keybinds
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
            void Toggle<TSystem, TState>(ModKeybind keybind, TSystem sys, UserInterface ui, TState state)
                where TSystem : ModSystem
                where TState : UIState
            {
                if (!keybind.JustPressed) return;

                // Ensure chat is open
                if (!Main.drawingPlayerChat)
                {
                    Main.drawingPlayerChat = true;
                    Main.chatRelease = false;
                    Main.chatText = string.Empty; // optional: start fresh
                }

                // Toggle state
                if (StateHelper.IsActive(ui))
                    StateHelper.Close(ui);
                else
                    StateHelper.OpenExclusive(ui, state);
            }

            Toggle(KeybindSystem.OpenCommandKeybind, ModContent.GetInstance<CommandSystem>(),
                ModContent.GetInstance<CommandSystem>().ui, ModContent.GetInstance<CommandSystem>().commandState);

            Toggle(KeybindSystem.OpenColorWindowKeybind, ModContent.GetInstance<ColorWindowSystem>(),
                ModContent.GetInstance<ColorWindowSystem>().ui, ModContent.GetInstance<ColorWindowSystem>().colorWindowState);

            Toggle(KeybindSystem.OpenEmojiKeybind, ModContent.GetInstance<EmojiSystem>(),
                ModContent.GetInstance<EmojiSystem>().ui, ModContent.GetInstance<EmojiSystem>().emojiState);

            Toggle(KeybindSystem.OpenGlyphKeybind, ModContent.GetInstance<GlyphSystem>(),
                ModContent.GetInstance<GlyphSystem>().ui, ModContent.GetInstance<GlyphSystem>().glyphState);

            Toggle(KeybindSystem.OpenItemWindowKeybind, ModContent.GetInstance<ItemWindowSystem>(),
                ModContent.GetInstance<ItemWindowSystem>().ui, ModContent.GetInstance<ItemWindowSystem>().itemWindowState);

            Toggle(KeybindSystem.OpenUploadWindow, ModContent.GetInstance<UploadSystem>(),
                ModContent.GetInstance<UploadSystem>().ui, ModContent.GetInstance<UploadSystem>().state);
        }
    }
}