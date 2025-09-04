using System;
using System.Collections.Generic;
using ChatPlus.Core.Chat;
using ChatPlus.Core.UI;
using Microsoft.Xna.Framework;
using Terraria;

namespace ChatPlus.Core.Features.Links;

public class LinkPanel : BasePanel<LinkEntry>
{
    protected override IEnumerable<LinkEntry> GetSource() => LinkManager.Links;
    protected override BaseElement<LinkEntry> BuildElement(LinkEntry data) => new LinkElement(data);
    protected override string GetDescription(LinkEntry data) => data.Display + "\nClick to insert link";
    protected override string GetTag(LinkEntry data) => data.Tag;

    public override void InsertSelectedTag()
    {
        if (items.Count == 0 || currentIndex < 0) return;
        string tag = items[currentIndex].Data.Tag;
        if (string.IsNullOrEmpty(tag)) return;

        // strip "[l:...]" if present
        string plain = tag;
        if (plain.StartsWith("[l:") && plain.EndsWith("]"))
            plain = plain[3..^1];

        string text = Main.chatText ?? string.Empty;
        int caret = Math.Clamp(HandleChatSystem.GetCaretPos(), 0, text.Length);
        char[] stops = [' ', '\t', '\n', '\r', ']'];

        // If user started typing "[l" but didn’t close it, replace that
        int lStart = text.LastIndexOf("[l", StringComparison.OrdinalIgnoreCase);
        bool openL = lStart >= 0 && text.IndexOf(']', lStart + 2) == -1;
        if (openL)
        {
            int lEnd = text.IndexOfAny(stops, lStart);
            if (lEnd < 0) lEnd = text.Length;
            string before = text[..lStart];
            string after = text[lEnd..];
            Main.chatText = before + plain + after;
            HandleChatSystem.SetCaretPos(before.Length + plain.Length);
            return;
        }

        // Fallback: insert plain at caret
        string pre = text[..caret];
        string post = text[caret..];
        Main.chatText = pre + plain + post;
        HandleChatSystem.SetCaretPos(pre.Length + plain.Length);
    }

    public override void Update(GameTime gt)
    {
        base.Update(gt);
    }
}
