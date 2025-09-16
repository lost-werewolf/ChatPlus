using System;
using System.Collections.Generic;
using ChatPlus.Core.Chat;
using ChatPlus.Core.UI;
using Terraria;

namespace ChatPlus.Common.Compat.CustomTags;

public class CustomTagPanel : BasePanel<CustomTag>
{
    protected override BaseElement<CustomTag> BuildElement(CustomTag data) => new CustomTagElement(data);

    protected override IEnumerable<CustomTag> GetSource()
    {
        // Dynamic provider mode
        if (CustomTagSystem.Providers.TryGetValue(prefix, out var provider))
        {
            string text = Main.chatText ?? string.Empty;
            int caret = Math.Clamp(HandleChatSystem.GetCaretPos(), 0, text.Length);

            string open = "[" + prefix;
            int startSearchIndex = Math.Max(0, caret - 1);
            int start = text.LastIndexOf(open, startSearchIndex, StringComparison.OrdinalIgnoreCase);
            if (start < 0)
                yield break;

            int bodyStart = start + open.Length;
            if (bodyStart < text.Length && text[bodyStart] == ':')
                bodyStart++;

            int end = text.IndexOf(']', bodyStart);
            if (end == -1 || end > caret)
                end = caret;

            string body = bodyStart < end ? text.Substring(bodyStart, end - bodyStart) : string.Empty;

            var items = provider(body);
            if (items == null)
                yield break;

            foreach (var (insert, view) in items)
            {
                yield return new CustomTag(prefix, insert, view);
            }
            yield break;
        }

        // Static list mode
        foreach (var t in CustomTagSystem.CustomTags)
        {
            if (string.Equals(t.tag, prefix, StringComparison.OrdinalIgnoreCase))
                yield return t;
        }
    }

    protected override string GetDescription(CustomTag data) => data.ActualTag;
    protected override string GetTag(CustomTag data) => data.ActualTag;

    private readonly string prefix;

    public CustomTagPanel(string prefix)
    {
        this.prefix = prefix;
    }

    public override void InsertSelectedTag()
    {
        if (items.Count == 0 || currentIndex < 0)
            return;

        var selected = items[currentIndex].Data;
        string fullTag = selected.ActualTag;
        if (string.IsNullOrEmpty(fullTag))
            return;

        string text = Main.chatText ?? string.Empty;
        int caret = Math.Clamp(HandleChatSystem.GetCaretPos(), 0, text.Length);

        string open = "[" + prefix;
        int startSearchIndex = Math.Max(0, caret - 1);
        int start = text.LastIndexOf(open, startSearchIndex, StringComparison.OrdinalIgnoreCase);

        if (start >= 0)
        {
            int afterPrefix = start + open.Length;
            if (afterPrefix < text.Length && text[afterPrefix] == ':')
                afterPrefix++;

            int endBracket = text.IndexOf(']', afterPrefix);
            int end = (endBracket >= 0 && endBracket <= caret) ? endBracket + 1 : caret;

            string before = text.Substring(0, start);
            string after = end < text.Length ? text.Substring(end) : string.Empty;

            Main.chatText = before + fullTag + after;
            HandleChatSystem.SetCaretPos(before.Length + fullTag.Length);
            return;
        }

        Main.chatText = text.Insert(caret, fullTag);
        HandleChatSystem.SetCaretPos(caret + fullTag.Length);
    }

    protected override bool MatchesFilter(CustomTag customTag)
    {
        // Providers already filter their own results
        if (CustomTagSystem.Providers.ContainsKey(prefix))
            return true;

        string text = Main.chatText ?? string.Empty;
        if (text.Length == 0)
            return true;

        int caret = Math.Clamp(HandleChatSystem.GetCaretPos(), 0, text.Length);
        string open = "[" + customTag.tag;
        int start = text.LastIndexOf(open, Math.Max(0, caret - 1));
        if (start < 0)
            return false;

        int queryStart = start + open.Length;
        if (queryStart < text.Length && text[queryStart] == ':')
            queryStart++;

        int end = text.IndexOf(']', queryStart);
        if (end == -1 || end > caret)
            end = caret;

        if (end <= queryStart)
            return true;

        string query = text.Substring(queryStart, end - queryStart);
        if (string.IsNullOrWhiteSpace(query))
            return true;

        return customTag.ActualTag.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0;
    }
}
