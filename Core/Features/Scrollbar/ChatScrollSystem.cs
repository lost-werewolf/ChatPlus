using System.Collections.Generic;
using ChatPlus.Common.Configs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace ChatPlus.Core.Features.Scrollbar;

public class ChatScrollSystem : ModSystem
{
    private UserInterface chatScrollUI;
    public ChatScrollState chatScrollState;

    public override void PostSetupContent()
    {
        // Initialize the user interface for the chat scrollbar
        chatScrollUI = new UserInterface();
        chatScrollState = new ChatScrollState();
        chatScrollUI.SetState(null);
    }

    /// <summary>
    /// Set the state to <see cref="ChatScrollState"/> when conditions are met (chat open, config option enabled, etc)
    /// Otherwise set state to null and skip drawing.
    /// </summary>
    public override void UpdateUI(GameTime gameTime)
    {
        if (!Conf.C.Scrollbar && chatScrollUI.CurrentState != null)
            chatScrollUI.SetState(null);

        if (Main.drawingPlayerChat)
        {
            if (chatScrollUI.CurrentState != chatScrollState)
            {
                chatScrollUI.SetState(chatScrollState);
                Main.chatMonitor.ResetOffset();
                chatScrollState.chatScrollbar.GoToBottom();  
            }
            chatScrollUI.Update(gameTime);
        }
        else
        {
            if (chatScrollUI.CurrentState != null)
            {
                chatScrollUI.SetState(null);
            }
        }
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        int index = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Death Text"));
        if (index != -1)
        {
            layers.Insert(index, new LegacyGameInterfaceLayer(
                "ChatPlus: Chat Scrollbar",
                () => {
                    bool moreThan10 = ChatScrollList.GetTotalLines() > 10;
                    if (Main.drawingPlayerChat && chatScrollUI?.CurrentState != null && Conf.C.Scrollbar && moreThan10)
                    {
                        chatScrollUI.Draw(Main.spriteBatch, Main.gameTimeCache);
                    }
                    return true;
                },
                InterfaceScaleType.UI)
            );
        }
    }
}
