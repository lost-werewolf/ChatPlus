using System.Collections.Generic;
using AdvancedChatFeatures.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AdvancedChatFeatures.Commands
{
    public class CommandPanel : NavigationPanel<Command>
    {
        // Properties
        protected override IEnumerable<Command> GetSource() => CommandInitializer.Commands;
        protected override NavigationElement<Command> BuildElement(Command data) => new CommandElement(data);
        protected override string GetDescription(Command data) => data.Description;
        protected override string GetTag(Command data) => data.Name;

        public override void Update(GameTime gt)
        {
            if (items.Count == 0)
                PopulatePanel();

            base.Update(gt);
        }
    }
}