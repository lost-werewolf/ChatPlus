using System;
using System.Collections.Generic;
using AdvancedChatFeatures.Common.Hooks;
using AdvancedChatFeatures.Emojis;
using AdvancedChatFeatures.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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

        protected override string GetFullTag(Command data)
            => data.Name;


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

            AutocompleteCommandOnTab();
        }

        private void AutocompleteCommandOnTab()
        {
            if (!JustPressed(Keys.Tab))
                return;

            if (items.Count == 0)
                return;

            var current = items[currentIndex];
            if (current == null) return;

            string insert = GetFullTag(current.Data);
            Main.chatText = insert;

            HandleChatHook.SetCaretPos(Main.chatText.Length);
        }

        protected override string ExtractQuery(string text)
        {
            text = text ?? string.Empty;
            if (!text.StartsWith("/")) return string.Empty;

            int space = text.IndexOf(' ');
            string cmd = (space > 1 ? text.Substring(1, space - 1) : text.Substring(1)).Trim();
            return cmd;
        }

        protected override bool MatchesFilter(Command data, string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return true;
            string name = (data.Name ?? string.Empty).TrimStart('/');
            return name.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}