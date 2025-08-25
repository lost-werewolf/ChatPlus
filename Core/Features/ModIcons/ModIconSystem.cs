using System.Collections.Generic;
using ChatPlus.Core.Helpers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace ChatPlus.Core.Features.ModIcons;

[Autoload(Side = ModSide.Client)]
public class ModIconSystem : ModSystem
{
    public UserInterface ui;
    public ModIconState state;

    public override void PostSetupContent()
    {
        if (!ModLoader.TryGetMod("ChitterChatter", out Mod _))
        {
            return;
        }

        ui = new UserInterface();
        state = new ModIconState();
        ui.SetState(null);
    }

    public override void UpdateUI(GameTime gameTime)
    {
        if (!ModLoader.TryGetMod("ChitterChatter", out Mod _))
        {
            return;
        }

        StateManager.OpenStateIfPrefixMatches(gameTime, ui, state, "[mi");
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        int index = layers.FindIndex(l => l.Name.Equals("Vanilla: Death Text"));
        if (index == -1) return;

        layers.Insert(index, new LegacyGameInterfaceLayer(
            "ChatPlus: ModIconSystem",
            () =>
            {
                if (!ModLoader.TryGetMod("ChitterChatter", out Mod _))
                {
                    return true;
                }
                else
                {
                    if (ui?.CurrentState != null)
                        ui.Draw(Main.spriteBatch, new GameTime());
                }
                return true;
            },
            InterfaceScaleType.UI
        ));
    }
}
