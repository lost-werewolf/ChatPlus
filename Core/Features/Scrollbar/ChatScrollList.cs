using System;
using System.Collections;
using System.Reflection;
using ChatPlus.Common.Configs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameInput;
using Terraria.UI;

namespace ChatPlus.Core.Features.Scrollbar;

/// <summary>
/// A scroll list paired with a <see cref="ChatScrollbar"/>
/// </summary>
public class ChatScrollList : UIElement
{
    private ChatScrollbar scrollbar;
    private int lastTotalLines;

    public ChatScrollList()
    {
        Left.Set(82f, 0f);
        Top.Set(Main.screenHeight - 247, 0f);
        Width.Set(Main.screenWidth - 300f, 0f);
        Height.Set(210f, 0f);
    }

    public float ViewPosition
    {
        get => scrollbar.ViewPosition;
        set => scrollbar.ViewPosition = value;
    }

    public void SetScrollbar(ChatScrollbar s)
    {
        scrollbar = s;
        scrollbar.SetView(GetInnerDimensions().Height, 0f);
    }

    public override void ScrollWheel(UIScrollWheelEvent evt)
    {
        if (scrollbar == null) return;
        PlayerInput.LockVanillaMouseScroll("ChatPlus/ChatScroll");
        const float line = 21f; const float linesPerNotch = 3f;
        float notches = evt.ScrollWheelValue / 120f;
        ViewPosition -= notches * linesPerNotch * line;

        int total = GetTotalLines();
        float viewH = GetInnerDimensions().Height;
        int show = GetShowCount() ?? (int)(viewH / line); if (show < 1) show = 1; if (show > total) show = total;

        int topIndex = (int)Math.Floor(ViewPosition / line); if (topIndex < 0) topIndex = 0;
        int maxTop = Math.Max(0, total - show); if (topIndex > maxTop) topIndex = maxTop;

        int startIndex = Math.Max(0, total - show - topIndex);
        int cur = GetStartChatLine(); if (startIndex != cur) SetStartChatLine(startIndex);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        if (scrollbar == null) return;

        // dynamic sizing from monitor show count
        int show = Math.Clamp(GetShowCount() ?? 10, 10, 20);
        const float line = 21f;
        float h = show * line;
        float top = Main.screenHeight - (h + 37f);

        Top.Set(top, 0f);
        Width.Set(Main.screenWidth - 300f, 0f);
        Height.Set(h, 0f);

        Sync();

        if (IsMouseHovering) PlayerInput.LockVanillaMouseScroll("ChatPlus/ChatScroll");
    }

    public override void Draw(SpriteBatch sb)
    {
        base.Draw(sb);
    }

    private void Sync()
    {
        int total = GetTotalLines(); float lineHeight = 21f; float viewHeight = GetInnerDimensions().Height;
        int show = GetShowCount() ?? Math.Max(1, (int)(viewHeight / lineHeight)); show = Math.Clamp(show, 1, Math.Max(1, total));

        float contentHeight = total * lineHeight; scrollbar.SetView(viewHeight, contentHeight);

        if (total != lastTotalLines)
        {
            if (total > lastTotalLines && GetStartChatLine() == 0) scrollbar.GoToBottom();
            ViewPosition = MathHelper.Clamp(ViewPosition, 0f, Math.Max(0f, contentHeight - viewHeight));
        }

        bool mouseActive = (scrollbar?.IsDragging == true) || (IsMouseHovering && PlayerInput.ScrollWheelDeltaForUI != 0);
        bool followMonitor = !mouseActive;

        if (followMonitor)
        {
            int monitorStart = GetStartChatLine();
            int topIndex = Math.Clamp(total - show - monitorStart, 0, Math.Max(0, total - show));
            ViewPosition = topIndex * lineHeight;
        }
        else
        {
            int topIndex = (int)Math.Floor(ViewPosition / lineHeight);
            int startIndex = Math.Clamp(total - show - topIndex, 0, Math.Max(0, total - show));
            if (startIndex != GetStartChatLine()) 
                SetStartChatLine(startIndex);
        }

        lastTotalLines = total;
    }

    #region Helpers
    public static int GetTotalLines()
    {
        var monitorType = Main.chatMonitor.GetType();
        var messagesField = monitorType.GetField("_messages", BindingFlags.Instance | BindingFlags.NonPublic);
        var list = messagesField?.GetValue(Main.chatMonitor) as IEnumerable;

        var containerType = typeof(Main).Assembly.GetType("Terraria.UI.Chat.ChatMessageContainer");
        var lineProp = containerType?.GetProperty("LineCount", BindingFlags.Instance | BindingFlags.Public);

        int total = 0;
        if (list != null)
        {
            foreach (var msg in list)
            {
                if (msg == null) continue;
                try { total += Math.Max(1, (int)lineProp.GetValue(msg)); }
                catch { total++; }
            }
        }
        return total;
    }

    private static int? GetShowCount()
    {
        var showCount = Main.chatMonitor?.GetType().GetField("_showCount", BindingFlags.Instance | BindingFlags.NonPublic);
        return showCount != null ? (int)showCount.GetValue(Main.chatMonitor) : null;
    }

    private static int GetStartChatLine()
    {
        var startChatLine = Main.chatMonitor?.GetType().GetField("_startChatLine", BindingFlags.Instance | BindingFlags.NonPublic);
        return startChatLine != null ? (int)startChatLine.GetValue(Main.chatMonitor) : 0;
    }

    private static void SetStartChatLine(int value)
    {
        var t = Main.chatMonitor?.GetType(); if (t == null) return;
        var f = t.GetField("_startChatLine", BindingFlags.Instance | BindingFlags.NonPublic); if (f == null) return;
        int cur = (int)f.GetValue(Main.chatMonitor); if (cur == value) return;
        f.SetValue(Main.chatMonitor, value); 
        // do NOT touch _recalculateOnNextUpdate here
    }

    public void Clear()
    {
        lastTotalLines = 0;
        scrollbar?.SetView(GetInnerDimensions().Height, 0f);
    }
    #endregion
}
