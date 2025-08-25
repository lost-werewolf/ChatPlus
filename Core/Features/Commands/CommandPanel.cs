using System.Collections.Generic;
using ChatPlus.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChatPlus.CommandHandler
{
    public class CommandPanel : BasePanel<Command>
    {
        // Properties
        protected override IEnumerable<Command> GetSource() => CommandInitializer.Commands;
        protected override BaseElement<Command> BuildElement(Command data) => new CommandElement(data);
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