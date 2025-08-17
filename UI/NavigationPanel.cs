using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using AdvancedChatFeatures.Colors;
using AdvancedChatFeatures.Commands;
using AdvancedChatFeatures.Common.Configs;
using AdvancedChatFeatures.Common.Hooks;
using AdvancedChatFeatures.Emojis;
using AdvancedChatFeatures.Glyphs;
using AdvancedChatFeatures.Helpers;
using AdvancedChatFeatures.ItemWindow;
using AdvancedChatFeatures.Uploads;
using Microsoft.Xna.Framework;
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
        protected abstract string GetTag(TData data);

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

            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].IsMouseHovering)
                {
                    SetSelectedIndex(i);
                    return;
                }
            }

            //foreach (var e in items)
            //e.SetSelected(false);

            Main.chatText += GetTag(items[currentIndex].Data);
        }

        public void PopulatePanel()
        {
            items.Clear();
            list.Clear();

            var source = GetSource();
            if (source == null) return;

            List<UIElement> fastList = [];
            foreach (var data in source)
            {
                var element = BuildElement(data);
                if (element == null) continue;

                items.Add(element);
                fastList.Add(element);
            }
            list.AddRange(fastList);

            if (items.Count > 0) SetSelectedIndex(0);
            else currentIndex = -1;
        }

        public void SetSelectedIndex(int index)
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

            float totalH = 0f;
            for (int i = 0; i < items.Count; i++)
                totalH += items[i].GetOuterDimensions().Height + pad;

            float maxView = Math.Max(0f, totalH - viewportH);
            list.ViewPosition = MathHelper.Clamp(view, 0f, maxView);

            // 🔹 Update description panel
            if (ConnectedPanel is DescriptionPanel<TData> desc)
            {
                // Skip updating upload description panel
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
                    desc.GetText()._color = ColorElement.HexToColor(GetTag(current.Data));
                }
            }
        }
        protected bool JustPressed(Keys key) => Main.keyState.IsKeyDown(key) && Main.oldKeyState.IsKeyUp(key);

        public override void Update(GameTime gt)
        {
            base.Update(gt);

            // Sizing and position
            Top.Set(-38, 0);
            Height.Set(Conf.C.autocompleteWindowConfig.ItemsPerWindow * 30, 0);
            list.Height.Set(Conf.C.autocompleteWindowConfig.ItemsPerWindow * 30, 0);

            HandleKeyPressed();
            HandleNavigationKeys(gt);
            HandleTabKeyPressed();
        }

        private void HandleKeyPressed()
        {
            foreach (Keys key in Enum.GetValues(typeof(Keys)))
            {
                if (!JustPressed(key))
                    continue;

                if (key == Keys.Tab || key == Keys.Up || key == Keys.Down) return;

                //Main.NewText("key: " + key);

                PopulatePanel();
            }
        }

        private void HandleTabKeyPressed()
        {
            if (!JustPressed(Keys.Tab) || items.Count == 0 || currentIndex < 0)
                return;

            string tag = GetTag(items[currentIndex].Data);

            if (this is CommandPanel)
            {
                Main.chatText = tag;
                HandleChatHook.SetCaretPos(Main.chatText.Length);
                return;
            }

            if (string.IsNullOrEmpty(tag))
                return;

            string text = Main.chatText ?? string.Empty;

            // Determine prefix and filtering logic
            string prefix = this switch
            {
                ColorPanel => "[c",
                EmojiPanel => "[e",
                GlyphPanel => "[g",
                ItemPanel => "[i",
                UploadPanel => "[u",
                _ => "[e"
            };

            int start = text.LastIndexOf(prefix, StringComparison.OrdinalIgnoreCase);
            if (start < 0)
            {
                // no prefix -> just append
                Main.chatText += tag;
                HandleChatHook.SetCaretPos(Main.chatText.Length);
                return;
            }

            // find end of unfinished segment (whitespace or ] or end of string)
            int end = text.IndexOfAny(new[] { ' ', '\t', '\n', '\r', ']' }, start);
            if (end < 0) end = text.Length;

            // replace segment
            string before = text[..start];
            string after = text[end..];
            Main.chatText = before + tag + after;

            HandleChatHook.SetCaretPos(before.Length + tag.Length);
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