using System.Collections.Generic;
using ChatPlus.Core.UI;

namespace ChatPlus.Core.Features.PlayerHeads;

public class PlayerHeadPanel : BasePanel<PlayerHead>
{
    protected override IEnumerable<PlayerHead> GetSource() => PlayerHeadInitializer.PlayerIcons;
    protected override BaseElement<PlayerHead> BuildElement(PlayerHead data) => new PlayerHeadElement(data);
    protected override string GetDescription(PlayerHead data) => data.PlayerName;
    protected override string GetTag(PlayerHead data) => data.Tag;

    public override void Update(Microsoft.Xna.Framework.GameTime gt)
    {
        // Refresh population each frame in case players join/leave
        if (items.Count != PlayerHeadInitializer.PlayerIcons.Count)
            PopulatePanel();

        base.Update(gt);
    }
}
