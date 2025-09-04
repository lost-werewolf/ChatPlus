using ChatPlus.Core.Features.PlayerIcons
;
using ChatPlus.Core.UI;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

public class PlayerIconState : BaseState<PlayerIcon>
{
    public PlayerIconState() : base(new PlayerIconPanel(), new DescriptionPanel<PlayerIcon>()) { }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
    }
}
