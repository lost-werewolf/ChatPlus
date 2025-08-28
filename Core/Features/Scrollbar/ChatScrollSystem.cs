using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace ChatPlus.Core.Features.Scrollbar;

public class ChatScrollSystem : ModSystem
{
    private UserInterface _chatUI;
    public ChatScrollState state;

    public override void PostSetupContent()
    {
        // Initialize the user interface for the chat scrollbar, but keep it hidden initially
        _chatUI = new UserInterface();
        state = new ChatScrollState();
        _chatUI.SetState(null);
    }

    public override void UpdateUI(GameTime gameTime)
    {
        // Toggle the custom chat UI when the vanilla chat opens or closes
        if (Main.drawingPlayerChat)
        {
            if (_chatUI.CurrentState != state)
            {
                // Chat just opened: activate our scroll UI
                _chatUI.SetState(state);
                Main.chatMonitor.ResetOffset();    // reset vanilla chat offset to show latest messages
                state.chatScrollbar.GoToBottom();  // initialize scrollbar thumb at bottom
            }
            // Update our UI state (scrollbar & list)
            _chatUI.Update(gameTime);
        }
        else
        {
            // Chat closed: remove our UI state
            if (_chatUI.CurrentState != null)
            {
                _chatUI.SetState(null);
            }
        }
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        // Draw our chat UI just before the vanilla chat text draws (just above "Death Text" layer)
        int index = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Death Text"));
        if (index != -1)
        {
            layers.Insert(index, new LegacyGameInterfaceLayer(
                "ChatPlus: Chat Scrollbar",
                delegate {
                    if (Main.drawingPlayerChat && _chatUI?.CurrentState != null)
                    {
                        _chatUI.Draw(Main.spriteBatch, Main.gameTimeCache);
                    }
                    return true;
                },
                InterfaceScaleType.UI)
            );
        }
    }
}
