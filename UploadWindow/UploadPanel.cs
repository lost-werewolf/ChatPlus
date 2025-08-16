using System.Collections.Generic;
using AdvancedChatFeatures.UI;
using Microsoft.Xna.Framework;
using Terraria;

namespace AdvancedChatFeatures.UploadWindow
{
    public class UploadPanel : NavigationPanel<Upload>
    {
        protected override NavigationElement<Upload> BuildElement(Upload data) => new UploadElement(data);
        protected override IEnumerable<Upload> GetSource() => UploadInitializer.Uploads;
        protected override string GetDescription(Upload data) => data.FileName;
        protected override string GetFullTag(Upload data) => data.Tag;

        public override void Update(GameTime gt)
        {
            if (items.Count == 0)
                PopulatePanel();

            base.Update(gt);
        }
    }
}
