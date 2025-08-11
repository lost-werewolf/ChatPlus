using System.Collections.Generic;
using AdvancedChatFeatures.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;

namespace AdvancedChatFeatures.UI.Commands.Elements
{
    public class CommandsPanel : DraggablePanel
    {
        // Elements
        public HeaderPanel header;
        public UIScrollbar scrollbar;
        private UIList commandList;

        // Selection handling
        private readonly List<CommandPanelElement> items = [];
        private int currentIndex = 0;

        public CommandsPanel()
        {
            Width.Set(300, 0);
            HAlign = 0.05f;
            VAlign = 0.95f;
            Height.Set(200, 0);
            SetPadding(0);
            OverflowHidden = false;
            BackgroundColor = ColorHelper.DarkBlue * 1.0f;

            header = new("Commands");
            Append(header);

            // List + scrollbar
            commandList = new UIList
            {
                ListPadding = 0f,
                Width = { Pixels = -28f, Percent = 1f },
                Top = { Pixels = 30 }, // below header
                Left = { Pixels = 3f },
                Height = { Pixels = 165 },
            };

            scrollbar = new UIScrollbar
            {
                HAlign = 1f,
                Height = { Percent = 1f, Pixels = -50 },
                Top = { Pixels = 28f + 10f },         // align with list
                Width = { Pixels = 20f },
                Left = { Pixels = -6f },
            };
            commandList.SetScrollbar(scrollbar);

            // Add command elements initially in the constructor
            foreach (ModCommand cmd in CommandsHelper.GetAllCommands())
            {
                Texture2D icon = CommandsHelper.GetModIcon(cmd.Mod.File);

                var element = new CommandPanelElement(cmd.Command, cmd.Description, icon, cmd.Mod.DisplayNameClean);
                //element.OnMouseOver += (_, _) => OnItemHovered(element);

                items.Add(element);
                commandList.Add(element);
            }

            Append(commandList);
            Append(scrollbar);
        }

        public void Repopulate()
        {
            commandList.Clear();

            // Add command elements
            foreach (ModCommand cmd in CommandsHelper.GetAllCommands())
            {
                Texture2D icon = CommandsHelper.GetModIcon(cmd.Mod.File);

                var element = new CommandPanelElement(cmd.Command, cmd.Description, icon, cmd.Mod.DisplayNameClean);
                //element.OnMouseOver += (_, _) => OnItemHovered(element);

                items.Add(element);
                commandList.Add(element);
            }
        }

        public override void Update(GameTime gameTime)
        {
            // Hot reload
            //commandList.Height.Set(165, 0);
            OverflowHidden = false;

            base.Update(gameTime);
        }
    }
}