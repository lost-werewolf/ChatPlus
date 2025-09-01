using ChatPlus.Core.Features.PlayerHeads;
using ChatPlus.Core.UI;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

public class PlayerHeadState : BaseState<PlayerHead>
{
    public PlayerHeadState() : base(new PlayerHeadPanel(), new DescriptionPanel<PlayerHead>()) { }

    private Player hoveredPlayer;

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
    }
}
