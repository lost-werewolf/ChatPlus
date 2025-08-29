﻿using System.Collections.Generic;
using ChatPlus.Core.Helpers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace ChatPlus.Core.Features.Items
{
    [Autoload(Side = ModSide.Client)]
    public class ItemSystem : ModSystem
    {
        public UserInterface ui;
        public ItemState state;

        public override void Load()
        {
            ui = new UserInterface();
            state = new ItemState();
            ui.SetState(null);
        }

        public override void Unload()
        {
            ui = new UserInterface();
            state = new ItemState();
            ui.SetState(null);
        }

        public override void UpdateUI(GameTime gameTime)
        {
            StateManager.OpenStateIfPrefixMatches(gameTime, ui, state, "[i");
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int index = layers.FindIndex(l => l.Name.Equals("Vanilla: Death Text"));
            if (index == -1) return;

            layers.Insert(index, new LegacyGameInterfaceLayer(
                "ChatPlus: ItemWindowSystem",
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
