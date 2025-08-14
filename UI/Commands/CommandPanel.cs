using System;
using System.Collections.Generic;
using AdvancedChatFeatures.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;

namespace AdvancedChatFeatures.UI.Commands
{
    /// <summary>
    /// A window of commands.
    /// Opens when user has "/" as the first character in the chat
    /// Has a mod filter which cycles through mods with commands.
    /// Has selection which navigates with arrow keys and executes on enter press.
    /// Has functionality for allowing holding arrow keys for fast navigation
    /// On click, puts the command into the text box
    /// </summary>
    public class CommandPanel : UIPanel
    {
        // Elements
        private UIScrollbar scrollbar;
        private UIList list;
        public readonly List<CommandElement> items = [];
        public int itemCount = 10;

        // Navigation
        public int currentIndex = 0; // first item

        public CommandPanel(int width)
        {
            Width.Set(width, 0);

            // Set position just above the chat
            VAlign = 1f;
            Top.Set(-38, 0);
            Left.Set(80, 0);
            Height.Set(30*itemCount, 0);
            SetPadding(0);

            // Set style
            OverflowHidden = true;
            BackgroundColor = ColorHelper.DarkBlue * 0.8f;

            // Initialize elements
            list = new UIList
            {
                ListPadding = 0f,
                Width = { Pixels = -20f, Percent = 1f },
                Top = { Pixels = 3f },
                Left = { Pixels = 3f },
                Height = { Pixels = 30*itemCount},
                ManualSortMethod = _ => { },
            };

            scrollbar = new UIScrollbar
            {
                HAlign = 1f,
                Height = { Pixels = -14f, Percent = 1f },
                Top = { Pixels = 7f },
                Width = { Pixels = 20f },
                Left = { Pixels = 0f },
            };
            list.SetScrollbar(scrollbar);

            // Add initial commands to the list
            PopulateCommandPanel(selectFirst: false);

            Append(list);
            Append(scrollbar);
        }

        /// <summary>
        /// Is called whenever the command panel needs to be repopulated.
        /// Notable every time the command panel is opened, and
        /// every time the chat text changes
        /// </summary>
        /// <param name="modFilter"></param>
        public void PopulateCommandPanel(bool selectFirst)
        {
            // Clear all items
            items.Clear();
            list.Clear();
            
            // Add all items
            foreach (Command cmd in CommandInitializer.Commands)
            {
                CommandElement element = new(cmd);
                items.Add(element);
                list.Add(element);
            }

            // Reset everything
            if (selectFirst)
                SetSelectedIndex(0);
        }

        public void SetSelectedIndex(int index)
        {
            if (items.Count == 0) return;

            // wrap
            if (index < 0) index = items.Count - 1;
            else if (index >= items.Count) index = 0;

            currentIndex = index;

            // selection flags
            for (int i = 0; i < items.Count; i++)
                items[i].SetSelected(false);

            var current = items[currentIndex];
            current.SetSelected(true);
            Main.chatText = current.Command.Name;

            // update view position
            float view = list.ViewPosition;                   
            int topIndex = (int)(view / 30);
            int bottomIndex = topIndex + itemCount - 1;

            if (currentIndex < topIndex)
                view = currentIndex * 30;                    
            else if (currentIndex > bottomIndex)
                view = (currentIndex - itemCount + 1) * 30;    

            float max = Math.Max(0f, items.Count * 30 - itemCount * 30);
            if (view < 0) view = 0;
            if (view > max) view = max;

            list.ViewPosition = view;

            // update tooltip
            var sys = ModContent.GetInstance<CommandSystem>();
            string tooltip = current.Command.Usage;
            Vector2 size = FontAssets.MouseText.Value.MeasureString(tooltip);
            float width = size.X;
            if (width > Width.Pixels)
            {
                int breakIndex = -1;
                for (int i = 0; i < tooltip.Length; i++)
                {
                    if (tooltip[i] == ' ')
                        breakIndex = i;

                    if (FontAssets.MouseText.Value.MeasureString(tooltip.Substring(0, i + 1)).X > Width.Pixels)
                    {
                        if (breakIndex != -1)
                            tooltip = tooltip.Substring(0, breakIndex) + "\n" + tooltip.Substring(breakIndex + 1);
                        break;
                    }
                }
            }
            if (sys?.commandState?.tooltipPanel != null)
                sys.commandState.tooltipPanel.text.SetText(tooltip);
        }

        private static bool JustPressed(Keys key) => Main.keyState.IsKeyDown(key) && Main.oldKeyState.IsKeyUp(key);

        // Hold
        private double repeatTimer;
        private Keys heldKey = Keys.None;

        public override void Update(GameTime gt)
        {
            base.Update(gt);

            UpdateFilter();
            UpdateArrowKeys(gt);
        }

        private string lastChatText = "";

        private void UpdateFilter()
        {
            string current = Main.chatText ?? "";

            foreach (Keys key in Enum.GetValues(typeof(Keys)))
            {
                if (Main.keyState.IsKeyDown(key) && Main.oldKeyState.IsKeyUp(key))
                {
                    // Ignore navigation/control keys
                    if (key == Keys.Up || key == Keys.Down ||
                        key == Keys.Left || key == Keys.Right ||
                        key == Keys.Enter || key == Keys.Tab || key == Keys.LeftShift || key == Keys.D7 || key == Keys.Back)
                        return;

                    if (!string.Equals(current, lastChatText, StringComparison.Ordinal))
                    {
                        lastChatText = current;
                        FilterCommands(current);
                    }
                }
            }
        }

        public void FilterCommands(string text)
        {
            Main.NewText("filter" + text);
            string typed = text.Substring(1);

            items.Clear();
            list.Clear();

            foreach (var cmd in CommandInitializer.Commands)
            {
                string nameNoSlash = cmd.Name.StartsWith("/") ? cmd.Name.Substring(1) : cmd.Name;
                if (nameNoSlash.StartsWith(typed, StringComparison.OrdinalIgnoreCase))
                {
                    CommandElement element = new(cmd);
                    items.Add(element);
                    list.Add(element);
                }
            }

            SetSelectedIndex(0);
        }

        private void UpdateArrowKeys(GameTime gt)
        {
            // Tap
            if (JustPressed(Keys.Up))
            {
                SetSelectedIndex(currentIndex - 1);
                heldKey = Keys.Up;
                repeatTimer = 0.35;
            }
            else if (JustPressed(Keys.Down))
            {
                SetSelectedIndex(currentIndex + 1);
                heldKey = Keys.Down;
                repeatTimer = 0.35;
            }

            // Hold
            double dt = gt.ElapsedGameTime.TotalSeconds;
            if (Main.keyState.IsKeyDown(heldKey))
            {
                repeatTimer -= dt;
                if (repeatTimer <= 0)
                {
                    repeatTimer += 0.06; // repeat speed
                    if (Main.keyState.IsKeyDown(Keys.Up)) SetSelectedIndex(currentIndex - 1);
                    else if (Main.keyState.IsKeyDown(Keys.Down)) SetSelectedIndex(currentIndex + 1);
                }
            }
        }

        #region extra sizing

        public void UpdateItemCount(int itemsVisible)
        {
            itemCount = itemsVisible;
            Height.Set(0, 0);
            Height.Set(itemsVisible * 30,0);
            list.Height.Set(itemsVisible * 30, 0);
        }

        #endregion
    }
}