using System.Collections.Generic;
using AdvancedChatFeatures.Helpers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace AdvancedChatFeatures.ColorWindow
{
    [Autoload(Side = ModSide.Client)]
    public class ColorWindowSystem : ModSystem
    {
        public UserInterface ui;
        public ColorWindowState colorWindowState;

        public override void Load()
        {
            ui = new UserInterface();
            colorWindowState = new ColorWindowState();
            ui.SetState(null);
        }

        public override void Unload()
        {
            ui = new UserInterface();
            colorWindowState = new ColorWindowState();
            ui.SetState(null);
        }

        public override void UpdateUI(GameTime gameTime)
        {
            StateHelper.ToggleForPrefixExclusive(ui, colorWindowState, gameTime, "[c");
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int index = layers.FindIndex(l => l.Name.Equals("Vanilla: Death Text"));
            if (index == -1) return;

            layers.Insert(index, new LegacyGameInterfaceLayer(
                "AdvancedChatFeatures: Colors Panel",
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
