using System;
using System.Collections.Generic;
using ChatPlus.Core.Chat;
using ChatPlus.Core.Features.Stats.Base;
using ChatPlus.Core.Features.Stats.PlayerStats;
using ChatPlus.Core.UI;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.UI;

namespace ChatPlus.Core.Features.PlayerIcons;

public class PlayerIconPanel : BasePanel<PlayerIcon>
{
    protected override IEnumerable<PlayerIcon> GetSource() => PlayerIconManager.PlayerIcons;
    protected override BaseElement<PlayerIcon> BuildElement(PlayerIcon data) => new PlayerIconElement(data);
    protected override string GetDescription(PlayerIcon data) => data.PlayerName;
    protected override string GetTag(PlayerIcon data) => data.Tag;

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

        // 1) Bare '@' segment outside tags: replace "@query"
        int atIndex = -1;
        if (caret > 0)
        {
            int probe = Math.Min(caret - 1, Math.Max(0, text.Length - 1));
            atIndex = text.LastIndexOf('@', probe);
        }

        if (atIndex >= 0)
        {
            int lastLb = text.LastIndexOf('[', atIndex);
            int lastRb = text.LastIndexOf(']', atIndex);
            bool outsideTags = lastLb <= lastRb;

            if (outsideTags)
            {
                int start = atIndex;
                int stop = FindStop(text, start + 1);
                if (stop < 0 || stop > caret)
                {
                    stop = caret;
                }

                string before = text.Substring(0, start);
                string after = text.Substring(stop);

                bool needSpace = NeedsLeadingSpace(before);
                string space = needSpace ? " " : string.Empty;

                Main.chatText = before + space + insert + after;
                HandleChatSystem.SetCaretPos(before.Length + space.Length + insert.Length);
                return;
            }
        }

        // 2) Open bracketed [p... fragment: replace it
        int pStart = text.LastIndexOf("[p", StringComparison.OrdinalIgnoreCase);
        if (pStart >= 0 && caret >= pStart)
        {
            int closing = text.IndexOf(']', pStart + 2);
            bool isOpen = closing == -1 || closing > caret;
            if (isOpen)
            {
                string before = text.Substring(0, pStart);
                string after = text.Substring(caret);

                bool needSpace = NeedsLeadingSpace(before);
                string space = needSpace ? " " : string.Empty;

                Main.chatText = before + space + insert + after;
                HandleChatSystem.SetCaretPos(before.Length + space.Length + insert.Length);
                return;
            }
        }

        // 3) Fallback: append with leading space if needed
        {
            string before = text;
            bool needSpace = NeedsLeadingSpace(before);
            string space = needSpace ? " " : string.Empty;

            Main.chatText = before + space + insert;
            HandleChatSystem.SetCaretPos(Main.chatText.Length);
        }

        static int FindStop(string s, int start)
        {
            if (start >= s.Length)
            {
                return -1;
            }

            char[] stops = [' ', '\t', '\n', '\r', ']', ',', '.', ':', ';', '!', '?'];
            return s.IndexOfAny(stops, start);
        }

        static bool NeedsLeadingSpace(string before)
        {
            if (string.IsNullOrEmpty(before))
            {
                return false;
            }

            char prev = before[before.Length - 1];
            if (char.IsWhiteSpace(prev))
            {
                return false;
            }

            // Avoid inserting a space right after an opening bracket at start of a tag
            if (prev == '[')
            {
                return false;
            }

            return true;
        }
    }

    public override void Update(GameTime gt)
    {
        // Refresh population each frame in case players join/leave
        if (items.Count != PlayerIconManager.PlayerIcons.Count)
            PopulatePanel();

        base.Update(gt);
    }

    public void OpenPlayerInfoForSelected()
    {
        if (!TryGetSelected(out var entry)) return;

        int who = entry.PlayerIndex;
        Player target = null;
        string name = null;
        if (who < 0 || who >= Main.maxPlayers || !Main.player[who].active)
        {
            // try by name
            name = entry.PlayerName ?? "";
            who = -1;
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                if (Main.player[i]?.active == true && string.Equals(Main.player[i].name, name, StringComparison.OrdinalIgnoreCase))
                { 
                    who = i;
                    target = Main.player[i];
                    break; 
                }
            }
            if (who < 0) { Main.NewText("Player not found.", Microsoft.Xna.Framework.Color.Orange); return; }
        }

        var s = PlayerInfoState.instance;
        if (s == null)
        {
            Main.NewText("Player info UI not available.", Microsoft.Xna.Framework.Color.Orange);
            return;
        }

        // 🔒 block if no access
        if (target != null && !PlayerInfoDrawer.HasAccess(Main.LocalPlayer, target))
        {
            //Main.NewText($"{target.name}'s stats is private.", Color.OrangeRed);
            //return;
        }

        // 2) Configure and open
        s.SetPlayer(who, Main.player[who]?.name);
        s.OpenForCurrentContext();
    }
}
