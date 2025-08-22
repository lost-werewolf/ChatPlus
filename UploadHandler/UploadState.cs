using ChatPlus.UI;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;

namespace ChatPlus.UploadHandler
{
    public class UploadState : BaseState<Upload>
    {
        public UploadPanel panel;
        public DescriptionPanel<Upload> desc;

        public UploadState()
        {
            panel = new();
            Append(panel);

            desc = new("Left click to upload an image\nRight click to open image folder\nExample: [u:Your_Image|100] for a 100px image");
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
