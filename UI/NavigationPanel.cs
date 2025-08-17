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
using AdvancedChatFeatures.Uploads;
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
        protected virtual string Prefix => ""; // override in subclasses

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

        public void PopulatePanel()
        {
            items.Clear();
            list.Clear();

            var source = GetSource();
            if (source == null) return;

            string query = ExtractQuery(Main.chatText ?? string.Empty);
            bool doFilter = !string.IsNullOrWhiteSpace(query);

            // Stable two buckets
            List<NavigationElement<TData>> exact = new();
            List<NavigationElement<TData>> partial = new();

            foreach (var data in source)
            {
                if (doFilter && !MatchesFilter(data, query))
                    continue;

                var element = BuildElement(data);
                if (element == null) continue;

                string tag = GetFullTag(data) ?? string.Empty;
                string desc = GetDescription(data) ?? string.Empty;

                bool isExact = (!string.IsNullOrEmpty(query)) &&
                               (tag.Equals(query, StringComparison.OrdinalIgnoreCase) ||
                                desc.Equals(query, StringComparison.OrdinalIgnoreCase));

                (isExact ? exact : partial).Add(element);
            }

            // Exact first, then partial; preserve original order within each bucket
            if (exact.Count > 0) { items.AddRange(exact); list.AddRange(exact); }
            if (partial.Count > 0) { items.AddRange(partial); list.AddRange(partial); }

            if (items.Count > 0) SetSelectedIndex(0);
            else currentIndex = -1;
        }

        protected virtual bool MatchesFilter(TData data, string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return true;
            string tag = GetFullTag(data) ?? string.Empty;
            string desc = GetDescription(data) ?? string.Empty;
            return tag.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0
                || desc.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public void SetHeight()
        {
            // Set height
            Height.Set(Conf.C.autocompleteWindowConfig.ItemsPerWindow * 30, 0);
            list.Height.Set(Conf.C.autocompleteWindowConfig.ItemsPerWindow * 30, 0);
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

            // 🔹 Update description panel
            if (ConnectedPanel is DescriptionPanel<TData> desc)
            {
                if (typeof(TData) == typeof(Upload))
                {
                    return;
                }

                var element = items[currentIndex];
                if (element != null)
                {
                    if (element.Data is Emoji emoji && emoji.Synonyms.Count > 0)
                    {
                        desc.SetTextWithLinebreak(string.Join(", ", emoji.Synonyms));
                    }
                    else
                    {
                        desc.SetTextWithLinebreak(GetDescription(element.Data));
                    }
                }

                // Set color text if description panel is connected to a color panel
                if (desc.ConnectedPanel.GetType() == typeof(ColorPanel))
                {
                    desc.GetText()._color = ColorWindowElement.HexToColor(GetFullTag(current.Data));
                }
            }
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

        protected virtual string ExtractQuery(string text)
        {
            text ??= string.Empty;

            // Commands: "/foo" -> "foo"
            if (Prefix == "/")
            {
                if (!text.StartsWith("/")) return string.Empty;
                int space = text.IndexOf(' ');
                return (space >= 0 ? text.Substring(1, space - 1) : text.Substring(1)).Trim();
            }

            // Emojis: "…;smil" -> "smil"  (last semicolon to next whitespace/end)
            if (Prefix == ";")
            {
                int start = text.LastIndexOf(';');
                if (start < 0 || start == text.Length - 1) return string.Empty;

                int end = text.IndexOfAny(new[] { ' ', '\t', '\n', '\r' }, start + 1);
                string span = end > start ? text.Substring(start + 1, (end - start - 1)) : text.Substring(start + 1);
                return span.Trim().Trim('"');
            }

            // Bracketed tags: "[i", "[g", "[c", "[u" etc.
            if (!string.IsNullOrEmpty(Prefix) && Prefix.StartsWith("["))
            {
                int start = text.LastIndexOf(Prefix, StringComparison.OrdinalIgnoreCase);
                if (start < 0) return string.Empty;

                int after = start + Prefix.Length;
                // Optional colon after the prefix, e.g. "[i:..."
                if (after < text.Length && text[after] == ':') after++;

                // Until closing ']' or end
                int rb = text.IndexOf(']', after);
                string span = rb > after ? text.Substring(after, rb - after) : text.Substring(after);
                return span.Trim().Trim('"');
            }

            return string.Empty;
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
            if (!JustPressed(Keys.Tab)) return;
            if (items.Count == 0) return;

            var current = items[currentIndex];
            if (current == null) return;

            string insert = GetFullTag(current.Data);
            if (string.IsNullOrEmpty(insert)) return;

            string text = Main.chatText ?? string.Empty;

            // Commands: replace "/foo" (until first space) with "/realcmd "
            if (Prefix == "/")
            {
                if (!text.StartsWith("/")) { Main.chatText += insert + " "; HandleChatHook.SetCaretPos(Main.chatText.Length); return; }
                int space = text.IndexOf(' ');
                int end = space >= 0 ? space : text.Length;
                Main.chatText = insert + " " + (space >= 0 ? text.Substring(space + 1) : string.Empty);
                HandleChatHook.SetCaretPos(insert.Length + 1);
                return;
            }

            // Emojis: replace the word that starts at the last ';' up to next whitespace/end
            if (Prefix == ";")
            {
                int start = text.LastIndexOf(';');
                if (start < 0) { Main.chatText += insert; HandleChatHook.SetCaretPos(Main.chatText.Length); return; }

                int end = text.IndexOfAny(new[] { ' ', '\t', '\n', '\r' }, start + 1);
                string before = text.Substring(0, start);
                string after = (end >= 0 ? text.Substring(end) : string.Empty);
                Main.chatText = before + insert + after;
                HandleChatHook.SetCaretPos((before + insert).Length);
                return;
            }

            // Bracketed tags like "[i", "[g", "[c", "[u]"
            if (!string.IsNullOrEmpty(Prefix) && Prefix.StartsWith("["))
            {
                int start = text.LastIndexOf(Prefix, StringComparison.OrdinalIgnoreCase);
                if (start < 0) { Main.chatText += insert; HandleChatHook.SetCaretPos(Main.chatText.Length); return; }

                int after = start + Prefix.Length;
                if (after < text.Length && text[after] == ':') after++;

                int rb = text.IndexOf(']', after);
                int end = (rb >= 0 ? rb + 1 : text.Length);

                string before = text.Substring(0, start);
                string afterText = (end < text.Length ? text.Substring(end) : string.Empty);

                Main.chatText = before + insert + afterText;
                HandleChatHook.SetCaretPos((before + insert).Length);
                return;
            }

            // Fallback
            Main.chatText += insert;
            HandleChatHook.SetCaretPos(Main.chatText.Length);
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
    }
}