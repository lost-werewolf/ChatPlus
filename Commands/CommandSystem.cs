using System.Collections.Generic;
using AdvancedChatFeatures.Common.Configs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Config.UI;
using Terraria.UI;

namespace AdvancedChatFeatures.Commands
{
    [Autoload(Side = ModSide.Client)]
    public class CommandSystem : ModSystem
    {
        public UserInterface ui;
        public CommandState commandState;

        public override void Load()
        {
            ui = new UserInterface();
            commandState = new CommandState();
            ui.SetState(null); // start hidden
        }

        public override void Unload()
        {
            ui = new UserInterface();
            commandState = new CommandState();
            ui.SetState(null); // start hidden
        }

        public override void OnWorldUnload()
        {
            base.OnWorldUnload();
        }

        public void ToggleState()
        {
            if (ui.CurrentState != commandState)
            {
                Main.drawingPlayerChat = true; // force open chat
                Main.chatText = "/"; // start with slash
                ui.SetState(commandState);
            }
            else
            {
                if (ui.CurrentState != null)
                    ui.SetState(null);
            }
        }

        public override void UpdateUI(GameTime gameTime)
        {
            if (Main.gameMenu == false && Main.InGameUI.CurrentState is UIModConfig)
                return;

            string text = Main.chatText ?? string.Empty;
            bool startsWithSlash = text.Length > 0 && text[0] == '/';

            if (startsWithSlash && Main.drawingPlayerChat)
            {
                if (ui.CurrentState != commandState)
                {
                    ui.SetState(commandState);
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