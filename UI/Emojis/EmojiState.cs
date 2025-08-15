using AdvancedChatFeatures.Common.Configs;
using AdvancedChatFeatures.Helpers;
using AdvancedChatFeatures.UI.Commands;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;

namespace AdvancedChatFeatures.UI.Emojis
{
    public class EmojiState : UIState
    {
        public EmojiPanel emojiPanel;
        public EmojiUsagePanel emojiUsagePanel;

        public EmojiState()
        {
            // Initialize the UI elements
            emojiPanel = new();
            Append(emojiPanel);

            emojiUsagePanel = new();
            emojiUsagePanel.UpdateTopPosition();
            Append(emojiUsagePanel);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
