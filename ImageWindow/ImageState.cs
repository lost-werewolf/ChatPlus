using AdvancedChatFeatures.ImageWindow;
using AdvancedChatFeatures.UI;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;

namespace AdvancedChatFeatures.ImageWindow
{
    public class ImageState : UIState
    {
        public ImagePanel panel;
        public DescriptionPanel<Image> desc;

        public ImageState()
        {
            panel = new();
            Append(panel);

            desc = new("Upload images\nClick here to upload a file");
            Append(desc);

            panel.ConnectedPanel = desc;
            desc.ConnectedPanel = panel;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
