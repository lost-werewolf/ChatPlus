using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace ChatPlus.Core.Features.PlayerHeads.PlayerInfo;
public static class PlayerInfoDrawer
{
    /// <summary>
    /// Draws a tooltip with the full player along with an arrow drawn
    /// Mimics bestiary UI
    /// </summary>
    public static void Draw(SpriteBatch sb, Player player)
    {
        Rectangle rect = new(0, 0, 0, 0);
        Utils.DrawInvBG(sb, rect);

        // Draw header

        // Draw player and background


        // Four panels:
        // Draw health top left (100/100)
        // Draw mana top right (20/20)
        // Draw defense bottom left 
        // Draw current hotbar item slot bottom right
        
        // Extra:
        // red team, death count since he joined
        // ammo, coins,
        // armor/accessories/equipment/dyes
    }
}
