using AdvancedChatFeatures.UI;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;

namespace AdvancedChatFeatures.Emojis
{
    public class EmojiState : UIState
    {
        public EmojiPanel emojiPanel;
        public DescriptionPanel<Emoji> emojiDescriptionPanel;

        public EmojiState()
        {
            // Initialize the UI elements
            emojiPanel = new();
            Append(emojiPanel);

            emojiDescriptionPanel = new("List of emojis");
            Append(emojiDescriptionPanel);

            emojiPanel.ConnectedPanel = emojiDescriptionPanel;
            emojiDescriptionPanel.ConnectedPanel = emojiPanel;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
