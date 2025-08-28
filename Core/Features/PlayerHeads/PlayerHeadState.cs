using ChatPlus.Core.UI;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;

namespace ChatPlus.Core.Features.PlayerHeads;

public class PlayerHeadState : UIState
{
    public PlayerHeadPanel Panel { get; }
    public DescriptionPanel<PlayerHead> Desc { get; }

    public PlayerHeadState()
    {
        Panel = new PlayerHeadPanel();
        Append(Panel);

        Desc = new DescriptionPanel<PlayerHead>(centerText: true);
        Append(Desc);

        Panel.ConnectedPanel = Desc;
        Desc.ConnectedPanel = Panel;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
    }
}
