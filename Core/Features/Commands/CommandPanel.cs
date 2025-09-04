using System.Collections.Generic;
using ChatPlus.Core.Chat;
using ChatPlus.Core.UI;
using Microsoft.Xna.Framework;
using Stubble.Core.Classes;
using Terraria;

namespace ChatPlus.Core.Features.Commands
{
    public class CommandPanel : BasePanel<Command>
    {
        // Properties
        protected override IEnumerable<Command> GetSource() => CommandManager.Commands;
        protected override BaseElement<Command> BuildElement(Command data) => new CommandElement(data);
        protected override string GetDescription(Command data) => data.Description;
        protected override string GetTag(Command data) => data.Name;

        public override void InsertSelectedTag()
        {
            if (items.Count == 0 || currentIndex < 0) return;

            string tag = GetTag(items[currentIndex].Data);
            if (string.IsNullOrEmpty(tag)) return;

            // Replace chat text with command
            Main.chatText = tag;
            HandleChatSystem.SetCaretPos(Main.chatText.Length);
            return;
        }

        public override void Update(GameTime gt)
        {
            base.Update(gt);
        }
    }
}