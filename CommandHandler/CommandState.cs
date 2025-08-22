using ChatPlus.UI;
using Terraria.UI;

namespace ChatPlus.CommandHandler
{
    public class CommandState : BaseState<Command>
    {
        public CommandPanel commandPanel;
        public DescriptionPanel<Command> commandDescriptionPanel;
        public CommandState()
        {
            // Initialize the UI elements
            commandPanel = new();
            Append(commandPanel);

            commandDescriptionPanel = new();
            Append(commandDescriptionPanel);

            commandPanel.ConnectedPanel = commandDescriptionPanel;
            commandDescriptionPanel.ConnectedPanel = commandPanel;
        }
    }
}