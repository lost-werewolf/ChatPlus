using AdvancedChatFeatures.UI;
using Terraria.UI;

namespace AdvancedChatFeatures.Commands
{
    public class CommandState : UIState
    {
        public CommandPanel commandPanel;
        public DescriptionPanel<Command> commandDescriptionPanel;
        public CommandState()
        {
            // Initialize the UI elements
            commandPanel = new();
            Append(commandPanel);

            commandDescriptionPanel = new(owner: commandPanel, "List of commands");
            Append(commandDescriptionPanel);
        }
    }
}