using System;
using System.Collections.Generic;
using System.Linq;
using AdvancedChatFeatures.ColorWindow;
using AdvancedChatFeatures.Commands;
using AdvancedChatFeatures.Common.Configs;
using AdvancedChatFeatures.Common.Hooks;
using AdvancedChatFeatures.Emojis;
using AdvancedChatFeatures.Glyphs;
using AdvancedChatFeatures.Helpers;
using AdvancedChatFeatures.ItemWindow;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace AdvancedChatFeatures.UI
{
    /// <summary>
    /// A panel that can be navigated on with arrow keys
    /// </summary>
    public abstract class NavigationPanel<TData> : DraggablePanel
    {
        // Elements
        public readonly UIScrollbar scrollbar;
        protected UIList list;
        protected readonly List<NavigationElement<TData>> items = [];

        // Force populate
        protected abstract IEnumerable<TData> GetSource(); // The source of data to populate the panel with
        protected abstract NavigationElement<TData> BuildElement(TData data); // The method to create a new element from the data
        protected abstract string GetDescription(TData data);
        protected abstract string GetFullTag(TData data);

        // Navigation
        protected int currentIndex = 0; // first item

        // Holding keys
        private double repeatTimer;
        private Keys heldKey = Keys.None;

        public NavigationPanel()
        {
            // Set width
            Width.Set(320, 0);

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
                ManualSortMethod = _ => { },
            };

            scrollbar = new UIScrollbar
            {
                HAlign = 1f,
                Height = { Pixels = -14f, Percent = 1f },
                Top = { Pixels = 7f },
                Width = { Pixels = 20f },
            };
            list.SetScrollbar(scrollbar);

            Append(list);
            Append(scrollbar);

            // Populate the panel with items
            PopulatePanel();
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);

            // Which element was clicked?
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].IsMouseHovering)
                {
                    SetSelectedIndex(i); // updates currentIndex + selection + description panel
                    return;
                }
            }

            // If none matched, clear all selection
            foreach (var e in items)
                e.SetSelected(false);

            currentIndex = -1;
        }

        protected void PopulatePanel()
        {
            items.Clear();
            list.Clear();

            var source = GetSource();
            List<UIElement> fastList = new();

            string query = BuildFilterQuery(Main.chatText ?? string.Empty);

            if (source != null)
            {
                foreach (TData data in source)
                {
                    // If query is empty → show all
                    if (!string.IsNullOrWhiteSpace(query))
                    {
                        string desc = GetDescription(data) ?? string.Empty;
                        string tag = GetFullTag(data) ?? string.Empty;

                        // Filter emojis
                        IEnumerable<string> synonyms = (data is Emoji e) ? e.Synonyms : Array.Empty<string>();

                        // Filter items
                        string name = (data is ItemWindow.Item item) ? item.DisplayName ?? "" : "";

                        bool match = desc.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                            tag.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                            name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                            synonyms.Any(s => s.Contains(query, StringComparison.OrdinalIgnoreCase));
                        if (!match)
                            continue;
                    }

                    var element = BuildElement(data);
                    if (element == null)
                        continue;

                    items.Add(element);
                    fastList.Add(element);
                }

                if (fastList.Count > 0)
                    list.AddRange(fastList);
            }

            // Reset selection
            if (items.Count > 0)
                SetSelectedIndex(0);
            else
                currentIndex = -1;
        }

        private static string BuildFilterQuery(string text)
        {
            text = text.Trim();

            if (text.StartsWith("/"))
            {
                int space = text.IndexOf(' ');
                return (space > 1 ? text.Substring(1, space - 1) : text.Substring(1)).Trim();
            }

            int lb = text.LastIndexOf('[');
            if (lb >= 0)
            {
                string inside = text.Substring(lb + 1);
                int colon = inside.IndexOf(':');
                if (colon >= 0)
                    return inside.Substring(colon + 1).Trim();

                return inside.Trim();
            }

            return text;
        }

        public void SetHeight()
        {
            // Set height
            Height.Set(Conf.C.featureStyleConfig.ItemsPerWindow * 30, 0);
            list.Height.Set(Conf.C.featureStyleConfig.ItemsPerWindow * 30, 0);
        }

        public virtual void SetSelectedIndex(int index)
        {
            if (items.Count == 0) return;

            // wrap around
            if (index < 0) index = items.Count - 1;
            else if (index >= items.Count) index = 0;

            // deselect all
            for (int i = 0; i < items.Count; i++)
                items[i].SetSelected(false);

            // update current index
            currentIndex = index;
            var current = items[currentIndex];
            current.SetSelected(true);

            // update view position
            list.Recalculate();
            float viewportH = list.GetInnerDimensions().Height;
            float pad = list.ListPadding;

            // distance from top to the selected item
            float yTop = 0f;
            for (int i = 0; i < currentIndex; i++)
                yTop += items[i].GetOuterDimensions().Height + pad;

            float itemH = items[currentIndex].GetOuterDimensions().Height;
            float yBottom = yTop + itemH;

            float view = list.ViewPosition;
            if (yTop < view) view = yTop;
            else if (yBottom > view + viewportH) view = yBottom - viewportH;

            // total content height for clamping
            float totalH = 0f;
            for (int i = 0; i < items.Count; i++)
                totalH += items[i].GetOuterDimensions().Height + pad;

            float maxView = Math.Max(0f, totalH - viewportH);
            list.ViewPosition = MathHelper.Clamp(view, 0f, maxView);
        }
        protected bool JustPressed(Keys key) => Main.keyState.IsKeyDown(key) && Main.oldKeyState.IsKeyUp(key);

        public override void Update(GameTime gt)
        {
            base.Update(gt);

            Top.Set(-38, 0);
            SetHeight();

            FilterSearch();

            HandleNavigationKeys(gt);
            HandleTabComplete();
        }

        private void FilterSearch()
        {
            foreach (Keys key in Enum.GetValues(typeof(Keys)))
            {
                if (!JustPressed(key))
                    continue;

                // skip navigation / control keys
                if (key == Keys.Up || key == Keys.Down || key == Keys.Left || key == Keys.Right ||
                    key == Keys.PageUp || key == Keys.PageDown || key == Keys.Home || key == Keys.End ||
                    key == Keys.Tab || key == Keys.Escape || key == Keys.Enter ||
                    key == Keys.LeftShift || key == Keys.RightShift ||
                    key == Keys.LeftControl || key == Keys.RightControl ||
                    key == Keys.LeftAlt || key == Keys.RightAlt)
                    return;

                //Main.NewText("key: " + key);

                PopulatePanel();
            }
        }

        private void HandleTabComplete()
        {
            if (this is CommandPanel)
                return;

            if (!JustPressed(Keys.Tab))
                return;

            if (items.Count == 0)
                return;

            var current = items[currentIndex];
            if (current == null) return;

            string tag = GetFullTag(current.Data);
            if (string.IsNullOrEmpty(tag)) return;

            if (!Main.chatText.Contains(']'))
            {
                Main.chatText = tag;
            }
            else
            {
                int lb = Main.chatText.LastIndexOf("[e:");
                if (lb >= 0)
                {
                    int rb = Main.chatText.IndexOf(']', lb);
                    if (rb == -1) // only if it's unfinished
                    {
                        // Replace unfinished with the chosen tag
                        string before = Main.chatText.Substring(0, lb);
                        Main.chatText = before + tag;
                    }
                    else
                    {
                        // If it was already closed, just append
                        Main.chatText += tag;
                    }
                }
                else
                {
                    // No [e: found at all → append
                    Main.chatText += tag;
                }
            }

            HandleChatHook.SetCaretPos(Main.chatText.Length);

            // 🔹 reset filter so next emoji search starts fresh
            Main.chatText += " ";
            PopulatePanel();
            ghostText = "";
        }

        private void HandleNavigationKeys(GameTime gt)
        {
            // Tap key
            if (JustPressed(Keys.Up))
            {
                SetSelectedIndex(currentIndex - 1);
                heldKey = Keys.Up;

                repeatTimer = 0.35;
                if (this is ItemPanel)
                {
                    repeatTimer = 0.1;
                }
            }
            else if (JustPressed(Keys.Down))
            {
                SetSelectedIndex(currentIndex + 1);
                heldKey = Keys.Down;
                repeatTimer = 0.35;
                if (this is ItemPanel)
                {
                    repeatTimer = 0.25;
                }
            }

            // Hold key to repeat navigation
            double dt = gt.ElapsedGameTime.TotalSeconds;
            if (Main.keyState.IsKeyDown(heldKey))
            {
                repeatTimer -= dt;
                if (repeatTimer <= 0)
                {
                    if (this is ItemPanel)
                        repeatTimer += 0.03;
                    else
                        repeatTimer += 0.06;

                    if (Main.keyState.IsKeyDown(Keys.Up)) SetSelectedIndex(currentIndex - 1);
                    else if (Main.keyState.IsKeyDown(Keys.Down)) SetSelectedIndex(currentIndex + 1);
                }
            }
        }

        private string ghostText = "";

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);

            if (!Conf.C.featureStyleConfig.ShowAutocompleteText)
                return;

            if (items.Count > 0 && currentIndex >= 0)
            {
                // pick the current suggestion
                if (this is EmojiPanel)
                {
                    ghostText = GetDescription(items[currentIndex].Data);
                    if (!string.IsNullOrEmpty(ghostText))
                        DrawHelper.DrawGhostText(sb, ghostText);
                }
                else if (this is GlyphPanel)
                {
                    //var suggestion = GetFullTag(items[currentIndex].Data);
                    //if (!string.IsNullOrEmpty(suggestion))
                    //    DrawHelper.DrawGhostText(sb, suggestion);
                }
            }
        }
    }
}