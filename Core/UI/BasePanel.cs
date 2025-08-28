using System;
using System.Collections.Generic;
using ChatPlus.Common.Configs;
using ChatPlus.Core.Chat;
using ChatPlus.Core.Features.Colors;
using ChatPlus.Core.Features.Commands;
using ChatPlus.Core.Features.Emojis;
using ChatPlus.Core.Features.Glyphs;
using ChatPlus.Core.Features.Items;
using ChatPlus.Core.Features.ModIcons;
using ChatPlus.Core.Features.PlayerHeads;
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

        // Navigation
        protected int currentIndex = 0; // first item

        // Commands
        private static bool freezeCommandFilter;
        private static string frozenCommandText;

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

            if (fastList.Count == 0)
            {
                // Update description panel to say "no entries found"
                if (ConnectedPanel is DescriptionPanel<TData> descPanel)
                {
                    descPanel.SetTextWithLinebreak("No entries found.");
                }
            }

            if (items.Count > 0) SetSelectedIndex(0);
            else currentIndex = -1;
        }

        #region Filter
        private bool MatchesFilter(TData data)
        {
            string tag = GetTag(data) ?? string.Empty;

            // Use frozen text while navigating commands with Up/Down so we don't re-filter to a single item
            string text = Main.chatText ?? string.Empty;
            if (this is CommandPanel && freezeCommandFilter && !string.IsNullOrEmpty(frozenCommandText))
                text = frozenCommandText;

            if (text.Length == 0) return true;

            string prefix =
                this is CommandPanel ? "/" :
                this is ColorPanel ? "[c" :
                this is EmojiPanel ? "[e" :
                this is GlyphPanel ? "[g" :
                this is ItemPanel ? "[i" :
                this is ModIconPanel ? "[m" :
                this is PlayerHeadPanel ? "[p" :
                this is UploadPanel ? "[u" : string.Empty;

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

            if (this is ItemPanel && data is Features.Items.Item item)
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
            string tag = GetTag(current.Data);

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
                        descPanel.SetTextWithLinebreak(string.Join(", ", emoji.Synonyms));
                    }
                    else
                    {
                        descPanel.SetTextWithLinebreak(desc);
                    }
                }

                // Set color text if description panel is connected to a color panel
                if (descPanel.ConnectedPanel.GetType() == typeof(ColorPanel))
                {
                    descPanel.GetText()._color = ColorElement.HexToColor(tag);
                }
            }
        }

        private void HandleKeyPressed()
        {
            foreach (Keys key in Enum.GetValues(typeof(Keys)))
            {
                if (!JustPressed(key))
                    continue;

                if (key == Keys.Tab || key == Keys.Up || key == Keys.Down) return;

                // Any non-Tab/Up/Down key: resume normal filtering
                freezeCommandFilter = false;
                frozenCommandText = null;

                PopulatePanel();
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

        public void InsertSelectedTag()
        {
            if (items.Count == 0 || currentIndex < 0) return;

            string tag = GetTag(items[currentIndex].Data);
            if (string.IsNullOrEmpty(tag)) return;

            if (this is CommandPanel)
            {
                Main.chatText = tag;
                HandleChatSystem.SetCaretPos(Main.chatText.Length);
                return;
            }

            string text = Main.chatText ?? string.Empty;

            string prefix = this switch
            {
                ColorPanel => "[c",
                EmojiPanel => "[e",
                GlyphPanel => "[g",
                ItemPanel => "[i",
                ModIconPanel => "[m",
                PlayerHeadPanel => "[p",
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

            int end = text.IndexOfAny([' ', '\t', '\n', '\r', ']'], start);
            if (end < 0) end = text.Length;

            string before = text[..start];
            string after = text[end..];
            Main.chatText = before + tag + after;

            HandleChatSystem.SetCaretPos(before.Length + tag.Length);
        }

        private void HandleNavigationKeys(GameTime gt)
        {
            // Tap key
            if (JustPressed(Keys.Up))
            {
                if (this is CommandPanel)
                {
                    if (!freezeCommandFilter) frozenCommandText = Main.chatText;
                    freezeCommandFilter = true;

                    SetSelectedIndex(currentIndex - 1);
                    if (items.Count > 0)
                    {
                        Main.chatText = GetTag(items[currentIndex].Data);
                        HandleChatSystem.SetCaretPos(Main.chatText.Length);
                    }
                }
                else
                {
                    SetSelectedIndex(currentIndex - 1);
                }
                heldKey = Keys.Up;
                repeatTimer = 0.35;
            }
            else if (JustPressed(Keys.Down))
            {
                if (this is CommandPanel)
                {
                    if (!freezeCommandFilter) frozenCommandText = Main.chatText;
                    freezeCommandFilter = true;

                    SetSelectedIndex(currentIndex + 1);
                    if (items.Count > 0)
                    {
                        Main.chatText = GetTag(items[currentIndex].Data);
                        HandleChatSystem.SetCaretPos(Main.chatText.Length);
                    }
                }
                else
                {
                    SetSelectedIndex(currentIndex + 1);
                }
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
                        if (this is CommandPanel && items.Count > 0)
                        {
                            Main.chatText = GetTag(items[currentIndex].Data);
                            HandleChatSystem.SetCaretPos(Main.chatText.Length);
                        }
                    }
                    else if (Main.keyState.IsKeyDown(Keys.Down))
                    {
                        SetSelectedIndex(currentIndex + 1);
                        if (this is CommandPanel && items.Count > 0)
                        {
                            Main.chatText = GetTag(items[currentIndex].Data);
                            HandleChatSystem.SetCaretPos(Main.chatText.Length);
                        }
                    }
                }
            }
        }
        #endregion
    }
}