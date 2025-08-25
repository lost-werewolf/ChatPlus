using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;

namespace ChatPlus.Core.Features.ModIcons;

public class ModIconState : UIState
{
    public ModIconPanel panel;
    public DescriptionPanel<ModIcon> desc;

    public ModIconState()
    {
        panel = new();
        Append(panel);

        desc = new("Mod icon");
        Append(desc);

        panel.ConnectedPanel = desc;
        desc.ConnectedPanel = panel;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
    }
}
