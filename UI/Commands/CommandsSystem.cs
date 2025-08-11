using System.Collections.Generic;
using AdvancedChatFeatures.Common.Configs;
using AdvancedChatFeatures.UI.Commands;
using AdvancedChatFeatures.UI.Commands.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace AdvancedChatFeatures.UI
{
    [Autoload(Side = ModSide.Client)]
    public class CommandsSystem : ModSystem
    {
        public UserInterface ui;
        public CommandsListState commandsListState;

        // When the user hits Close, we keep the UI hidden until `/` goes away once.
        internal bool _snoozed;

        public override void OnWorldLoad()
        {
            Main.QueueMainThreadAction(() =>
            {
                ui = new UserInterface();
                commandsListState = new CommandsListState();
                ui.SetState(null); // start hidden
            });
        }

        public override void OnWorldUnload()
        {
            ui?.SetState(null);
            ui = null;
            commandsListState = null;
            _snoozed = false;
        }

        public override void UpdateUI(GameTime gameTime)
        {
            if (!Conf.C.AutoCompleteCommands || ui == null)
                return;

            bool chatOpen = Main.drawingPlayerChat;
            string text = Main.chatText ?? string.Empty;
            bool hasSlash = chatOpen && text.Length > 0 && text[0] == '/';

            if (!hasSlash && _snoozed)
                _snoozed = false;

            if (hasSlash && !_snoozed)
            {
                if (ui.CurrentState != commandsListState)
                    ui.SetState(commandsListState);
            }
            else if (ui.CurrentState != null)
            {
                ui.SetState(null);
            }

            // Mod filter cycle
            if (ui.CurrentState == commandsListState)
            {
                HeaderPanel header = commandsListState?.commandsPanel?.header;
                if (header != null)
                {
                    bool tab = Main.keyState.IsKeyDown(Keys.Tab) && Main.oldKeyState.IsKeyUp(Keys.Tab);
                    if (tab)
                    {
                        bool back = Main.keyState.IsKeyDown(Keys.LeftShift) || Main.keyState.IsKeyDown(Keys.RightShift);
                        header.modFilterButton?.CycleIndex(back ? -1 : +1);
                    }
                }
            }

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