using System.Collections.Generic;
using AdvancedChatFeatures.Helpers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace AdvancedChatFeatures.ItemWindow
{
    [Autoload(Side = ModSide.Client)]
    public class ItemWindowSystem : ModSystem
    {
        public UserInterface ui;
        public ItemWindowState itemWindowState;

        public override void Load()
        {
            ui = new UserInterface();
            itemWindowState = new ItemWindowState();
            ui.SetState(null);
        }

        public override void Unload()
        {
            ui = new UserInterface();
            itemWindowState = new ItemWindowState();
            ui.SetState(null);
        }

        public override void UpdateUI(GameTime gameTime)
        {
            StateHelper.ToggleForPrefixExclusive(ui, itemWindowState, gameTime, "[i");
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int index = layers.FindIndex(l => l.Name.Equals("Vanilla: Death Text"));
            if (index == -1) return;

            layers.Insert(index, new LegacyGameInterfaceLayer(
                "AdvancedChatFeatures: ItemWindowSystem",
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
