using System;
using System.Collections.Generic;
using AdvancedChatFeatures.Common.Configs;
using AdvancedChatFeatures.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI.Chat;

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
    public class CommandPanel : DraggablePanel
    {
        // Elements
        public UIScrollbar scrollbar;
        private UIList list;
        public readonly List<CommandElement> items = [];

        // Positioning and style
        public int width;
        public int itemCount = 10;

        // Navigation
        public int currentIndex = 0; // first item

        public CommandPanel(int width)
        {
            // Size
            this.width = width;

            // Set position just above the chat
            VAlign = 1f;
            Top.Set(-38, 0);
            Left.Set(80, 0);

            // Style
            OverflowHidden = true;
            BackgroundColor = ColorHelper.DarkBlue * 1.0f;
            SetPadding(0);

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

            ResetDimensions();

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

        private string ghostText = "";

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);

            if (Conf.C.autocompleteConfig.ShowGhostText)
                DrawGhostText(sb);
        }

        private void DrawGhostText(SpriteBatch sb)
        {
            if (string.IsNullOrEmpty(ghostText)) return;

            float uiScale = Main.UIScale;

            // Chat input origin in UI coords (same X as vanilla)
            Vector2 chatOriginUi = new Vector2(80f, (Main.screenHeight / uiScale) - 32f);

            // Measure typed text and ghost
            Vector2 typedSizePx = FontAssets.MouseText.Value.MeasureString(Main.chatText ?? "");
            Vector2 ghostSizePx = FontAssets.MouseText.Value.MeasureString(ghostText);

            Vector2 typedSizeUi = typedSizePx / uiScale;
            Vector2 ghostSizeUi = ghostSizePx / uiScale;

            // Vanilla chat baseline Y offset (~6px)
            const float baselineOffset = 6f / 2f; // tweak until it matches
            Vector2 posUi = chatOriginUi + new Vector2(typedSizeUi.X, baselineOffset);

            // Background with small padding
            const int pad = 2;
            Vector2 bgSizeUi = new(
                ghostSizeUi.X + (pad * 2f / uiScale),
                ghostSizeUi.Y + (pad * 2f / uiScale) - 10
            );
            Vector2 bgPosUi = posUi - new Vector2(pad / uiScale, pad / uiScale);

            // extra offset
            bgPosUi += new Vector2(12, 0);
            posUi += new Vector2(12, 0);

            // Use non-transparent color
            DrawHelper.DrawRectangle(sb, bgSizeUi, bgPosUi, ColorHelper.Blue * 0.5f);

            // Draw ghost text
            Utils.DrawBorderString(sb, ghostText, posUi, Color.White);
        }

        public void SetSelectedIndex(int index)
        {
            if (items.Count == 0) return;

            if (index < 0) index = items.Count - 1;
            else if (index >= items.Count) index = 0;

            currentIndex = index;

            for (int i = 0; i < items.Count; i++)
                items[i].SetSelected(false);

            var current = items[currentIndex];
            current.SetSelected(true);

            // Auto-fill ONLY when navigating with Up/Down and only on the command token (no args)
            bool navigating = Main.keyState.IsKeyDown(Keys.Up) || Main.keyState.IsKeyDown(Keys.Down);
            string input = Main.chatText ?? "";
            bool isCommandToken = input.Length > 0 && input[0] == '/' && input.IndexOf(' ') < 0;

            if (navigating && isCommandToken && Conf.C.autocompleteConfig.EnableAutocomplete)
            {
                if (Conf.C.autocompleteConfig.ShowGhostText)
                    return;
                Main.chatText = current.Command.Name ?? "";
                lastChatText = Main.chatText; // keep filter from immediately re-firing on our own write
            }

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

            // update tooltip (unchanged)
            var sys = ModContent.GetInstance<CommandSystem>();
            if (sys?.commandState?.tooltipPanel != null)
            {
                string tooltip = current.Command.Usage ?? "";
                float uiScale = Main.UIScale;
                float textScale = 0.9f;
                float availableUi = sys.commandState.tooltipPanel.Width.Pixels;

                Vector2 size = FontAssets.MouseText.Value.MeasureString(tooltip);
                float widthUi = (size.X * textScale) / uiScale;

                if (widthUi > availableUi)
                {
                    int breakIndex = -1;
                    for (int i = 0; i < tooltip.Length; i++)
                    {
                        if (tooltip[i] == ' ')
                            breakIndex = i;

                        Vector2 m = FontAssets.MouseText.Value.MeasureString(tooltip.Substring(0, i + 1));
                        float mUi = (m.X * textScale) / uiScale;

                        if (mUi > availableUi)
                        {
                            if (breakIndex != -1)
                                tooltip = tooltip.Substring(0, breakIndex) + "\n" + tooltip.Substring(breakIndex + 1);
                            break;
                        }
                    }
                }

                sys.commandState.tooltipPanel.text.SetText(tooltip);
            }
        }

        private static bool JustPressed(Keys key) => Main.keyState.IsKeyDown(key) && Main.oldKeyState.IsKeyUp(key);

        // Hold
        private double repeatTimer;
        private Keys heldKey = Keys.None;

        public override void Update(GameTime gt)
        {
            base.Update(gt);

            UpdateFilter();
            UpdateArrowKeysPressed(gt);

                UpdateTabKeyPressed();
        }

        private string lastChatText = "";

        private void UpdateTabKeyPressed()
        {
            if (JustPressed(Keys.Tab))
            {
                if (Conf.C.autocompleteConfig.ShowGhostText)
                {
                    Main.chatText += ghostText;
                }
                else
                {
                    if (items.Count > 0 && currentIndex >= 0 && currentIndex <= items.Count)
                    {
                        Main.chatText = items[currentIndex].Command.Name;
                    }
                }
            }
        }

        private HashSet<Keys> IgnoredKeys = [];

        private void UpdateFilter()
        {
            string current = Main.chatText ?? "";

            IgnoredKeys =
                [
                Keys.Up, Keys.Down, Keys.Left, Keys.Right, Keys.Enter, Keys.Tab, Keys.LeftShift, Keys.D7, Keys.Back
                ];


            foreach (Keys ignoredKey in IgnoredKeys)
            {
                if (JustPressed(ignoredKey))
                {
                     return;
                }
                else if (!string.Equals(current, lastChatText, StringComparison.Ordinal))
                {
                    lastChatText = current;
                    FilterCommands(current);
                }
            }
        }

        public void FilterCommands(string text)
        {
            string typed = text.Length > 0 && text[0] == '/' ? text.Substring(1) : text;

            items.Clear();
            list.Clear();

            foreach (var cmd in CommandInitializer.Commands)
            {
                string nameNoSlash = cmd.Name.StartsWith('/') ? cmd.Name.Substring(1) : cmd.Name;
                if (nameNoSlash.StartsWith(typed, StringComparison.OrdinalIgnoreCase))
                {
                    CommandElement element = new(cmd);
                    items.Add(element);
                    list.Add(element);
                }
            }

            SetSelectedIndex(0);

            // Update ghost text only if enabled
            if (Conf.C.autocompleteConfig.ShowGhostText)
            {
                if (items.Count > 0 && !string.IsNullOrEmpty(Main.chatText))
                {
                    var selected = items[currentIndex].Command.Name;
                    if (selected.StartsWith(Main.chatText, StringComparison.OrdinalIgnoreCase))
                        ghostText = selected.Substring(Main.chatText.Length);
                    else
                        ghostText = string.Empty;
                }
                else
                {
                    ghostText = string.Empty;
                }
            }
            else
            {
                ghostText = string.Empty;
            }
        }

        private void UpdateArrowKeysPressed(GameTime gt)
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

        public void ResetDimensions()
        {
            // Size
            Width.Set(220, 0);
            itemCount = Conf.C == null ? 10 : Conf.C.autocompleteConfig.CommandsVisible;
            Height.Set(30 * itemCount, 0);
            list.Height.Set(30 * itemCount, 0);
        }
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