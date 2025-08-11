using AdvancedChatFeatures.UI.Commands.Elements;
using Terraria.UI;

namespace AdvancedChatFeatures.UI.Commands
{
    public class CommandsListState : UIState
    {
        public CommandsPanel commandsPanel;

        public CommandsListState()
        {
            // Initialize the UI elements
            commandsPanel = new();
            Append(commandsPanel);
        }
    }
}