using System.Collections.Generic;
using ChatPlus.Core.Helpers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace ChatPlus.Core.Features.Links;

[Autoload(Side = ModSide.Client)]
public class LinkSystem : ModSystem
{
    public UserInterface ui;
    public LinkState state;

    public override void PostSetupContent()
    {
        ui = new UserInterface();
        state = new LinkState();
        ui.SetState(null);
    }

    public override void UpdateUI(GameTime gameTime)
    {
        StateManager.OpenStateIfPrefixMatches(gameTime, ui, state, "[l");
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        int index = layers.FindIndex(l => l.Name.Equals("Vanilla: Death Text"));
        if (index == -1) return;

        layers.Insert(index, new LegacyGameInterfaceLayer(
            "ChatPlus: LinkSystem",
            () =>
            {
                if (ui?.CurrentState != null)
                    ui.Draw(Main.spriteBatch, new GameTime());
                return true;
            },
            InterfaceScaleType.UI));
    }
}
