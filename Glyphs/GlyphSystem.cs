using System.Collections.Generic;
using AdvancedChatFeatures.Common.Configs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace AdvancedChatFeatures.UI.Glyphs
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
            string text = Main.chatText ?? string.Empty;
            bool startsWithGlyph = text.StartsWith("[g", System.StringComparison.OrdinalIgnoreCase);

            if (startsWithGlyph && Main.drawingPlayerChat)
            {
                if (ui.CurrentState != glyphState)
                {
                    ui.SetState(glyphState);
                }
                ui.Update(gameTime);
            }
            else
            {
                if (ui.CurrentState != null)
                    ui.SetState(null);
            }
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int index = layers.FindIndex(l => l.Name.Equals("Vanilla: Death Text"));
            if (index == -1) return;

            layers.Insert(index, new LegacyGameInterfaceLayer(
                "AdvancedChatFeatures: Glyphs Panel",
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
