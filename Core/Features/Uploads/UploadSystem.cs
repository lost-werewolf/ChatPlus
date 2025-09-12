using System.Collections.Generic;
using ChatPlus.Core.Helpers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace ChatPlus.Core.Features.Uploads
{
    [Autoload(Side = ModSide.Client)]
    public class UploadSystem : ModSystem
    {
        public UserInterface ui;
        public UploadState state;

        public override void Load()
        {
            ui = new UserInterface();
            state = new UploadState();
            ui.SetState(null);
        }

        public override void Unload()
        {
            ui = new UserInterface();
            state = new UploadState();
            ui.SetState(null);
        }

        public override void UpdateUI(GameTime gameTime)
        {
            ChatPlus.StateManager.OpenStateByTriggers(
                gameTime,
                ui,
                state,
                ChatTriggers.UnclosedTag("[u"),
                ChatTriggers.CurrentWordStartsWith('#')
            );
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int index = layers.FindIndex(l => l.Name.Equals("Vanilla: Death Text"));
            if (index == -1) return;

            layers.Insert(index, new LegacyGameInterfaceLayer(
                "ChatPlus: Uploads Panel",
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
