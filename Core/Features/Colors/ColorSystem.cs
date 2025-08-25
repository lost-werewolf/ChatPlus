using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace ChatPlus.Core.Features.Colors
{
    [Autoload(Side = ModSide.Client)]
    public class ColorSystem : ModSystem
    {
        public UserInterface ui;
        public ColorState colorState;

        public override void Load()
        {
            ui = new UserInterface();
            colorState = new ColorState();
            ui.SetState(null);
        }

        public override void Unload()
        {
            ui = new UserInterface();
            colorState = new ColorState();
            ui.SetState(null);
        }

        public override void UpdateUI(GameTime gameTime)
        {
            StateHandler.OpenStateIfPrefixMatches(gameTime, ui, colorState, "[c");
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int index = layers.FindIndex(l => l.Name.Equals("Vanilla: Death Text"));
            if (index == -1) return;

            layers.Insert(index, new LegacyGameInterfaceLayer(
                "ChatPlus: Colors Panel",
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
