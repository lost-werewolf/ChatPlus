using System;
using System.Collections.Generic;
using AdvancedChatFeatures.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.Chat;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization; // for NetworkText
using Terraria.ModLoader;
using Terraria.ModLoader.Core;
using Terraria.UI;

namespace AdvancedChatFeatures.UI.Commands.Elements
{
    public class CommandsPanel : DraggablePanel
    {
        // Elements
        public UIScrollbar scrollbar;
        private UIList commandList;
        private string lastChatText = "";

        // Selection handling
        private readonly List<CommandPanelElement> items = new();
        private int selectedIndex = -1;

        // visible subset after filtering
        private readonly List<CommandPanelElement> visible = new();

        // key repeat
        private Keys repeatKey = Keys.None;
        private double repeatStartMs = 0;
        private double repeatLastMs = 0;
        private const double initialRepeatDelayMs = 300;
        private const double repeatIntervalMs = 80;

        public CommandsPanel()
        {
            Width.Set(300, 0);
            HAlign = 0.05f;
            VAlign = 0.95f;
            Height.Set(200, 0);
            MaxHeight.Set(240, 0);
            SetPadding(0);
            OverflowHidden = true;
            BackgroundColor = ColorHelper.DarkBlue * 1.0f;

            var header = new HeaderPanel("Commands");
            Append(header);

            // List + scrollbar
            commandList = new UIList
            {
                ListPadding = 4f,
                Width = { Pixels = -20f, Percent = 1f },
                Top = { Pixels = 28f },         // below header
            };

            scrollbar = new UIScrollbar
            {
                HAlign = 1f,
                Height = { Percent = 1f },
                Top = { Pixels = 28f + 5f },         // align with list
                Width = { Pixels = 20f }
            };
            commandList.SetScrollbar(scrollbar);

            // Add commands
            foreach (var cmdList in CommandLoader.Commands.Values)
                foreach (ModCommand cmd in cmdList)
                {
                    string name = cmd.Command;
                    string usage = cmd.Description;
                    string modName = cmd.Mod.DisplayNameClean;
                    Texture2D icon = null;

                    var file = cmd.Mod.File;
                    if (file != null && cmd.Mod.Name != "Terraria")
                        icon = CommandsHelper.GetModIcon(file);

                    var element = new CommandPanelElement(name, usage, icon, modName);
                    element.OnMouseOver += (_, _) => OnItemHovered(element);

                    items.Add(element);
                    commandList.Add(element);
                }

            Append(commandList);
            Append(scrollbar);

            if (items.Count > 0)
                SetSelectedIndex(0, force: true);
        }

        private void RebuildVisibleList(IEnumerable<CommandPanelElement> source)
        {
            commandList.Clear();
            visible.Clear();
            foreach (var e in source)
            {
                visible.Add(e);
                commandList.Add(e);
            }
            commandList.Recalculate();
            scrollbar?.Recalculate();
        }

        private void ApplyFilterFromChat()
        {
            string text = Main.chatText ?? string.Empty;

            if (!text.StartsWith("/"))
            {
                RebuildVisibleList(items);
                return;
            }

            string token = text.Substring(1);
            int space = token.IndexOfAny(new[] { ' ', '\t' });
            string typed = space >= 0 ? token.Substring(0, space) : token;

            if (string.IsNullOrWhiteSpace(typed))
            {
                RebuildVisibleList(items);
                return;
            }

            // Find prefix matches first (closest matches)
            var prefixMatches = items.FindAll(e =>
                e.name.StartsWith(typed, StringComparison.OrdinalIgnoreCase));

            // Find other matches that aren't already in prefixMatches
            var otherMatches = items.FindAll(e =>
                e.name.IndexOf(typed, StringComparison.OrdinalIgnoreCase) >= 0 &&
                !prefixMatches.Contains(e));

            // Merge lists: prefix matches first, then other matches
            var orderedMatches = new List<CommandPanelElement>();
            orderedMatches.AddRange(prefixMatches);
            orderedMatches.AddRange(otherMatches);

            // If no matches, show all
            if (orderedMatches.Count == 0)
                orderedMatches = new List<CommandPanelElement>(items);

            // Rebuild visible list
            RebuildVisibleList(orderedMatches);

            // Select the first match
            if (orderedMatches.Count > 0)
                SetSelectedIndex(0, force: true);
        }

        private void OnItemHovered(CommandPanelElement el)
        {
            var currentList = (visible.Count > 0 && visible.Count < items.Count) ? visible : items;
            int idx = currentList.IndexOf(el);
            if (idx != -1)
                SetSelectedIndex(idx, force: true);
        }

        private void SetSelectedIndex(int idx, bool force = false)
        {
            var currentList = (visible.Count > 0 && visible.Count < items.Count) ? visible : items;

            if (currentList.Count == 0) { selectedIndex = -1; return; }

            if (idx < 0) idx = currentList.Count - 1;
            else if (idx >= currentList.Count) idx = 0;

            if (!force && selectedIndex >= 0 && selectedIndex < currentList.Count && selectedIndex == idx)
                return;

            // Clear previous selection highlight
            if (selectedIndex >= 0)
            {
                if (selectedIndex < items.Count) items[selectedIndex].SetSelected(false);
                if (selectedIndex < visible.Count) visible[selectedIndex].SetSelected(false);
            }

            selectedIndex = idx;
            var sel = currentList[selectedIndex];
            sel.SetSelected(true);

            // Keep selected in view
            if (commandList != null && scrollbar != null)
            {
                commandList.Recalculate();
                scrollbar.Recalculate();

                float viewTop = commandList.ViewPosition;
                float viewHeight = commandList.GetInnerDimensions().Height;

                float itemTop = sel.Top.Pixels;
                float itemHeight = sel.GetOuterDimensions().Height;
                float itemBottom = itemTop + itemHeight;

                if (itemTop < viewTop)
                    commandList.ViewPosition = itemTop;
                else if (itemBottom > viewTop + viewHeight)
                    commandList.ViewPosition = itemBottom - viewHeight;
            }
        }

        private bool ShouldRepeat(Keys key, GameTime gt)
        {
            bool down = Main.keyState.IsKeyDown(key);
            bool pressed = down && Main.oldKeyState.IsKeyUp(key);

            double now = gt.TotalGameTime.TotalMilliseconds;

            if (pressed)
            {
                repeatKey = key;
                repeatStartMs = now;
                repeatLastMs = now;
                return true;
            }

            if (!down && repeatKey == key)
            {
                repeatKey = Keys.None;
                return false;
            }

            if (down && repeatKey == key)
            {
                if (now - repeatStartMs >= initialRepeatDelayMs &&
                    now - repeatLastMs >= repeatIntervalMs)
                {
                    repeatLastMs = now;
                    return true;
                }
            }

            return false;
        }

        private static bool JustPressed(Keys key)
            => Main.keyState.IsKeyDown(key) && Main.oldKeyState.IsKeyUp(key);

        public override void Update(GameTime gameTime)
        {
            // Hot reload
            OverflowHidden = false;
            scrollbar.Height.Set(-50, 1);
            scrollbar.Top.Set(28 + 10, 0);
            scrollbar.Left.Set(-6, 0);
            commandList.Left.Set(3, 0);
            commandList.Top.Set(30, 0);
            commandList.Height.Set(170, 0);
            commandList.MaxHeight.Set(200, 0);
            commandList.Width.Set(-6 - 20 - 2, 1);

            // Apply conditional filter from chat text
            ApplyFilterFromChat();

            bool chatOpen = Main.drawingPlayerChat;

            if (chatOpen)
            {
                if (Main.chatText != lastChatText)
                {
                    ApplyFilterFromChat();
                    lastChatText = Main.chatText;
                }

                if (ShouldRepeat(Keys.Down, gameTime))
                    SetSelectedIndex(selectedIndex + 1);

                if (ShouldRepeat(Keys.Up, gameTime))
                    SetSelectedIndex(selectedIndex - 1);

                if (JustPressed(Keys.Enter) && selectedIndex >= 0)
                {
                    var currentList = (visible.Count > 0 && visible.Count < items.Count) ? visible : items;
                    if (selectedIndex < currentList.Count)
                    {
                        ExecuteCommand(currentList[selectedIndex]);
                    }
                }
            }

            base.Update(gameTime);
        }

        private void ExecuteCommand(CommandPanelElement element)
        {
            string commandText = "/" + element.name;

            // Try to process via CommandLoader (client side) first.
            // This mimics how the chat system routes commands.
            var message = new ChatMessage(commandText);
            var playerCaller = new PlayerCommandCaller(Main.LocalPlayer); // CommandType.Server, still executes most ModCommands client-hosted
            if (CommandLoader.HandleCommand(message.Text, playerCaller))
            {
                Main.drawingPlayerChat = false;
                Main.chatText = string.Empty;
                return;
            }

            // Fallback: paste into chat so the user can press Enter (ensures compatibility)
            Main.drawingPlayerChat = true;
            Main.chatText = commandText + " ";
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);
        }
    }
}