using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace ChatPlus.Core.Features.Emojis.EmojiButton;
public class EmojiButtonSystem : ModSystem
{
    private UserInterface ui;
    private EmojiButton button;

    public override void Load()
    {
        ui = new UserInterface();
        button = new EmojiButton();
        ui.SetState(new UIState());
        ui.CurrentState.Append(button);
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        int index = layers.FindIndex(l => l.Name.Equals("Vanilla: Death Text"));
        if (index == -1) return;

        layers.Insert(index, new LegacyGameInterfaceLayer(
            "ChatPlus: Emoji Button",
            () =>
            {
                if (Main.drawingPlayerChat) // only when chat is open
                {
                    ui.Update(Main._drawInterfaceGameTime);
                    ui.Draw(Main.spriteBatch, Main._drawInterfaceGameTime);
                }
                return true;
            },
            InterfaceScaleType.UI
        ));
    }
}

