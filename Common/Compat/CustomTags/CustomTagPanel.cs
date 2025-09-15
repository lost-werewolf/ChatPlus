using System;
using System.Collections.Generic;
using System.Linq;
using ChatPlus.Core.Chat;
using ChatPlus.Core.Features.Emojis;
using ChatPlus.Core.Helpers;
using ChatPlus.Core.UI;
using Terraria;

namespace ChatPlus.Common.Compat.CustomTags;

public class CustomTagPanel : BasePanel<CustomTag>
{
    protected override BaseElement<CustomTag> BuildElement(CustomTag data) => new CustomTagElement(data);
    protected override IEnumerable<CustomTag> GetSource()
    {
        // Show only tags that belong to this panel’s prefix (e.g., "t" or "r")
        foreach (var t in CustomTagSystem.CustomTags)
        {
            if (string.Equals(t.tag, prefix, StringComparison.OrdinalIgnoreCase))
            {
                yield return t;
            }
        }
    }

    protected override string GetDescription(CustomTag data) => data.ActualTag;
    protected override string GetTag(CustomTag data) => data.ActualTag;

    private readonly string prefix; // "t", "r", ...

    public CustomTagPanel(string prefix)
    {
        this.prefix = prefix;
    }

    public override void InsertSelectedTag()
    {
        if (items.Count == 0)
            return;

        if (currentIndex < 0)
            return;

        var selected = items[currentIndex].Data;
        string fullTag = selected.ActualTag;
        if (string.IsNullOrEmpty(fullTag))
            return;

        string text = Main.chatText ?? string.Empty;
        int caret = Math.Clamp(HandleChatSystem.GetCaretPos(), 0, text.Length);

        // Match "[" + prefix with optional colon and replace the whole open slice.
        string open = "[" + this.prefix;

        int startSearchIndex = Math.Max(0, caret - 1);
        int start = text.LastIndexOf(open, startSearchIndex, StringComparison.OrdinalIgnoreCase);

        if (start >= 0)
        {
            int afterPrefix = start + open.Length;

            // Optional colon
            if (afterPrefix < text.Length)
            {
                if (text[afterPrefix] == ':')
                {
                    afterPrefix++;
                }
            }

            // If there's a closing bracket before or at caret, include it.
            int endBracket = text.IndexOf(']', afterPrefix);
            int end;
            if (endBracket >= 0 && endBracket <= caret)
            {
                end = endBracket + 1;
            }
            else
            {
                end = caret;
            }

            string before = text.Substring(0, start);
            string after;
            if (end < text.Length)
            {
                after = text.Substring(end);
            }
            else
            {
                after = string.Empty;
            }

            Main.chatText = before + fullTag + after;
            HandleChatSystem.SetCaretPos(before.Length + fullTag.Length);
            return;
        }

        // Fallback: insert at caret
        Main.chatText = text.Insert(caret, fullTag);
        HandleChatSystem.SetCaretPos(caret + fullTag.Length);
    }

    protected override bool MatchesFilter(CustomTag customTag)
    {
        string text = Main.chatText ?? string.Empty;
        if (text.Length == 0)
            return true;

        int caret = Math.Clamp(HandleChatSystem.GetCaretPos(), 0, text.Length);

        // Match [tag]
        string open = "[" + customTag.tag;
        int start = text.LastIndexOf(open, Math.Max(0, caret - 1));
        if (start < 0)
            return false;

        // Position after prefix and optional colon
        int queryStart = start + open.Length;
        if (queryStart < text.Length && text[queryStart] == ':')
            queryStart++;

        int end = text.IndexOf(']', queryStart);
        if (end == -1 || end > caret)
            end = caret;

        if (end <= queryStart)
            return true; // nothing typed yet → show all

        string query = text.Substring(queryStart, end - queryStart);
        if (string.IsNullOrWhiteSpace(query))
            return true;

        return customTag.ActualTag.IndexOf(query) >= 0;
    }


}
