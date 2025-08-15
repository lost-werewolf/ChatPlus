using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace AdvancedChatFeatures.Emojis
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
                Main.chatText = "[e"; // start with slash
                ui.SetState(emojiState);
            }
            else
            {
                if (ui.CurrentState != null)
                    ui.SetState(null);
            }
        }

        public override void UpdateUI(GameTime gameTime)
        {
            if (Main.drawingPlayerChat && EmojiTagHandler.IsTypingEmojiTag(Main.chatText))
            {
                if (ui.CurrentState != emojiState)
                {
                    ui.SetState(emojiState);
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