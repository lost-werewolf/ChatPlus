using System.Collections.Generic;
using AdvancedChatFeatures.Common.Configs;
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

        public override void UpdateUI(GameTime gameTime)
        {
            if (!Conf.C.autocompleteConfig.EnableAutocomplete)
                return;

            string text = Main.chatText ?? string.Empty;
            // Open when starting an emoji token, ":" as the first char
            bool startsWithColon = text.Length > 0 && text[0] == ':';

            if (startsWithColon && Main.drawingPlayerChat)
            {
                // Only switch state when it actually changes
                if (ui.CurrentState != emojiState)
                {
                    ui.SetState(emojiState);
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