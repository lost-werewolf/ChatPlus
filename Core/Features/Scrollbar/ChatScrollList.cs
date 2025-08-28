using System;
using System.Collections;
using System.Reflection;
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
        // Terraria uses 120 per notch; scroll a few lines per notch.
        const float linesPerNotch = 3f;
        ViewPosition -= (evt.ScrollWheelValue / 120f) * (linesPerNotch * 21f);
        Sync();
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        if (scrollbar == null) return;
        Top.Set(Main.screenHeight - 247, 0f);
        Width.Set(Main.screenWidth - 300f, 0f);

        Sync();

        if (IsMouseHovering)
            PlayerInput.LockVanillaMouseScroll("ChatPlus/ChatScroll");
    }

    public override void Draw(SpriteBatch sb)
    {
        base.Draw(sb);
    }

    private void Sync()
    {
        int total = GetTotalLines();
        float lineHeight = 21f;
        float viewHeight = GetInnerDimensions().Height;
        int show = GetShowCount() ?? Math.Max(1, (int)(viewHeight / lineHeight));
        show = Math.Clamp(show, 1, Math.Max(1, total));

        float contentHeight = total * lineHeight;
        scrollbar.SetView(viewHeight, contentHeight);

        if (total != lastTotalLines)
        {
            // stay at bottom only if we were already there
            if (total > lastTotalLines && (GetStartChatLine() == 0))
                scrollbar.GoToBottom();

            ViewPosition = MathHelper.Clamp(ViewPosition, 0f, Math.Max(0f, contentHeight - viewHeight));
        }

        // Only drive vanilla when mouse is actually controlling (dragging or scrolled this frame while hovering)
        bool mouseActive = (scrollbar?.IsDragging == true) ||
                           (IsMouseHovering && PlayerInput.ScrollWheelDeltaForUI != 0);
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
        var startChatLine = Main.chatMonitor?.GetType().GetField("_startChatLine", BindingFlags.Instance | BindingFlags.NonPublic);
        startChatLine?.SetValue(Main.chatMonitor, value);

        // Set recalculate to true to force update
        var recalculate = Main.chatMonitor?.GetType()?.GetField("_recalculateOnNextUpdate", BindingFlags.Instance | BindingFlags.NonPublic);
        recalculate?.SetValue(Main.chatMonitor, true);
    }

    public void Clear()
    {
        lastTotalLines = 0;
        scrollbar?.SetView(GetInnerDimensions().Height, 0f);
    }
    #endregion
}
