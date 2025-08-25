using Microsoft.Xna.Framework.Graphics;

namespace ChatPlus.Core.Features.Uploads
{
    public class UploadState : BaseState<Upload>
    {
        public UploadPanel panel;
        public DescriptionPanel<Upload> desc;

        public UploadState()
        {
            panel = new();
            Append(panel);

            desc = new("Click to upload");
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
