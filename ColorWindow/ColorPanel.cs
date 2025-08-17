using System.Collections.Generic;
using AdvancedChatFeatures.UI;
using Microsoft.Xna.Framework;

namespace AdvancedChatFeatures.ColorWindow
{
    public class ColorPanel : NavigationPanel<Color>
    {
        protected override NavigationElement<Color> BuildElement(Color data) =>
            new ColorWindowElement(data);

        protected override IEnumerable<Color> GetSource() =>
            ColorInitializer.Colors;

        protected override string GetDescription(Color data)
            => data.Description;

        protected override string GetFullTag(Color data)
            => data.Tag;

        protected override string Prefix => "[c";

        public override void Update(GameTime gt)
        {
            if (items.Count == 0)
                PopulatePanel();

            base.Update(gt);
        }
    }
}
