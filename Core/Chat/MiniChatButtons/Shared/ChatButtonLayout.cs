using System.Collections.Generic;
using ChatPlus.Common.Configs;
using Terraria;

namespace ChatPlus.Core.Chat.MiniChatButtons.Shared;

public enum ChatButtonType
{
    Commands,
    Colors,
    Emojis,
    Glyphs,
    Items,
    ModIcons,
    Mentions,
    PlayerIcons,
    Uploads,
    Settings
}

public static class ChatButtonLayout
{
    // Fixed right-to-left order. ShowEmojiButton first keeps its current rightmost spot.
    private static readonly ChatButtonType[] OrderRightToLeft =
    [
        ChatButtonType.Emojis,
        ChatButtonType.Uploads,
        ChatButtonType.Mentions,
        ChatButtonType.Items,
        ChatButtonType.Glyphs,
        ChatButtonType.Colors,
        ChatButtonType.Commands,
        ChatButtonType.ModIcons,
        ChatButtonType.PlayerIcons,
        ChatButtonType.Settings,
    ];

    public static bool IsEnabled(ChatButtonType kind)
    {
        var s = Conf.C;
        if (s == null) return true; // default to visible when config not initialized

        return kind switch
        {
            ChatButtonType.Commands => s.ShowCommandButton,
            ChatButtonType.Colors => s.ShowColorButton,
            ChatButtonType.Emojis => s.ShowEmojiButton,
            ChatButtonType.Glyphs => s.ShowGlyphButton,
            ChatButtonType.Items => s.ShowItemButton,
            ChatButtonType.ModIcons => s.ShowModIconButton,
            ChatButtonType.Mentions => s.ShowMentionButton,
            ChatButtonType.PlayerIcons => s.ShowPlayerIconButton,
            ChatButtonType.Uploads => s.ShowUploadButton,
            ChatButtonType.Settings => s.ShowSettingsButton,
            _ => true
        };
    }

    public static int VisibleIndex(ChatButtonType type)
    {
        var enabled = new List<ChatButtonType>(OrderRightToLeft.Length);
        foreach (var k in OrderRightToLeft)
        {
            if (IsEnabled(k)) enabled.Add(k);
        }

        return enabled.IndexOf(type); // 0 = rightmost
    }

    public static Vector2 ComputeTopLeft(ChatButtonType type, int size = 24, int gap = 3)
    {
        int index = VisibleIndex(type);
        if (index < 0) return new Vector2(-10000, -10000);

        // Anchor to the same spot EmojiButton used for the rightmost button
        float baseX = Main.screenWidth - 300 + 24 + 21;
        float x = baseX - index * (size + gap+1);
        float y = Main.screenHeight - 59;
        return new Vector2(x, y);
    }
}
