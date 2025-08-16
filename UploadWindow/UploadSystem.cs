using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace AdvancedChatFeatures.UploadWindow
{
    [Autoload(Side = ModSide.Client)]
    public class UploadSystem : ModSystem
    {
        public UserInterface ui;
        public UploadState state;

        public override void Load()
        {
            ui = new UserInterface();
            state = new UploadState();
            ui.SetState(null);
        }

        public override void Unload()
        {
            ui = new UserInterface();
            state = new UploadState();
            ui.SetState(null);
        }

        public override void UpdateUI(GameTime gameTime)
        {
            string text = Main.chatText ?? string.Empty;
            bool startsWithUpload = text.StartsWith("[u", System.StringComparison.OrdinalIgnoreCase);

            if (startsWithUpload && Main.drawingPlayerChat)
            {
                if (ui.CurrentState != state)
                {
                    ui.SetState(state);
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
                "AdvancedChatFeatures: Uploads Panel",
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
