using System.Collections.Generic;
using AdvancedChatFeatures.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;

namespace AdvancedChatFeatures.UI.Commands.Elements
{
    public class CommandsPanel : DraggablePanel
    {
        // Elements
        public HeaderPanel header;
        public UIScrollbar scrollbar;
        private UIList list;

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
            list = new UIList
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
            list.SetScrollbar(scrollbar);

            // Add command elements initially in the constructor
            Repopulate(modFilter: null);

            Append(list);
            Append(scrollbar);
        }

        public void Repopulate(Mod modFilter = null)
        {
            list.Clear();

            foreach (ModCommand cmd in CommandsHelper.GetAllCommands())
            {
                if (modFilter != null && cmd.Mod != modFilter)
                    continue;

                Texture2D icon = Ass.VanillaIcon.Value;
                try
                {
                    var file = cmd.Mod?.File;               // can be null!
                    if (file != null && cmd.Mod.Name != "Terraria")
                        icon = CommandsHelper.GetModIcon(file).Value ?? Ass.VanillaIcon.Value;
                }
                catch
                {
                    // swallow & use fallback icon 
                    icon = Ass.VanillaIcon.Value;
                }

                var element = new CommandPanelElement(cmd.Command, cmd.Description, icon, cmd.Mod.DisplayNameClean);
                list.Add(element);
            }

            list.Recalculate();
            scrollbar?.Recalculate();
        }

        public override void Update(GameTime gameTime)
        {
            // Hot reload
            //list.Height.Set(165, 0);
            OverflowHidden = false;

            base.Update(gameTime);
        }
    }
}