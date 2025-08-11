using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace AdvancedChatFeatures.UI.DrawConfig
{
    [Autoload(Side = ModSide.Client)]
    public class DrawConfigSystem : ModSystem
    {
        public UserInterface userInterface;
        public DrawConfigState drawConfigState;

        public override void OnWorldLoad()
        {
            userInterface = new();
            drawConfigState = new();
            userInterface.SetState(drawConfigState);
        }

        public override void OnWorldUnload()
        {
            // Cleanup
            userInterface?.SetState(null);
            userInterface = null;
            drawConfigState = null;
        }

        public override void UpdateUI(GameTime gameTime)
        {
            // Only update when chat is open and the setting is enabled
            if (userInterface?.CurrentState == null) return;
            if (!Common.Configs.Conf.C.ConfigIcon) return;
            if (!Main.drawingPlayerChat) return;

            userInterface.Update(gameTime);
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
                        if (userInterface?.CurrentState != null &&
                            Common.Configs.Conf.C.ConfigIcon &&
                            Main.drawingPlayerChat)
                        {
                            userInterface.Draw(Main.spriteBatch, new GameTime());
                        }
                        return true;
                    },
                    InterfaceScaleType.UI));
            }
        }
    }
}
