using AdvancedChatFeatures.UI.Commands.Elements;
using Terraria.UI;

namespace AdvancedChatFeatures.UI.Commands
{
    public class CommandsListState : UIState
    {
        public CommandsListState()
        {
            // Initialize the UI elements
            CommandsPanel commandsList = new();
            Append(commandsList);
        }
    }
}