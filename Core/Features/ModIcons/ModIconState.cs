using ChatPlus.Core.Features.ModIcons.ModInfo;
using ChatPlus.Core.Features.PlayerHeads.PlayerInfo;
using ChatPlus.Core.UI;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace ChatPlus.Core.Features.ModIcons;

public class ModIconState : BaseState<ModIcon>
{
    public ModIconState() : base(new ModIconPanel(), new DescriptionPanel<ModIcon>())
    {
    }

    private Mod hoveredMod;

    public override void Draw(SpriteBatch sb)
    {
        base.Draw(sb);
    }
}
