using System;
using System.Collections.Generic;
using AdvancedChatFeatures.Common.Configs;
using AdvancedChatFeatures.UI.Commands;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace AdvancedChatFeatures.UI.Emojis
{
    [Autoload(Side = ModSide.Client)]
    public class EmojiSystem : ModSystem
    {
        public UserInterface ui;
        public EmojiState emojiState;

        public override void OnModLoad()
        {
            base.OnModLoad();
        }

        public override void Load()
        {
            ui = new UserInterface();
            emojiState = new EmojiState();
            ui.SetState(null); // start hidden
        }

        public override void Unload()
        {
            ui = new UserInterface();
            emojiState = new EmojiState();
            ui.SetState(null); // start hidden
        }

        public override void OnWorldUnload()
        {
            base.OnWorldUnload();
        }

        public void ToggleState()
        {
            if (ui.CurrentState != emojiState)
            {
                Main.drawingPlayerChat = true; // force open chat
                Main.chatText = ":"; // start with slash
                ui.SetState(emojiState);
                emojiState.emojiPanel.PopulateEmojiPanel();
            }
            else
            {
                if (ui.CurrentState != null)
                    ui.SetState(null);
            }
        }

        public override void UpdateUI(GameTime gameTime)
        {
            if (!Conf.C.autocompleteConfig.EnableAutocomplete)
                return;

            string text = Main.chatText ?? string.Empty;
            bool typingEmoji = IsTypingEmojiTag(text);

            if (typingEmoji && Main.drawingPlayerChat)
            {
                if (ui.CurrentState != emojiState)
                {
                    ui.SetState(emojiState);
                    // populate only when opening to avoid perf hits
                    emojiState.emojiPanel.PopulateEmojiPanel();
                }
                ui.Update(gameTime);
            }
            else
            {
                if (ui.CurrentState != null)
                    ui.SetState(null);
            }
        }

        // Opens when the *last* '[' begins an emoji tag and there's no closing ']'
        private static bool IsTypingEmojiTag(string text)
        {
            if (string.IsNullOrEmpty(text))
                return false;

            int i = text.LastIndexOf('[');
            if (i < 0)
                return false;

            // Substring from last '[' to end
            string tail = text.Substring(i);
            // If there's a closing bracket after it, tag is complete -> don't open
            if (tail.IndexOf(']') >= 0)
                return false;

            // Case-insensitive check for "[e" or "[emoji"
            return tail.StartsWith("[e", StringComparison.OrdinalIgnoreCase)
                || tail.StartsWith("[emoji", StringComparison.OrdinalIgnoreCase);
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int index = layers.FindIndex(l => l.Name.Equals("Vanilla: Death Text"));
            if (index == -1) return;

            layers.Insert(index, new LegacyGameInterfaceLayer(
                "AdvancedChatFeatures: Emojis Panel",
                () =>
                {
                    if (ui?.CurrentState != null)
                        ui.Draw(Main.spriteBatch, new GameTime());
                    return true;
                },
                InterfaceScaleType.UI
            ));
        }
    }
}