using System.Collections.Generic;
using ChatPlus.Core.UI;
using Microsoft.Xna.Framework;

namespace ChatPlus.Core.Features.Colors
{
    public class ColorPanel : BasePanel<ColorEntry>
    {
        protected override BaseElement<ColorEntry> BuildElement(ColorEntry data) => new ColorElement(data);
        protected override IEnumerable<ColorEntry> GetSource() => ColorManager.Colors;
        protected override string GetDescription(ColorEntry data) => data.Description;
        protected override string GetTag(ColorEntry data) => data.Tag;

        public override void Update(GameTime gt)
        {
            base.Update(gt);
        }
    }
}
