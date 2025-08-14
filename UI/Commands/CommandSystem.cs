using System.Collections.Generic;
using AdvancedChatFeatures.Common.Configs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace AdvancedChatFeatures.UI.Commands
{
    [Autoload(Side = ModSide.Client)]
    public class CommandSystem : ModSystem
    {
        public UserInterface ui;
        public CommandState commandState;

        public override void OnModLoad()
        {
            base.OnModLoad();
        }

        public override void OnWorldLoad()
        {
            ui = new UserInterface();
            commandState = new CommandState();
            ui.SetState(null); // start hidden
        }

        public override void OnWorldUnload()
        {
            ui?.SetState(null);
            ui = null;
            commandState = null;
        }

        public override void UpdateUI(GameTime gameTime)
        {
            if (!Conf.C.AutoCompleteCommands)
                return;

            string text = Main.chatText ?? string.Empty;
            bool startsWithSlash = text.Length > 0 && text[0] == '/';

            if (startsWithSlash && Main.drawingPlayerChat)
            {
                // Only switch state when it actually changes
                if (ui.CurrentState != commandState)
                {
                    ui.SetState(commandState);
                    commandState.commandPanel.PopulateCommandPanel(selectFirst: false);
                    commandState.commandPanel.currentIndex = -1;
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