using System;
using System.Collections.Generic;
using ChatPlus.Common.Configs;
using ChatPlus.Core.Chat;
using ChatPlus.Core.Features.Colors;
using ChatPlus.Core.Features.Commands;
using ChatPlus.Core.Features.Emojis;
using ChatPlus.Core.Features.Glyphs;
using ChatPlus.Core.Features.Items;
using ChatPlus.Core.Features.Links;
using ChatPlus.Core.Features.Mentions;
using ChatPlus.Core.Features.ModIcons;
using ChatPlus.Core.Features.PlayerColors;
using ChatPlus.Core.Features.PlayerIcons;
using ChatPlus.Core.Features.Uploads;
using ChatPlus.Core.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace ChatPlus.Core.UI
{
    public abstract class BasePanel<TData> : DraggablePanel
    {
        // Elements
        public readonly UIScrollbar scrollbar;
        protected UIList list;
        public readonly List<BaseElement<TData>> items = [];

        // Force populate
        protected abstract IEnumerable<TData> GetSource(); // The source of data to populate the panel with
        protected abstract BaseElement<TData> BuildElement(TData data); // The method to create a new element from the data
        protected abstract string GetDescription(TData data);
        protected abstract string GetTag(TData data);

        public bool TryGetSelected(out TData data)
        {
            if (currentIndex >= 0 && currentIndex < items.Count && items[currentIndex] != null)
            {
                data = items[currentIndex].Data;
                return true;
            }
            data = default;
            return false;
        }

        // Navigation
        protected int currentIndex = 0; // first item

        // Holding keys
        private double repeatTimer;
        private Keys heldKey = Keys.None;
        protected bool JustPressed(Keys key) => Main.keyState.IsKeyDown(key) && Main.oldKeyState.IsKeyUp(key);

        public BasePanel()
        {
            // Set width
            Width.Set(320, 0);

            // Set position just above the chat
            VAlign = 1f;
            Top.Set(-38, 0);
            Left.Set(190, 0);

            // Style
            OverflowHidden = true;
            BackgroundColor = new Color(33, 43, 79) * 1.0f;
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
        }

        public override void OnActivate()
        {
            base.OnActivate();

            PopulatePanel();

            int itemCount = 10;
            if (Conf.C != null)
            {
                itemCount = (int)Conf.C.AutocompleteItemCount;
            }

            Top.Set(-38, 0f);
            Height.Set(itemCount * 30, 0f);
            list.Height.Set(itemCount * 30, 0f);

            list.ViewPosition = 0f;
            currentIndex = 0;
            if (items.Count > 0)
            {
                SetSelectedIndex(0);
            }

            Recalculate();

            Main.oldKeyState = Main.keyState;

            // Scroll to top
            scrollbar.SetView(0, scrollbar.MaxViewSize);
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            bool leftShiftDown = Main.keyState.IsKeyDown(Keys.LeftShift);

            if (!leftShiftDown)
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
            }
            else
            {
                RemoveUploadElement();
            }
        }

        private void RemoveUploadElement()
        {
            int indexToDelete = -1;

            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].IsMouseHovering && items[i].Data is Upload)
                {
                    indexToDelete = i;
                    break;
                }
            }

            if (indexToDelete >= 0 && indexToDelete < items.Count)
            {
                var element = items[indexToDelete];
                if (element.Data is Upload upload)
                {
                    UploadManager.Uploads.Remove(upload);

                    items.RemoveAt(indexToDelete);
                    element.Remove();

                    Recalculate();
                    PopulatePanel();
                }
            }
        }

        public void ClearPanel()
        {
            items.Clear();
            list.Clear();
        }

        public void PopulatePanel()
        {
            ClearPanel();

            var source = GetSource();
            if (source == null) return;

            List<UIElement> fastList = [];
            foreach (var data in source)
            {
                var element = BuildElement(data);
                if (element == null) continue;
                if (!MatchesFilter(data)) continue;

                items.Add(element);
                fastList.Add(element);
            }
            list.AddRange(fastList);
        }

        #region Filter
        protected virtual bool MatchesFilter(TData data)
        {
            if (this is EmojiPanel && EmojiSystem.FilterReset)
                return true;

            string tag = GetTag(data) ?? string.Empty;

            // Use frozen text while navigating commands with Up/Down so we don't re-filter to a single item
            string text = Main.chatText ?? string.Empty;

            if (text.Length == 0) return true;

            string prefix =
                this is CommandPanel ? "/" :
                this is ColorPanel ? "[c" :
                this is EmojiPanel ? "[e" :
                this is GlyphPanel ? "[g" :
                this is ItemPanel ? "[i" :
                this is ModIconPanel ? "[m" :
                this is PlayerIconPanel ? "[p" :
                this is UploadPanel ? "[u" :
                this is LinkPanel ? "[l" :
                this is MentionPanel ? "@" : string.Empty;

            if (prefix.Length == 0)
                return tag.IndexOf(text, StringComparison.OrdinalIgnoreCase) >= 0;

            char[] stopChars = [' ', '\t', '\n', '\r', ']'];

            if (prefix == "/")
            {
                int s = text.LastIndexOf('/');
                if (s < 0 || s + 1 >= text.Length) return true;

                int e = text.IndexOfAny(stopChars, s + 1); if (e < 0) e = text.Length;
                string q = text.Substring(s + 1, e - (s + 1)).Trim();
                if (q.Length == 0) return true;
                return tag.IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0;
            }

            int start = text.LastIndexOf(prefix, StringComparison.OrdinalIgnoreCase);
            if (start < 0)
                return tag.IndexOf(text, StringComparison.OrdinalIgnoreCase) >= 0;

            int qStart = start + prefix.Length;
            if (qStart < text.Length && (text[qStart] == ':' || text[qStart] == '/')) qStart++;
            if (qStart >= text.Length) return true;

            int qEnd = text.IndexOfAny(stopChars, qStart); if (qEnd < 0) qEnd = text.Length;
            string query = text.Substring(qStart, qEnd - qStart).Trim();
            if (query.Length == 0) return true;

            if (this is EmojiPanel && data is Emoji emoji)
            {
                if (tag.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0) return true;
                foreach (string syn in emoji.Synonyms)
                    if (!string.IsNullOrEmpty(syn) && syn.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0)
                        return true;
                return false;
            }

            if (this is ItemPanel && data is ItemEntry item)
            {
                if (tag.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0) return true;
                if ((item.DisplayName ?? string.Empty).IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0) return true;
                if (int.TryParse(query, out int qid) && qid == item.ID) return true;
                return false;
            }

            return tag.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        #endregion

        public override void Update(GameTime gt)
        {
            base.Update(gt);

            // debug TODO
            //OverflowHidden = false;
            //list.OverflowHidden = false;

            // Sizing and position
            int itemCount = 10;
            if (Conf.C != null)
                itemCount = (int)Conf.C.AutocompleteItemCount;
            Top.Set(-38, 0);
            Height.Set(itemCount * 30, 0);
            list.Height.Set(itemCount * 30, 0);

            HandleKeyPressed();
            HandleNavigationKeys(gt);
            HandleTabKeyPressed();
        }

        #region Navigation

        public void SetSelectedIndex(int index)
        {
            if (items.Count == 0) return;

            // Wrap around.
            if (index < 0) index = items.Count - 1;
            else if (index >= items.Count) index = 0;

            // Deselect all.
            for (int i = 0; i < items.Count; i++)
            {
                items[i].SetSelected(false);
            }

            // Update current index and select.
            currentIndex = index;
            var current = items[currentIndex];
            current.SetSelected(true);
            string tag = GetTag(current.Data);

            // Ensure layout is calculated before measuring.
            list.Recalculate();

            float viewportH = list.GetInnerDimensions().Height;
            float pad = list.ListPadding;

            if (viewportH <= 1f)
            {
                // First-activation frame: viewport not ready yet.
                // Do not auto-scroll; pin to top to avoid jumping to "second" row later.
                list.ViewPosition = 0f;
            }
            else
            {
                // Distance from top to selected item.
                float yTop = 0f;
                for (int i = 0; i < currentIndex; i++)
                {
                    yTop += items[i].GetOuterDimensions().Height + pad;
                }

                float itemH = items[currentIndex].GetOuterDimensions().Height;
                float yBottom = yTop + itemH;

                float view = list.ViewPosition;

                if (yTop < view)
                {
                    view = yTop;
                }
                else if (yBottom > view + viewportH)
                {
                    view = yBottom - viewportH;
                }

                // Clamp against total content height.
                float totalH = 0f;
                for (int i = 0; i < items.Count; i++)
                {
                    totalH += items[i].GetOuterDimensions().Height + pad;
                }

                float maxView = Math.Max(0f, totalH - viewportH);
                list.ViewPosition = MathHelper.Clamp(view, 0f, maxView);
            }

            // 🔹 Update description panel
            if (ConnectedPanel is DescriptionPanel<TData> descPanel)
            {
                // Skip updating upload description panel
                if (typeof(TData) == typeof(Upload))
                {
                    return;
                }

                var element = items[currentIndex];
                if (element != null)
                {
                    string desc = GetDescription(element.Data);

                    if (element.Data is Emoji emoji && emoji.Synonyms.Count > 0)
                    {
                        descPanel.SetText(string.Join(", ", emoji.Synonyms));
                    }
                    else
                    {
                        descPanel.SetText(desc);
                    }
                }

                // Set color text if description panel is connected to a color panel
                if (descPanel.ConnectedPanel.GetType() == typeof(ColorPanel))
                {
                    descPanel.GetText()._color = PlayerColorHandler.HexToColor(tag);
                }
            }
        }

        private void HandleKeyPressed()
        {
            foreach (Keys key in Enum.GetValues(typeof(Keys)))
            {
                if (!JustPressed(key))
                    continue;

                if (key == Keys.Tab ||
                    key == Keys.Up ||
                    key == Keys.Down ||
                    key == Keys.LeftControl || key == Keys.LeftShift)
                    return;

                int prevIndex = currentIndex;
                PopulatePanel();

                // ensure something is still selected
                if (items.Count > 0)
                {
                    int newIndex = Math.Clamp(prevIndex, 0, items.Count - 1);
                    SetSelectedIndex(newIndex);
                }
                else
                {
                    if (ConnectedPanel is DescriptionPanel<TData> descPanel)
                    {
                        descPanel.SetText("No entries found.");
                    }
                }
            }
        }

        private void HandleTabKeyPressed()
        {
            if (!JustPressed(Keys.Tab) || items.Count == 0 || currentIndex < 0)
                return;

            InsertSelectedTag();

            if (this is ColorPanel)
            {
                Main.chatText += "]";
            }
        }

        /// <summary>
        /// Handle inserting most tags. 
        /// Special cases are overriden in their separate panels for commands, emojis, mentions, etc.
        /// </summary>
        public virtual void InsertSelectedTag()
        {
            if (items.Count == 0 || currentIndex < 0) return;

            string tag = GetTag(items[currentIndex].Data);
            if (string.IsNullOrEmpty(tag)) return;

            string text = Main.chatText ?? string.Empty;
            int caret = Math.Clamp(HandleChatSystem.GetCaretPos(), 0, text.Length);
            char[] stops = [' ', '\t', '\n', '\r', ']'];

            string prefix = this switch
            {
                ColorPanel => "[c",
                GlyphPanel => "[g",
                ItemPanel => "[i",
                ModIconPanel => "[m",
                PlayerIconPanel => "[p",
                UploadPanel => "[u",
                _ => "[e"
            };

            int start = text.LastIndexOf(prefix, StringComparison.OrdinalIgnoreCase);
            if (start < 0)
            {
                Main.chatText += tag;
                HandleChatSystem.SetCaretPos(Main.chatText.Length);
                return;
            }

            int end2 = text.IndexOfAny(stops, start);
            if (end2 < 0) end2 = text.Length;

            string before2 = text[..start];
            string after2 = text[end2..];
            Main.chatText = before2 + tag + after2;
            HandleChatSystem.SetCaretPos(before2.Length + tag.Length);
        }

        private void HandleNavigationKeys(GameTime gt)
        {
            // Tap key
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

            // Hold key to repeat navigation
            double dt = gt.ElapsedGameTime.TotalSeconds;
            if (Main.keyState.IsKeyDown(heldKey))
            {
                repeatTimer -= dt;
                if (repeatTimer <= 0)
                {
                    repeatTimer += 0.06;

                    if (Main.keyState.IsKeyDown(Keys.Up))
                    {
                        SetSelectedIndex(currentIndex - 1);
                    }
                    else if (Main.keyState.IsKeyDown(Keys.Down))
                    {
                        SetSelectedIndex(currentIndex + 1);
                    }
                }
            }
        }
        #endregion
    }
}