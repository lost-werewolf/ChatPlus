using System;
using System.Collections.Generic;
using ChatPlus.Core.Chat;
using ChatPlus.Core.UI;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.UI;

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

    public override void LeftClick(UIMouseEvent evt)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].IsMouseHovering)
            {
                SetSelectedIndex(i);
                InsertSelectedTag();
                return;
            }
        }
    }

    public override void InsertSelectedTag()
    {
        if (items.Count == 0)
        {
            return;
        }

        if (currentIndex < 0)
        {
            return;
        }

        string insert = GetTag(items[currentIndex].Data);
        if (string.IsNullOrEmpty(insert))
        {
            return;
        }

        string text = Main.chatText ?? string.Empty;
        int caret = Math.Clamp(HandleChatSystem.GetCaretPos(), 0, text.Length);

        int atIndex = -1;
        if (caret > 0)
        {
            int probe = Math.Min(caret - 1, Math.Max(0, text.Length - 1));
            atIndex = text.LastIndexOf('@', probe);
        }

        bool outsideTags = false;
        if (atIndex >= 0)
        {
            int lastLb = text.LastIndexOf('[', atIndex);
            int lastRb = text.LastIndexOf(']', atIndex);
            outsideTags = lastLb <= lastRb;
        }

        if (atIndex >= 0 && outsideTags)
        {
            int start = atIndex;
            int stop = FindStop(text, start + 1);
            if (stop < 0 || stop > caret)
            {
                stop = caret;
            }

            string before = text.Substring(0, start);
            string after = text.Substring(stop);
            Main.chatText = before + insert + after;
            HandleChatSystem.SetCaretPos(before.Length + insert.Length);
            return;
        }

        int fragStart = text.LastIndexOf("[mention", StringComparison.OrdinalIgnoreCase);
        if (fragStart >= 0 && caret >= fragStart)
        {
            int closing = text.IndexOf(']', fragStart + 8);
            bool isOpen = closing == -1 || closing > caret;
            if (isOpen)
            {
                string before = text.Substring(0, fragStart);
                string after = text.Substring(caret);
                Main.chatText = before + insert + after;
                HandleChatSystem.SetCaretPos(before.Length + insert.Length);
                return;
            }
        }

        Main.chatText += insert;
        HandleChatSystem.SetCaretPos(Main.chatText.Length);

        static int FindStop(string s, int start)
        {
            if (start >= s.Length)
            {
                return -1;
            }

            char[] stops = [' ', '\t', '\n', '\r', ']', ',', '.', ':', ';', '!', '?'];
            return s.IndexOfAny(stops, start);
        }

        var sys = Terraria.ModLoader.ModContent.GetInstance<MentionSystem>();
        sys?.ui?.SetState(null);
    }
}
