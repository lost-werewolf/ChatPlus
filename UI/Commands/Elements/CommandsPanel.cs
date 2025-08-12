using System.Collections.Generic;
using AdvancedChatFeatures.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ModLoader;

namespace AdvancedChatFeatures.UI.Commands.Elements
{
    /// <summary>
    /// A window of commands.
    /// Opens when user has "/" as the first character in the chat
    /// Has a mod filter which cycles through mods with commands.
    /// Has selection which navigates with arrow keys and executes on enter press.
    /// Has functionality for allowing holding arrow keys for fast navigation
    /// On click, puts the command into the text box
    /// </summary>
    public class CommandsPanel : DraggablePanel
    {
        // Elements
        public HeaderPanel header;
        public UIScrollbar scrollbar;
        private UIList list;

        // Selection handling
        private readonly List<CommandPanelElement> items = [];
        private int currentIndex = -1;

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

            // Set selected index
            if (items.Count > 0) SetSelectedIndex(0, force: true);

            Append(list);
            Append(scrollbar);
        }

        public void Repopulate(Mod modFilter = null)
        {
            items.Clear();
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

                var element = new CommandPanelElement(cmd.Command, cmd.Description, icon);
                element.OnMouseOver += (_, __) => SetSelectedIndex(items.IndexOf(element));
                list.Add(element);
                items.Add(element);
            }

            list.Recalculate();
            scrollbar?.Recalculate();

            // Set selected index
            if (items.Count > 0)
            {
                if (currentIndex < 0 || currentIndex >= items.Count) currentIndex = 0;
                SetSelectedIndex(currentIndex, force: true);
            }
            else currentIndex = -1;
        }

        private void SetSelectedIndex(int idx, bool force = false)
        {
            if (items.Count == 0)
            {
                currentIndex = -1;
                return;
            }

            // wrap
            if (idx < 0) idx = items.Count - 1;
            else if (idx >= items.Count) idx = 0;

            if (!force && idx == currentIndex) return;

            // clear old
            if (currentIndex >= 0 && currentIndex < items.Count)
                items[currentIndex].SetSelected(false);

            currentIndex = idx;
            var sel = items[currentIndex];
            sel.SetSelected(true);

            // keep in view
            list.Recalculate();
            scrollbar?.Recalculate();

            float viewTop = list.ViewPosition;
            float viewHeight = list.GetInnerDimensions().Height;
            float itemTop = sel.Top.Pixels;
            float itemHeight = sel.GetOuterDimensions().Height;
            float itemBottom = itemTop + itemHeight;

            if (itemTop < viewTop) list.ViewPosition = itemTop;
            else if (itemBottom > viewTop + viewHeight) list.ViewPosition = itemBottom - viewHeight;
        }

        private static bool JustPressed(Keys key)
            => Main.keyState.IsKeyDown(key) && Main.oldKeyState.IsKeyUp(key);

        public bool TryExecuteSelected()
        {
            if (currentIndex >= 0 && currentIndex < items.Count)
            {
                CommandsHelper.ExecuteCommand(items[currentIndex].name);
                return true;
            }
            return false;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // navigate up and down
            if (JustPressed(Keys.Up)) SetSelectedIndex(currentIndex - 1);
            if (JustPressed(Keys.Down)) SetSelectedIndex(currentIndex + 1);
        }
    }
}