using AdvancedChatFeatures.UI;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;

namespace AdvancedChatFeatures.UploadWindow
{
    public class UploadState : UIState
    {
        public UploadPanel panel;
        public DescriptionPanel<Upload> desc;

        public UploadState()
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
