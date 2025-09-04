using System;
using System.Collections.Generic;
using ChatPlus.Core.Chat;
using ChatPlus.Core.UI;
using Microsoft.Xna.Framework;
using Terraria;

namespace ChatPlus.Core.Features.Emojis
{
    public class EmojiPanel : BasePanel<Emoji>
    {
        protected override BaseElement<Emoji> BuildElement(Emoji data) => new EmojiElement(data);

        protected override IEnumerable<Emoji> GetSource() => EmojiManager.Emojis;

        protected override string GetDescription(Emoji data) => data.Description;

        protected override string GetTag(Emoji data) => data.Tag;

        protected override bool MatchesFilter(Emoji data)
        {
            string text = Main.chatText ?? string.Empty;
            int caret = Math.Clamp(HandleChatSystem.GetCaretPos(), 0, text.Length);

            int colon = -1;
            if (caret > 0)
            {
                colon = text.LastIndexOf(':', caret - 1);
            }

            bool colonMode = false;
            if (colon >= 0)
            {
                int lb = text.LastIndexOf('[', colon);
                int rb = text.LastIndexOf(']', colon);
                colonMode = lb <= rb;
            }

            string query = null;

            if (colonMode)
            {
                int qStart = colon + 1;
                int qLen = caret - qStart;
                if (qLen < 0) qLen = 0;

                if (qLen > 0)
                {
                    query = text.Substring(qStart, qLen);
                }
                else
                {
                    query = string.Empty;
                }
            }
            else
            {
                int eStart = text.LastIndexOf("[e", StringComparison.OrdinalIgnoreCase);
                bool eOpen = false;
                if (eStart >= 0)
                {
                    if (caret >= eStart)
                    {
                        int closing = text.IndexOf(']', eStart + 2);
                        eOpen = closing == -1;
                    }
                }

                if (eOpen)
                {
                    int qStart = eStart + 2;
                    if (qStart < text.Length)
                    {
                        char ch = text[qStart];
                        if (ch == ':' || ch == '/')
                        {
                            qStart++;
                        }
                    }

                    int qLen = caret - qStart;
                    if (qLen < 0) qLen = 0;

                    if (qLen > 0)
                    {
                        query = text.Substring(qStart, qLen).Trim();
                    }
                    else
                    {
                        query = string.Empty;
                    }
                }
            }

            if (EmojiSystem.IsForceOpen && string.IsNullOrEmpty(query))
            {
                return true;
            }

            if (query == null || query.Length == 0)
            {
                return true;
            }

            // Compare against the plain key (from Tag) and Description + Synonyms.
            string key = ExtractKeyFromTag(data.Tag);
            if (string.IsNullOrEmpty(key))
            {
                key = data.Description;
            }

            if (!string.IsNullOrEmpty(key))
            {
                if (key.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return true;
                }
            }

            if (data.Synonyms != null)
            {
                foreach (string syn in data.Synonyms)
                {
                    if (!string.IsNullOrEmpty(syn))
                    {
                        if (syn.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;


            static string ExtractKeyFromTag(string tag)
            {
                if (string.IsNullOrEmpty(tag)) return null;

                int c = tag.IndexOf(':');
                int r = tag.LastIndexOf(']');
                if (c >= 0 && r > c)
                {
                    return tag.Substring(c + 1, r - c - 1);
                }

                if (tag.StartsWith("[") && tag.EndsWith("]"))
                {
                    return tag.Substring(1, tag.Length - 2);
                }

                return tag;
            }
        }
        public override void InsertSelectedTag()
        {
            if (items.Count == 0) return;
            if (currentIndex < 0) return;

            // BasePanel<Emoji> already gives you a strongly-typed element.
            Emoji emoji = items[currentIndex].Data;

            string insert = ComputeInsert(emoji);
            if (string.IsNullOrEmpty(insert)) return;

            string text = Main.chatText ?? string.Empty;
            int caret = Math.Clamp(HandleChatSystem.GetCaretPos(), 0, text.Length);

            // Prefer replacing from the nearest ':' token (if outside any tag)
            int colon = -1;
            if (caret > 0)
            {
                colon = text.LastIndexOf(':', caret - 1);
            }

            bool colonMode = false;
            if (colon >= 0)
            {
                int lb = text.LastIndexOf('[', colon);
                int rb = text.LastIndexOf(']', colon);
                colonMode = lb <= rb; // true if outside a [...] tag
            }

            if (colonMode)
            {
                string before = text.Substring(0, colon);
                string after = text.Substring(caret);
                Main.chatText = before + insert + after;
                HandleChatSystem.SetCaretPos(before.Length + insert.Length);
                EmojiSystem.CloseAfterCommit();
                return;
            }

            // Otherwise if we're in an open "[e" context, replace from that start
            int eStart = text.LastIndexOf("[e", StringComparison.OrdinalIgnoreCase);
            bool eOpen = false;
            if (eStart >= 0)
            {
                if (caret >= eStart)
                {
                    int closing = text.IndexOf(']', eStart + 2);
                    eOpen = closing == -1;
                }
            }

            if (eOpen)
            {
                string before = text.Substring(0, eStart);
                string after = text.Substring(caret);
                Main.chatText = before + insert + after;
                HandleChatSystem.SetCaretPos(before.Length + insert.Length);
                EmojiSystem.CloseAfterCommit();
                return;
            }

            // Fallback: append
            Main.chatText += insert;
            HandleChatSystem.SetCaretPos(Main.chatText.Length);
            EmojiSystem.CloseAfterCommit();


            static string ComputeInsert(in Emoji e)
            {
                // If Tag already looks like [e:...], use it as-is.
                if (!string.IsNullOrEmpty(e.Tag))
                {
                    string t = e.Tag.Trim();
                    if (t.StartsWith("[e", StringComparison.OrdinalIgnoreCase) && t.EndsWith("]"))
                    {
                        return t;
                    }

                    string keyFromTag = ExtractKeyFromTag(t);
                    if (!string.IsNullOrEmpty(keyFromTag))
                    {
                        return "[e:" + keyFromTag + "]";
                    }
                }

                // Otherwise build from Description.
                if (!string.IsNullOrEmpty(e.Description))
                {
                    string key = NormalizeKey(e.Description);
                    if (!string.IsNullOrEmpty(key))
                    {
                        return "[e:" + key + "]";
                    }
                }

                return null;
            }

            static string ExtractKeyFromTag(string tag)
            {
                if (string.IsNullOrEmpty(tag)) return null;

                int c = tag.IndexOf(':');
                int r = tag.LastIndexOf(']');
                if (c >= 0 && r > c)
                {
                    return tag.Substring(c + 1, r - c - 1);
                }

                // If someone stored "[e:smile" or "smile]" or just "smile", do a best effort.
                if (tag.StartsWith("[e", StringComparison.OrdinalIgnoreCase))
                {
                    int colon = tag.IndexOf(':');
                    if (colon >= 0 && colon + 1 < tag.Length)
                    {
                        string tail = tag.Substring(colon + 1);
                        if (tail.EndsWith("]"))
                        {
                            tail = tail.Substring(0, tail.Length - 1);
                        }
                        return tail.Trim();
                    }
                }

                if (tag.StartsWith("[") && tag.EndsWith("]"))
                {
                    return tag.Substring(1, tag.Length - 2).Trim();
                }

                return tag.Trim();
            }

            static string NormalizeKey(string s)
            {
                if (string.IsNullOrWhiteSpace(s)) return null;

                s = s.Trim().ToLowerInvariant();

                // Convert spaces and dashes to underscores; collapse double underscores.
                var chars = new List<char>(s.Length);
                bool lastUnderscore = false;

                for (int i = 0; i < s.Length; i++)
                {
                    char ch = s[i];

                    if (char.IsWhiteSpace(ch) || ch == '-')
                    {
                        if (!lastUnderscore)
                        {
                            chars.Add('_');
                            lastUnderscore = true;
                        }
                        continue;
                    }

                    lastUnderscore = false;
                    chars.Add(ch);
                }

                return new string(chars.ToArray());
            }
        }

        public override void Update(GameTime gt)
        {
            base.Update(gt);
        }
    }
}
