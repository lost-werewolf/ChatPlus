using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;

namespace AdvancedChatFeatures.UI.Commands
{
    public class CommandState : UIState
    {
        public CommandPanel commandPanel;
        public CommandUsagePanel commandUsagePanel;
        public CommandState()
        {
            // Initialize the UI elements
            commandPanel = new();
            Append(commandPanel);

            commandUsagePanel = new();
            Append(commandUsagePanel);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}