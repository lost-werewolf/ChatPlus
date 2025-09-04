using System;
using System.Collections.Generic;
using ChatPlus.Core.Chat;
using ChatPlus.Core.UI;
using Terraria;

namespace ChatPlus.Core.Features.Mentions;

public class MentionPanel : BasePanel<Mention>
{
    protected override BaseElement<Mention> BuildElement(Mention data)
    {
        return new MentionElement(data);
    }

    protected override string GetDescription(Mention data)
    {
        return data.Tag;
    }

    protected override IEnumerable<Mention> GetSource()
    {
        for (int i = 0; i < Main.maxPlayers; i++)
        {
            var p = Main.player[i];
            if (p?.active == true && !string.IsNullOrWhiteSpace(p.name))
                yield return new Mention(p.name);
        }
    }

    protected override string GetTag(Mention data)
    {
        return "@" + data.Tag + " ";
    }

    public override void InsertSelectedTag()
    {
        if (items.Count == 0 || currentIndex < 0) return;

        string name = items[currentIndex].Data.Tag;
        if (string.IsNullOrWhiteSpace(name)) return;

        string mention = "@" + name + " "; // what we actually want to insert

        string text = Main.chatText ?? string.Empty;
        int caret = Math.Clamp(HandleChatSystem.GetCaretPos(), 0, text.Length);

        // Replace the nearest @word before the caret if present
        int searchStart = Math.Min(caret - 1, text.Length - 1);
        int at = searchStart >= 0 ? text.LastIndexOf('@', searchStart) : -1;
        if (at >= 0)
        {
            int end = at + 1;
            while (end < text.Length && !char.IsWhiteSpace(text[end])) end++;

            string before = text[..at];
            string after = text[end..];

            Main.chatText = before + mention + after;
            HandleChatSystem.SetCaretPos(before.Length + mention.Length);
            return;
        }

        // No '@' token → insert at caret (optionally add a space before)
        bool needSpaceBefore = caret > 0 && !char.IsWhiteSpace(text[caret - 1]);
        string pre = text[..caret];
        string post = text[caret..];

        Main.chatText = pre + (needSpaceBefore ? " " : "") + mention + post;
        HandleChatSystem.SetCaretPos(pre.Length + (needSpaceBefore ? 1 : 0) + mention.Length);
    }
}