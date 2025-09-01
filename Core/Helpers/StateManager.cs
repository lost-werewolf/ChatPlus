using System;
using ChatPlus.Core.Features.Colors;
using ChatPlus.Core.Features.Commands;
using ChatPlus.Core.Features.Emojis;
using ChatPlus.Core.Features.Glyphs;
using ChatPlus.Core.Features.Items;
using ChatPlus.Core.Features.Links;
using ChatPlus.Core.Features.ModIcons;
using ChatPlus.Core.Features.PlayerHeads;
using ChatPlus.Core.Features.Uploads;
using ChatPlus.Core.UI;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace ChatPlus.Core.Helpers;

public static class StateManager
{
    public static void OpenStateIfPrefixMatches(GameTime gameTime, UserInterface ui, UIState state, string prefix)
    {
        if (!Main.drawingPlayerChat)
        {
            if (ui.CurrentState != null)
                ui.SetState(null);
            return;
        }
        string text = Main.chatText ?? string.Empty;

        int start = text.LastIndexOf(prefix, StringComparison.OrdinalIgnoreCase);
        if (start == -1)
        {
            if (ui.CurrentState == state)
                ui.SetState(null);
            return;
        }

        int end = text.IndexOf(']', start + prefix.Length);
        bool isClosed = end != -1;

        if (!isClosed)
        {
            if (ui.CurrentState != state)
            {
                CloseOthers(ui);     // ensure exclusivity
                ui.SetState(state);
            }
            ui.Update(gameTime);
        }
        else
        {
            if (ui.CurrentState == state)
                ui.SetState(null);
        }
    }

    public static bool IsAnyStateActive()
    {
        var cmdSys = ModContent.GetInstance<CommandSystem>();
        var colorSys = ModContent.GetInstance<ColorSystem>();
        var emojiSys = ModContent.GetInstance<EmojiSystem>();
        var glyphSys = ModContent.GetInstance<GlyphSystem>();
        var itemSys = ModContent.GetInstance<ItemSystem>();
        var modIconSys = ModContent.GetInstance<ModIconSystem>();
        var playerHeadSys = ModContent.GetInstance<PlayerHeadSystem>();
        var uploadSys = ModContent.GetInstance<UploadSystem>();
        var linkSys = ModContent.GetInstance<LinkSystem>();

        return cmdSys?.ui?.CurrentState != null ||
               colorSys?.ui?.CurrentState != null ||
               emojiSys?.ui?.CurrentState != null ||
               glyphSys?.ui?.CurrentState != null ||
               itemSys?.ui?.CurrentState != null ||
               modIconSys?.ui?.CurrentState != null ||
               playerHeadSys?.ui?.CurrentState != null ||
               uploadSys?.ui?.CurrentState != null ||
               linkSys?.ui?.CurrentState != null;
    }

    public static void CloseOthers(UserInterface keep)
    {
        void Close(UserInterface u)
        {
            if (u != null && u != keep && u.CurrentState != null)
                u.SetState(null);
        }

        var cmdSys = ModContent.GetInstance<CommandSystem>();
        var colorSys = ModContent.GetInstance<ColorSystem>();
        var emojiSys = ModContent.GetInstance<EmojiSystem>();
        var glyphSys = ModContent.GetInstance<GlyphSystem>();
        var itemSys = ModContent.GetInstance<ItemSystem>();
        var modIconSys = ModContent.GetInstance<ModIconSystem>();
        var playerHeadSys = ModContent.GetInstance<PlayerHeadSystem>();
        var uploadSys = ModContent.GetInstance<UploadSystem>();
        var linkSys = ModContent.GetInstance<LinkSystem>();

        Close(cmdSys?.ui);
        Close(colorSys?.ui);
        Close(emojiSys?.ui);
        Close(glyphSys?.ui);
        Close(itemSys?.ui);
        Close(modIconSys?.ui);
        Close(playerHeadSys?.ui);
        Close(uploadSys?.ui);
        Close(linkSys?.ui);
    }
}
