using System.Collections.Generic;
using ChatPlus.Core.Helpers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace ChatPlus.Core.Features.Mentions;

[Autoload(Side = ModSide.Client)]
public class MentionSystem : ModSystem
{
    public UserInterface ui;
    public MentionState state;

    public override void Load()
    {
        ui = new UserInterface();
        state = new MentionState();
        ui.SetState(null);
        Terraria.UI.Chat.ChatManager.Register<MentionTagHandler>("mention");
    }

    public override void Unload()
    {
        ui = null;
        state = null;
    }

    public override void UpdateUI(GameTime gameTime)
    {
        // Open while user is typing an '@' mention that is not yet followed by a space
        if (!Main.drawingPlayerChat)
        {
            if (ui?.CurrentState != null) ui.SetState(null);
            return;
        }
        string text = Main.chatText ?? string.Empty;
        int at = text.LastIndexOf('@');
        if (at == -1)
        {
            if (ui?.CurrentState != null) ui.SetState(null);
            return;
        }
        // if there is a space after the @ in the current word, close
        int space = text.IndexOf(' ', at + 1);
        if (space != -1)
        {
            if (ui?.CurrentState != null) ui.SetState(null);
            return;
        }

        if (ui.CurrentState != state)
        {
            StateManager.CloseOthers(ui);
            ui.SetState(state);
            ui.CurrentState?.Recalculate();
        }
        ui.Update(gameTime);
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        int index = layers.FindIndex(l => l.Name.Equals("Vanilla: Death Text"));
        if (index == -1) index = layers.Count;
        layers.Insert(index, new LegacyGameInterfaceLayer(
            "ChatPlus: Mentions",
            () =>
            {
                if (ui?.CurrentState != null)
                    ui.Draw(Main.spriteBatch, new GameTime());
                return true;
            }, InterfaceScaleType.UI));
    }
}