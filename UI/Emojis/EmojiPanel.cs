using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Terraria;

namespace AdvancedChatFeatures.UI.Emojis
{
    public class EmojiPanel : NavigationPanel
    {
        private string _lastChatText = string.Empty;

        // For Tab key repeat
        private double repeatTimer;
        private Keys heldKey = Keys.None;

        public EmojiPanel()
        {
            Width.Set(320, 0);
            SetEmojiPanelHeight();
            PopulateEmojiPanel();
        }

        public void PopulateEmojiPanel()
        {
            items.Clear();
            list.Clear();

            List<EmojiElement> elements = [];
            foreach (var emoji in EmojiInitializer.Emojis)
            {
                var element = new EmojiElement(emoji);
                items.Add(element);
                //list.Add(element);
                elements.Add(element);
            }
            list.AddRange(elements);

            SetSelectedIndex(0);
        }

        public override void SetSelectedIndex(int index)
        {
            base.SetSelectedIndex(index);
            // No ghost text & do not write into chat while navigating with arrows
        }

        public override void Update(GameTime gt)
        {
            base.Update(gt);

            // Handle Tab once per frame
            if (JustPressed(Keys.Tab))
            {
                HandleTabKeyPressed();
                repeatTimer = 0.55;
                heldKey = Keys.Tab;
            }

            // Key-hold repeat for Tab
            double dt = gt.ElapsedGameTime.TotalSeconds;
            if (Main.keyState.IsKeyDown(heldKey))
            {
                repeatTimer -= dt;
                if (repeatTimer <= 0)
                {
                    repeatTimer += 0.06; // repeat speed
                    if (Main.keyState.IsKeyDown(Keys.Tab))
                        HandleTabKeyPressed();
                }
            }
            else
            {
                heldKey = Keys.None;
            }

            // Re-filter whenever chat text changes (covers holding Backspace)
            string text = Main.chatText ?? string.Empty;
            if (!string.Equals(text, _lastChatText, StringComparison.Ordinal))
            {
                _lastChatText = text;

                // Skip filtering while arrow keys are held (navigation updates selection)
                bool navigating = Main.keyState.IsKeyDown(Keys.Up) || Main.keyState.IsKeyDown(Keys.Down) || Main.keyState.IsKeyDown(Keys.Tab);
                if (!navigating)
                    ApplyFilter();
            }
        }

        private static string GetEmojiQueryFromChat(string chat)
        {
            if (string.IsNullOrEmpty(chat)) return string.Empty;

            // If user is typing an emoji token starting with ':', take what follows the last ':'.
            // This lets ":sm" filter for "smile", and "hello :sm" also work on the last token.
            int lastColon = chat.LastIndexOf(':');
            if (lastColon >= 0)
                return chat[(lastColon + 1)..]; // text after the last ':'

            // Otherwise, no explicit token – return empty to show all
            return string.Empty;
        }

        private void ApplyFilter()
        {
            string chat = Main.chatText ?? string.Empty;
            string query = GetEmojiQueryFromChat(chat);
            string q = query.Trim();
            bool hasQuery = q.Length > 0;

            items.Clear();
            list.Clear();

            if (!hasQuery)
            {
                // No query: show all
                List<EmojiElement> elements = [];
                foreach (var emoji in EmojiInitializer.Emojis)
                {
                    var element = new EmojiElement(emoji);
                    items.Add(element);
                    //list.Add(element);
                    elements.Add(element);
                }
                list.AddRange(elements);

                if (items.Count > 0) SetSelectedIndex(0);
                return;
            }

            // Normalize query for matching against DisplayName and Tag (without colons)
            string qLower = q.ToLowerInvariant();

            // 1) Prefix matches first
            foreach (var emoji in EmojiInitializer.Emojis)
            {
                string name = emoji.DisplayName ?? "";
                string nameLower = name.ToLowerInvariant();
                string tagNameLower = (emoji.Tag ?? "").Trim(':').ToLowerInvariant();

                List<EmojiElement> elements = [];

                if (nameLower.StartsWith(qLower) || tagNameLower.StartsWith(qLower))
                {
                    var e = new EmojiElement(emoji);
                    items.Add(e);
                    //list.Add(e);
                    elements.Add(e);
                }
                list.AddRange(elements);
            }

            // 2) Contains matches next (exclude already added)
            foreach (var emoji in EmojiInitializer.Emojis)
            {
                if (items.Any(x => ((EmojiElement)x).Emoji == emoji)) continue;

                string name = emoji.DisplayName ?? "";
                string nameLower = name.ToLowerInvariant();
                string tagNameLower = (emoji.Tag ?? "").Trim(':').ToLowerInvariant();

                if (nameLower.Contains(qLower) || tagNameLower.Contains(qLower))
                {
                    var e = new EmojiElement(emoji);
                    items.Add(e);
                    list.Add(e);
                }
            }

            if (items.Count > 0)
                SetSelectedIndex(0);
            else
                currentIndex = 0;
        }

        private void HandleTabKeyPressed()
        {
            if (items.Count > 0 && currentIndex >= 0 && currentIndex <= items.Count)
            {
                var current = (EmojiElement)items[currentIndex];

                if (Main.chatText.Length <= 3)
                    Main.chatText = current.Emoji.Tag; // "[g:0]"
                else
                    Main.chatText += current.Emoji.Tag; // "[g:0]"
            }
        }
    }
}
