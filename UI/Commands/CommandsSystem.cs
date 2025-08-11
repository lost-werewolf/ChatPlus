using System.Collections.Generic;
using AdvancedChatFeatures.Common.Configs;
using AdvancedChatFeatures.Helpers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace AdvancedChatFeatures.UI.Commands
{
    [Autoload(Side = ModSide.Client)]
    public class CommandsSystem : ModSystem
    {
        public UserInterface ui;
        public CommandsListState commandsListState;

        public override void OnWorldLoad()
        {
            ui = new();
            commandsListState = new();
            ui.SetState(commandsListState);
        }

        public override void OnWorldUnload()
        {
            // Cleanup
            ui?.SetState(null);
            ui = null;
            commandsListState = null;
        }

        public override void UpdateUI(GameTime gameTime)
        {
            if (ui == null) return;

            bool chatOpen = Main.drawingPlayerChat;
            bool enabled = Conf.C.AutoCompleteCommands;
            string text = Main.chatText ?? string.Empty;
            bool shouldOpen = chatOpen && enabled && text.Length > 0 && text[0] == '/';

            //Main.NewText(shouldOpen.ToString());

            var target = shouldOpen ? (UIState)commandsListState : null;
            if (ui.CurrentState != target)
                ui.SetState(target);

            if (ui.CurrentState != null)
                ui.Update(gameTime);
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int index = layers.FindIndex(l => l.Name.Equals("Vanilla: Death Text"));
            if (index == -1) return;

            layers.Insert(index, new LegacyGameInterfaceLayer(
                "AdvancedChatFeatures: Commands Panel",
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
