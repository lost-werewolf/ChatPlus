using System;
using ChatPlus.Core.Chat;
using Terraria;

namespace ChatPlus.Core.Features.ModIcons.ModInfo;

public static class ChatSession
{
    public readonly struct Snapshot
    {
        public readonly bool ChatOpen;
        public readonly string Text;
        public readonly int Caret;
        public readonly (int start, int end)? Selection;

        public Snapshot(bool chatOpen, string text, int caret, (int, int)? sel)
        {
            ChatOpen = chatOpen;
            Text = text ?? string.Empty;
            Caret = caret;
            Selection = sel;
        }
    }

    public static Snapshot Capture()
        => new Snapshot(
            Main.drawingPlayerChat,
            Main.chatText,
            HandleChatSystem.GetCaretPos(),
            HandleChatSystem.GetSelection()
        );

    public static void Restore(Snapshot s)
    {
        Main.drawingPlayerChat = s.ChatOpen;
        Main.chatText = s.Text ?? string.Empty;
        HandleChatSystem.SetCaretPos(Math.Clamp(s.Caret, 0, Main.chatText.Length));
        if (s.Selection.HasValue)
        {
            var (start, end) = s.Selection.Value;
            //HandleChatSystem.SetSelection(Math.Clamp(start, 0, Main.chatText.Length),
                                          //Math.Clamp(end, 0, Main.chatText.Length));
        }
    }
}
