using System.Collections.Generic;

namespace ChatPlus.Core.Features.ModIcons;

public class ModIconPanel : BasePanel<ModIcon>
{
    protected override IEnumerable<ModIcon> GetSource() => ModIconInitializer.ModIcons;
    protected override BaseElement<ModIcon> BuildElement(ModIcon data) => new ModIconElement(data);
    protected override string GetDescription(ModIcon data) => data.DisplayName;
    protected override string GetTag(ModIcon data) => data.Tag;

    public override void Update(Microsoft.Xna.Framework.GameTime gt)
    {
        if (items.Count == 0)
            PopulatePanel();
        base.Update(gt);
    }
}
