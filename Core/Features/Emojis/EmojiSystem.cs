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
        public EmojiState emojiState;

        public object StateHandler { get; private set; }

        public override void OnModLoad()
        {
            base.OnModLoad();
        }

        public override void Load()
        {
            ui = new UserInterface();
            emojiState = new EmojiState();
            ui.SetState(null); // start hidden
        }

        public override void Unload()
        {
            ui = new UserInterface();
            emojiState = new EmojiState();
            ui.SetState(null); // start hidden
        }

        public override void OnWorldUnload()
        {
            base.OnWorldUnload();
        }

        public override void UpdateUI(GameTime gameTime)
        {
            StateManager.OpenStateIfPrefixMatches(gameTime, ui, emojiState, "[e");
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