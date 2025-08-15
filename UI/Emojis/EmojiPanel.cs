using System.Collections.Generic;
using AdvancedChatFeatures.Helpers;
using Microsoft.Xna.Framework;
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

        public override void Update(GameTime gt)
        {
            base.Update(gt);
        }
    }
}
