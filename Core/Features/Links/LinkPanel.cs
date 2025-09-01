using System.Collections.Generic;
using ChatPlus.Core.UI;
using Microsoft.Xna.Framework;
using Terraria;

namespace ChatPlus.Core.Features.Links;

public class LinkPanel : BasePanel<LinkEntry>
{
    protected override IEnumerable<LinkEntry> GetSource() => LinkInitializer.Links;
    protected override BaseElement<LinkEntry> BuildElement(LinkEntry data) => new LinkElement(data);
    protected override string GetDescription(LinkEntry data) => data.Display + "\nClick to insert link";
    protected override string GetTag(LinkEntry data) => data.Tag;

    public override void Update(GameTime gt)
    {
        if (items.Count == 0)
            PopulatePanel();
        base.Update(gt);
    }
}
