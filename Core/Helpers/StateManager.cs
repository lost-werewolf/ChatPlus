using ChatPlus.Core.Features.Colors;
using ChatPlus.Core.Features.Commands;
using ChatPlus.Core.Features.Emojis;
using ChatPlus.Core.Features.Glyphs;
using ChatPlus.Core.Features.Items;
using ChatPlus.Core.Features.ModIcons;
using ChatPlus.Core.Features.Uploads;
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
            if (ui?.CurrentState != null)
            {
                ui.SetState(null);
            }
            return;
        }

        string text = Main.chatText ?? "";

        // Look for last occurrence of the prefix
        int start = text.LastIndexOf(prefix);
        if (start == -1)
        {
            // No active prefix → close
            if (ui?.CurrentState == state)
                ui.SetState(null);
            return;
        }

        // Check if this prefix is already "closed" with a ']'
        int end = text.IndexOf(']', start + prefix.Length);
        bool isClosed = end != -1;

        if (!isClosed)
        {
            // Still typing an open prefix → keep state open
            if (ui.CurrentState != state)
                ui.SetState(state);

            ui.Update(gameTime);
        }
        else
        {
            // Found a closing bracket → close state
            if (ui?.CurrentState == state)
                ui?.SetState(null);
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
        var uploadSys = ModContent.GetInstance<UploadSystem>();

        return cmdSys?.ui?.CurrentState != null ||
               colorSys?.ui?.CurrentState != null ||
               emojiSys?.ui?.CurrentState != null ||
               glyphSys?.ui?.CurrentState != null ||
               itemSys?.ui?.CurrentState != null ||
               modIconSys?.ui?.CurrentState != null ||
               uploadSys?.ui?.CurrentState != null;
    }
}
