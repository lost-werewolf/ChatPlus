using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.UI;

namespace ChatPlus.Core.Features.Scrollbar.UI;
/// <summary>
/// A scrollable list. Paired with a <see cref="ChatScrollbarElement"/>
/// </summary>
public class ChatScrollList : UIElement
{
    private ChatScrollbarElement _scrollbar;
    private UIElement _innerList;
    // Track chat content state for scroll logic
    private int _lastTotalLines = 0;
    private bool _wasAtBottom = true;

    public ChatScrollList()
    {
        // Inner container (currently not holding individual message elements, but could)
        _innerList = new UIElement
        {
            Width = { Percent = 1f },   // match parent width
            Height = { Percent = 1f },
            OverflowHidden = false
        };
        Append(_innerList);

        // Position the scroll list overlay to cover the chat message area
        Left.Set(82f, 0f);
        Top.Set(Main.screenHeight - 247, 0f);
        Width.Set(Main.screenWidth - 300f, 0f);
        Height.Set(210f, 0f);
    }

    public float ViewPosition
    {
        get
        {
            return this._scrollbar.ViewPosition;
        }
        set
        {
            this._scrollbar.ViewPosition = value;
        }
    }

    public void Add(UIElement item)
    {
        this._innerList.Append(item);
        this._innerList.Recalculate();
    }

    public float GetTotalHeight()
    {
        return _innerList.Height.Pixels;
    }

    public void SetScrollbar(ChatScrollbarElement scrollbar)
    {
        _scrollbar = scrollbar;
        // Initialize scrollbar thumb size for the viewport (no content initially)
        _scrollbar.SetView(GetInnerDimensions().Height, 0f);
    }

    public override void ScrollWheel(UIScrollWheelEvent evt)
    {
        base.ScrollWheel(evt);
        if (_scrollbar == null) return;
        // Terraria: ScrollWheelValue > 0 means scroll up (to older messages)
        _scrollbar.ViewPosition -= evt.ScrollWheelValue;
        SyncChatMonitorToScroll();
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        if (_scrollbar == null) return;
        // Synchronize scroll every frame
        SyncChatMonitorToScroll();
        // If mouse over chat area, prevent vanilla scroll behavior
        if (IsMouseHovering)
        {
            PlayerInput.LockVanillaMouseScroll("ModLoader/UIList");
        }
    }

    private void SyncChatMonitorToScroll()
    {
        // 1. Determine total lines of chat text and visible lines
        int totalLines = ScrollHelper.GetTotalLineCount();
        if (totalLines < 0) totalLines = 0;
        float viewHeight = GetInnerDimensions().Height;
        float lineHeight = 21f;  // approximate height of a chat line in pixels
        // Try to get the actual number of lines the game is showing (ShowCount), otherwise calculate from height
        int showCount = TryGetMonitorShowCount(out int monitorShow)
                        ? monitorShow
                        : Math.Max(1, (int)Math.Floor(viewHeight / lineHeight));
        showCount = Math.Clamp(showCount, 1, totalLines > 0 ? totalLines : 1);

        // 2. Update scrollbar size based on content
        float contentHeight = totalLines * lineHeight;
        _scrollbar.SetView(viewHeight, contentHeight);

        // 3. Adjust scroll position if content size changed
        if (totalLines != _lastTotalLines)
        {
            if (totalLines > _lastTotalLines)
            {
                // New lines added
                if (_wasAtBottom)
                {
                    // If was at bottom, stay at bottom to show new messages
                    _scrollbar.GoToBottom();
                }
                // If user was scrolled up, do nothing (new lines remain out of view)
            }
            else if (totalLines < _lastTotalLines)
            {
                // Lines removed (chat cleared or trimmed) -> ensure we don't scroll past the end
                _scrollbar.ViewPosition = MathHelper.Clamp(
                    _scrollbar.ViewPosition, 0f, Math.Max(0f, contentHeight - viewHeight)
                );
            }
        }

        // 4. Update tracking state for next time
        _wasAtBottom = (_scrollbar.ViewPosition >= _scrollbar.MaxViewSize - _scrollbar.ViewSize - 1e-2);
        _lastTotalLines = totalLines;

        // 5. Set chat monitor offset (which lines to actually display)
        if (totalLines <= showCount)
        {
            // If all lines fit, always show the latest lines (no scrolling needed)
            SetMonitorStartChatLine(0, recalc: true);
        }
        else
        {
            // Calculate the index of the top visible line based on scroll position
            int topLineIndex = (int)Math.Floor(_scrollbar.GetValue() / lineHeight);
            if (topLineIndex < 0) topLineIndex = 0;
            // Visible line count (could use showCount directly)
            int visibleCount = showCount;
            if (visibleCount > totalLines) visibleCount = totalLines;
            // Compute startChatLine: at bottom, this should be 0; if scrolled up, startChatLine increases
            int startIndex = Math.Clamp(totalLines - visibleCount - topLineIndex,
                                        0, Math.Max(0, totalLines - visibleCount));
            SetMonitorStartChatLine(startIndex, recalc: true);
        }
    }

    private bool TryGetMonitorShowCount(out int showCount)
    {
        showCount = 0;
        var monitor = Main.chatMonitor;
        if (monitor == null) return false;
        Type monitorType = monitor.GetType();
        // Check if Terraria's chat monitor exposes ShowCount (or similar) 
        var prop = monitorType.GetProperty("ShowCount", BindingFlags.Instance | BindingFlags.Public);
        if (prop != null && prop.GetValue(monitor) is int value)
        {
            showCount = value;
            return true;
        }
        // Fallback to private field _showCount if available
        var field = monitorType.GetField("_showCount", BindingFlags.Instance | BindingFlags.NonPublic);
        if (field != null)
        {
            showCount = (int)field.GetValue(monitor);
            return true;
        }
        return false;
    }

    private void SetMonitorStartChatLine(int value, bool recalc)
    {
        var monitor = Main.chatMonitor;
        if (monitor == null) return;
        Type type = monitor.GetType();
        // Try public property first (e.g., Offset or StartChatLine)
        var prop = type.GetProperty("Offset", BindingFlags.Instance | BindingFlags.Public)
                   ?? type.GetProperty("StartChatLine", BindingFlags.Instance | BindingFlags.Public);
        if (prop != null && prop.CanWrite)
        {
            prop.SetValue(monitor, value);
        }
        else
        {
            // Otherwise, set private field _startChatLine
            var field = type.GetField("_startChatLine", BindingFlags.Instance | BindingFlags.NonPublic);
            field?.SetValue(monitor, value);
        }
        if (recalc)
        {
            // Flag the chat monitor to recalculate wrapping if needed
            var recalcField = type.GetField("_recalculateOnNextUpdate", BindingFlags.Instance | BindingFlags.NonPublic);
            recalcField?.SetValue(monitor, true);
        }
    }

    public void Clear()
    {
        // Remove any child elements (if we had added message UI elements in future)
        _innerList.RemoveAllChildren();
        // Reset scroll tracking
        _lastTotalLines = 0;
        _wasAtBottom = true;
        // Reset scrollbar view (content height = 0)
        _scrollbar?.SetView(GetInnerDimensions().Height, 0f);
    }
}
