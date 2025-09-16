using System.Reflection;
using ChatPlus.Core.Features.Scrollbar;
using ChatPlus.Core.Features.Stats.Base;
using ChatPlus.Core.Features.Stats.ModStats;
using ChatPlus.Core.Features.Stats.PlayerStats;
using ChatPlus.Core.Features.Stats.UploadStats;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace ChatPlus.Core.Helpers;
public static class DrawSystemsInFullscreenMap
{
    public static void DrawInfoStatesTopMost()
    {

        var ui = Main.InGameUI;
        if (ui?.CurrentState is not BaseInfoState baseState)
            return;

        // restore sb
        Main.spriteBatch.End();
        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend,SamplerState.LinearClamp,DepthStencilState.None,Main.Rasterizer,null,Main.UIScaleMatrix);

        // Make sure layout is up to date, then draw on TOP of the map
        ui.Update(Main._drawInterfaceGameTime);
        ui.Draw(Main.spriteBatch, Main._drawInterfaceGameTime);

        // restore sb to SamplerStateForCursor
        Main.spriteBatch.End();
        Main.spriteBatch.Begin(SpriteSortMode.Deferred,BlendState.AlphaBlend,Main.SamplerStateForCursor,DepthStencilState.None,null,null,Main.UIScaleMatrix);
    }

    public static void DrawSystems()
    {
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

        ModContent.GetInstance<ChatScrollSystem>().Draw();
    }

    public static void DrawHoverInfoSystems()
    {
        ModContent.GetInstance<TopMostPlayerInfoOverlaySystem>().Draw();
        ModContent.GetInstance<TopMostUploadInfoOverlaySystem>().Draw();
        ModContent.GetInstance<TopMostModInfoOverlaySystem>().Draw();
    }
}
