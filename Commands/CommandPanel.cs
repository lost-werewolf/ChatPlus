using System.Collections.Generic;
using AdvancedChatFeatures.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace AdvancedChatFeatures.Commands
{
    /// <summary> Represents a command panel in the UI. </summary>
    public class CommandPanel : NavigationPanel<Command>
    {
        // Properties
        protected override IEnumerable<Command> GetSource()
            => CommandInitializer.Commands;

        protected override NavigationElement<Command> BuildElement(Command data)
            => new CommandElement(data);

        protected override string GetDescription(Command data)
            => data.Description;

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
        }

        public override void SetSelectedIndex(int index)
        {
            base.SetSelectedIndex(index);

            // Get the current command
            var current = (CommandElement)items[currentIndex];
            string baseText = Main.chatText ?? string.Empty;
            string selected = current.Command.Name ?? string.Empty;
        }

        public override void Update(GameTime gt)
        {
            if (items.Count == 0)
            PopulatePanel();

            base.Update(gt);
        }
    }
}