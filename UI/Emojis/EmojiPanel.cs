using Microsoft.Xna.Framework;

namespace AdvancedChatFeatures.UI.Emojis
{
    public class EmojiPanel : NavigationPanel
    {
        public EmojiPanel()
        {
            SetEmojiPanelHeight();
            PopulateEmojiPanel();
        }

        public void PopulateEmojiPanel()
        {
            items.Clear();
            list.Clear();

            foreach (var emoji in EmojiInitializer.Emojis)
            {
                EmojiElement element = new(emoji);
                items.Add(element);
                list.Add(element);
            }

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
