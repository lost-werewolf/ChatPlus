using System.Collections.Generic;
using ChatPlus.Core.Helpers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace ChatPlus.Core.Features.PlayerHeads;

[Autoload(Side = ModSide.Client)]
public class PlayerHeadSystem : ModSystem
{
    public UserInterface ui;
    public PlayerHeadState state;

    public override void Load()
    {
        ui = new UserInterface();
        state = new PlayerHeadState();
        ui.SetState(null);
    }

    public override void Unload()
    {
        ui = new UserInterface();
        state = new PlayerHeadState();
        ui.SetState(null);
    }

    public override void UpdateUI(GameTime gameTime)
    {
        // Open while user is typing an unclosed [p: tag
        StateManager.OpenStateIfPrefixMatches(gameTime, ui, state, "[p");
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        int index = layers.FindIndex(l => l.Name.Equals("Vanilla: Mouse Text"));
        if (index == -1)
            index = layers.Count;

        layers.Insert(index, new LegacyGameInterfaceLayer(
            "ChatPlus: PlayerIcons",
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
