using ChatPlus.Core.UI;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;

namespace ChatPlus.Core.Features.PlayerHeads;

public class PlayerHeadState : BaseState<PlayerHead>
{
    public PlayerHeadState() : base(new PlayerHeadPanel(), new DescriptionPanel<PlayerHead>())
    {
    }
}
