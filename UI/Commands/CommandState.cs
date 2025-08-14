using AdvancedChatFeatures.UI.Commands.Elements;
using Microsoft.Xna.Framework;
using Terraria.GameContent;
using Terraria.UI;

namespace AdvancedChatFeatures.UI.Commands
{
    public class CommandState : UIState
    {
        public CommandPanel commandPanel;
        public TooltipPanel tooltipPanel;
        public CommandState()
        {
            // Set width based on the longest command
            //int longestWidth = 0;
            //foreach (var cmd in CommandInitializer.Commands)
            //{
            //    Vector2 size = FontAssets.MouseText.Value.MeasureString(cmd.Name) * 0.9f;
            //    if (size.X > longestWidth) longestWidth = (int)size.X;
            //}

            // Initialize the UI elements
            commandPanel = new(220);
            Append(commandPanel);

            tooltipPanel = new(220);
            Append(tooltipPanel);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}