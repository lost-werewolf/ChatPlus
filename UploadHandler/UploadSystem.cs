using System.Collections.Generic;
using AdvancedChatFeatures.Helpers;
using AdvancedChatFeatures.ItemHandler;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace AdvancedChatFeatures.UploadHandler
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
            StateHandler.OpenStateIfPrefixMatches(gameTime, ui, state, "[u");
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int index = layers.FindIndex(l => l.Name.Equals("Vanilla: Death Text"));
            if (index == -1) return;

            layers.Insert(index, new LegacyGameInterfaceLayer(
                "AdvancedChatFeatures: Uploads Panel",
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
