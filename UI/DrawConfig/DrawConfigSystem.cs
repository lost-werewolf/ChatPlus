using System.Collections.Generic;
using AdvancedChatFeatures.Common.Configs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace AdvancedChatFeatures.UI.DrawConfig
{
    [Autoload(Side = ModSide.Client)]
    public class DrawConfigSystem : ModSystem
    {
        public UserInterface ui;
        public DrawConfigState drawConfigState;

        public override void OnWorldLoad()
        {
            ui = new();
            drawConfigState = new();
            ui.SetState(drawConfigState);
        }

        public override void OnWorldUnload()
        {
            // Cleanup
            ui?.SetState(null);
            ui = null;
            drawConfigState = null;
        }

        public override void UpdateUI(GameTime gameTime)
        {
            // Only update when chat is open and the setting is enabled
            if (!Conf.C.features.ConfigIcon) return;
            if (ui?.CurrentState == null) return;
            if (!Main.drawingPlayerChat) return;

            ui.Update(gameTime);
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int index = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (index != -1)
            {
                layers.Insert(index, new LegacyGameInterfaceLayer(
                    "AdvancedChatFeatures: DrawConfigSystem",
                    () =>
                    {
                        if (ui?.CurrentState != null &&
                            Common.Configs.Conf.C.features.ConfigIcon &&
                            Main.drawingPlayerChat)
                        {
                            ui.Draw(Main.spriteBatch, new GameTime());
                        }
                        return true;
                    },
                    InterfaceScaleType.UI));
            }
        }
    }
}
