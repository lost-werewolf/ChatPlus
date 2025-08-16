using System.Collections.Generic;
using AdvancedChatFeatures.Helpers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace AdvancedChatFeatures.Glyphs
{
    [Autoload(Side = ModSide.Client)]
    public class GlyphSystem : ModSystem
    {
        public UserInterface ui;
        public GlyphState glyphState;

        public override void Load()
        {
            ui = new UserInterface();
            glyphState = new GlyphState();
            ui.SetState(null);
        }

        public override void Unload()
        {
            ui = new UserInterface();
            glyphState = new GlyphState();
            ui.SetState(null);
        }

        public override void UpdateUI(GameTime gameTime)
        {
            StateHelper.ToggleForPrefixExclusive(ui, glyphState, gameTime, "[g");
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int index = layers.FindIndex(l => l.Name.Equals("Vanilla: Death Text"));
            if (index == -1) return;

            layers.Insert(index, new LegacyGameInterfaceLayer(
                "AdvancedChatFeatures: GlyphSystem",
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
