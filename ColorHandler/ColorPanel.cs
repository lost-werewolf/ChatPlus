using System.Collections.Generic;
using AdvancedChatFeatures.UI;
using Microsoft.Xna.Framework;

namespace AdvancedChatFeatures.ColorHandler
{
    public class ColorPanel : BasePanel<ColorItem>
    {
        protected override BaseElement<ColorItem> BuildElement(ColorItem data) => new ColorElement(data);
        protected override IEnumerable<ColorItem> GetSource() => ColorInitializer.Colors;
        protected override string GetDescription(ColorItem data) => data.Description;
        protected override string GetTag(ColorItem data) => data.Tag;

        public override void Update(GameTime gt)
        {
            if (items.Count == 0)
                PopulatePanel();

            base.Update(gt);
        }
    }
}
