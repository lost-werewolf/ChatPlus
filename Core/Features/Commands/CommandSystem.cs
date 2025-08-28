﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace ChatPlus.Core.Features.Commands
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

        private bool JustPressed(Keys key) => Main.keyState.IsKeyDown(key) && Main.oldKeyState.IsKeyUp(key);

        public override void UpdateUI(GameTime gameTime)
        {
            if (!Main.drawingPlayerChat)
            {
                if (ui.CurrentState != null)
                {
                    ui.SetState(null);
                }
                return;
            }

            char prefix = '/';

            if (Main.chatText.StartsWith(prefix))
            {
                if (ui.CurrentState != commandState)
                {
                    ui.SetState(commandState);
                }
                ui.Update(gameTime);
            }
            else
            {
                if (ui.CurrentState == commandState)
                {
                    ui.SetState(null);
                }
            }
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int index = layers.FindIndex(l => l.Name.Equals("Vanilla: Death Text"));
            if (index == -1) return;

            layers.Insert(index, new LegacyGameInterfaceLayer(
                "ChatPlus: Commands Panel",
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