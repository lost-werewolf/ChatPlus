using ChatPlus.Core.UI;
using Microsoft.Xna.Framework.Graphics;

namespace ChatPlus.Core.Features.ModIcons;

public class ModIconState : BaseState<ModIcon>
{
    public ModIconState() : base(new ModIconPanel(), new DescriptionPanel<ModIcon>())
    {
    }

    public override void Draw(SpriteBatch sb)
    {
        base.Draw(sb);
    }
}
