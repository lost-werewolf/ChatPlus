using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace ChatPlus.Core.Features.Scrollbar
{
    public class ChatScrollSystem : ModSystem
    {
        private UserInterface ui;
        public ChatScrollState state;

        public override void PostSetupContent()
        {
            ui = new UserInterface();
            state = new ChatScrollState();
            ui.SetState(null);
        }

        /// <summary>
        /// Sets the current state to a scroll state if chat is open, or null otherwise.
        /// </summary>
        public override void UpdateUI(GameTime gameTime)
        {
            if (!Main.drawingPlayerChat)
            {
                // Close scroll state
                if (ui.CurrentState != null) ui.SetState(null);
                return;
            }

            if (ui.CurrentState != state)
            {
                // Open scroll state
                ui.SetState(state);
                Main.chatMonitor.ResetOffset();
                state.chatScrollbar.GoToBottom();
            }

            ui.Update(gameTime);
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int vanillaDeathTextLayerIndex = layers.FindIndex(layer => layer.Name == "Vanilla: Death Text");
            if (vanillaDeathTextLayerIndex != -1)
            {
                layers.Insert(vanillaDeathTextLayerIndex, new LegacyGameInterfaceLayer(
                "ChatPlus: Scroll System",
                () =>
                {
                    if (Main.drawingPlayerChat && ui?.CurrentState != null)
                        ui.Draw(Main.spriteBatch, Main.gameTimeCache);
                    return true;
                },
                InterfaceScaleType.UI
            ));
            }
        }
    }
}
