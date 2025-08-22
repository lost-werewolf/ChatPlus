using System.Collections.Generic;
using ChatPlus.UI;
using Microsoft.Xna.Framework;
using Terraria;

namespace ChatPlus.UploadHandler
{
    public class UploadPanel : BasePanel<Upload>
    {
        protected override BaseElement<Upload> BuildElement(Upload data) => new UploadElement(data);
        protected override IEnumerable<Upload> GetSource() => UploadInitializer.Uploads;
        protected override string GetDescription(Upload data) => data.FileName;
        protected override string GetTag(Upload data) => data.Tag;

        public override void Update(GameTime gt)
        {
            if (items.Count == 0)
                PopulatePanel();

            base.Update(gt);
        }
    }
}
