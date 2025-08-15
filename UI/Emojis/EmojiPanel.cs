using System.Collections.Generic;
using AdvancedChatFeatures.Helpers;
using AdvancedChatFeatures.UI.Glyphs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.UI;

namespace AdvancedChatFeatures.UI.Emojis
{
    public class EmojiPanel : NavigationPanel
    {
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

            Log.Info($"start");

            var elements = new List<UIElement>(EmojiInitializer.Emojis.Count);

            foreach (Emoji emoji in EmojiInitializer.Emojis)
            {
                EmojiElement element = new(emoji);
                items.Add(element);
                list.Add(element);
                //elements.Add(new EmojiElement(emoji));
            }

            //list.AddRange(elements);

            Log.Info($"end");

            SetSelectedIndex(0);
        }

        public override void SetSelectedIndex(int index)
        {
            base.SetSelectedIndex(index);
        }

        // Holding keys
        private double repeatTimer;
        private Keys heldKey = Keys.None;

        public override void Update(GameTime gt)
        {
            base.Update(gt);

            if (JustPressed(Keys.Tab) || Main.keyState.IsKeyDown(Keys.Tab))

                // Tap key
                if (JustPressed(Keys.Tab))
                {
                    HandleTabKeyPressed();
                    repeatTimer = 0.55;
                    heldKey = Keys.Tab;
                }

            // Hold key
            double dt = gt.ElapsedGameTime.TotalSeconds;
            if (Main.keyState.IsKeyDown(heldKey))
            {
                repeatTimer -= dt;
                if (repeatTimer <= 0)
                {
                    repeatTimer += 0.06; // repeat speed
                    if (Main.keyState.IsKeyDown(Keys.Tab)) HandleTabKeyPressed();
                }
            }
        }

        private void HandleTabKeyPressed()
        {
            if (items.Count > 0 && currentIndex >= 0 && currentIndex <= items.Count)
            {
                var current = (EmojiElement)items[currentIndex];

                if (Main.chatText.Length <= 3)
                    Main.chatText = current.Emoji.Tag; // "[e:0]"
                else
                    Main.chatText += current.Emoji.Tag; // "[e:0]"
            }
        }
    }
}
