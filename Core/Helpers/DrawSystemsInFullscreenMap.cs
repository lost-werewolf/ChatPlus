using ChatPlus.Core.Features.ModIcons;
using ChatPlus.Core.Features.PlayerIcons.PlayerInfo;
using ChatPlus.Core.Features.Scrollbar;
using ChatPlus.Core.Features.Uploads.UploadInfo;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace ChatPlus.Core.Helpers;

internal class DrawSystemsInFullscreenMap : ModSystem
{
    public static void Draw()
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

    public static void DrawTopMostSystems()
    {
        ModContent.GetInstance<TopMostPlayerInfoOverlaySystem>().Draw();
        ModContent.GetInstance<TopMostUploadInfoOverlaySystem>().Draw();
        ModContent.GetInstance<TopMostModInfoOverlaySystem>().Draw();
    }
}
