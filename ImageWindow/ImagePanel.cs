using System.Collections.Generic;
using AdvancedChatFeatures.ImageWindow;
using AdvancedChatFeatures.UI;
using Microsoft.Xna.Framework;
using Terraria;

namespace AdvancedChatFeatures.ImageWindow
{
    public class ImagePanel : NavigationPanel<Image>
    {
        protected override NavigationElement<Image> BuildElement(Image data) => new ImageElement(data);
        protected override IEnumerable<Image> GetSource() => ImageInitializer.Uploads;
        protected override string GetDescription(Image data) => data.FileName;
        protected override string GetFullTag(Image data) => data.Tag;

        public override void Update(GameTime gt)
        {
            if (items.Count == 0)
                PopulatePanel();

            base.Update(gt);
        }
    }
}
