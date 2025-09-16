using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace ChatPlus.Core.Helpers;
internal class DrawSystemsInFullscreenMap : ModSystem
{
    public override void PostDrawFullscreenMap(ref string mouseText)
    {
        base.PostDrawFullscreenMap(ref mouseText);
        //return;

        // restart SB
        Main.spriteBatch.End();
        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.UIScaleMatrix);

        var sm = ChatPlus.StateManager;
        sm.CommandSystem?.ui?.Draw(Main.spriteBatch, Main._drawInterfaceGameTime);
        sm.ColorSystem?.ui?.Draw(Main.spriteBatch, Main._drawInterfaceGameTime);
        sm.CustomTagSystem?.ui?.Draw(Main.spriteBatch, Main._drawInterfaceGameTime);
        sm.EmojiSystem?.ui?.Draw(Main.spriteBatch, Main._drawInterfaceGameTime);
        sm.GlyphSystem?.ui?.Draw(Main.spriteBatch, Main._drawInterfaceGameTime);
        sm.ItemSystem?.ui?.Draw(Main.spriteBatch, Main._drawInterfaceGameTime);
        sm.ModIconSystem?.ui?.Draw(Main.spriteBatch, Main._drawInterfaceGameTime);
        sm.MentionSystem?.ui?.Draw(Main.spriteBatch, Main._drawInterfaceGameTime);
        sm.PlayerIconSystem?.ui?.Draw(Main.spriteBatch, Main._drawInterfaceGameTime);
        sm.UploadSystem?.ui?.Draw(Main.spriteBatch, Main._drawInterfaceGameTime);

        // restart SB
        Main.spriteBatch.End();
        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);
    }

}
