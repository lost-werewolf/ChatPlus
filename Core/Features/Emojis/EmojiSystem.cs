using System.Collections.Generic;
using ChatPlus.Core.Helpers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace ChatPlus.Core.Features.Emojis
{
    [Autoload(Side = ModSide.Client)]
    public class EmojiSystem : ModSystem
    {
        public UserInterface ui;
        public EmojiState state;

        public override void PostSetupContent()
        {
            ui = new UserInterface();
            state = new EmojiState();
            ui.SetState(null); // start hidden
        }

        public static bool OpenedFromColon { get; private set; }

        public override void UpdateUI(GameTime gameTime)
        {
            ChatPlus.StateManager.OpenStateByTriggers(
                gameTime,
                ui,
                state,
                ChatTriggers.UnclosedTag("[e"),
                ChatTriggers.CharOutsideTags(':')
            );
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int index = layers.FindIndex(l => l.Name.Equals("Vanilla: Death Text"));
            if (index == -1) return;

            layers.Insert(index, new LegacyGameInterfaceLayer(
                "ChatPlus: Emojis Panel",
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