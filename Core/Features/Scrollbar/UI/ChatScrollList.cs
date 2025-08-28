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
public class ChatScrollList : UIElement, IEnumerable<UIElement>, IEnumerable
{
    public delegate bool ElementSearchMethod(UIElement element);

    public class UIInnerList : UIElement
    {
        public override bool ContainsPoint(Vector2 point)
        {
            return true;
        }

        protected override void DrawChildren(SpriteBatch spriteBatch)
        {
            Vector2 position = Parent.GetDimensions().Position();
            Vector2 dimensions = new Vector2(Parent.GetDimensions().Width, Parent.GetDimensions().Height);
            foreach (UIElement element in Elements)
            {
                Vector2 position2 = element.GetDimensions().Position();
                Vector2 dimensions2 = new Vector2(element.GetDimensions().Width, element.GetDimensions().Height);
                if (Collision.CheckAABBvAABBCollision(position, dimensions, position2, dimensions2))
                {
                    element.Draw(spriteBatch);
                }
            }
        }

        public override Rectangle GetViewCullingArea()
        {
            return Parent.GetDimensions().ToRectangle();
        }
    }

    public List<UIElement> _items = new List<UIElement>();

    public ChatScrollbarElement _scrollbar;

    public UIElement _innerList = new UIInnerList();

    public float _innerListHeight;

    public float ListPadding = 0f;

    public Action<List<UIElement>> ManualSortMethod;

    public int Count => _items.Count;

    public float ViewPosition
    {
        get
        {
            return _scrollbar.ViewPosition;
        }
        set
        {
            _scrollbar.ViewPosition = value;
        }
    }

    public ChatScrollList()
    {
        _innerList.OverflowHidden = false;
        _innerList.Width.Set(0f, 1f);
        _innerList.Height.Set(0f, 1f);
        OverflowHidden = false;
        Append(_innerList);

        // Set Chat Dimensions!
        //Rectangle r = new(82, Main.screenHeight-247, Main.screenWidth - 300, 210);
        Left.Set(82, 0);
        Top.Set(Main.screenHeight - 247,0);
        Width.Set(Main.screenWidth - 300,0);
        Height.Set(210, 0);

        // Preserve insertion order
        ManualSortMethod = (_) => { };
    }

    public float GetTotalHeight()
    {
        return _innerListHeight;
    }

    public void Goto(ElementSearchMethod searchMethod)
    {
        for (int i = 0; i < _items.Count; i++)
        {
            if (searchMethod(_items[i]))
            {
                _scrollbar.ViewPosition = _items[i].Top.Pixels;
                break;
            }
        }
    }

    public void Goto(ElementSearchMethod searchMethod, bool center = false)
    {
        float height = GetInnerDimensions().Height;
        for (int i = 0; i < _items.Count; i++)
        {
            UIElement uIElement = _items[i];
            if (searchMethod(uIElement))
            {
                _scrollbar.ViewPosition = uIElement.Top.Pixels;
                if (center)
                {
                    _scrollbar.ViewPosition = uIElement.Top.Pixels - height / 2f + uIElement.GetOuterDimensions().Height / 2f;
                }

                break;
            }
        }
    }

    public virtual void Add(UIElement item)
    {
        _items.Add(item);
        _innerList.Append(item);
        UpdateOrder();
        _innerList.Recalculate();
    }

    public virtual void Clear()
    {
        _innerList.RemoveAllChildren();
        _items.Clear();
    }

    public override void Recalculate()
    {
        base.Recalculate();
        UpdateScrollbar();
    }

    public override void ScrollWheel(UIScrollWheelEvent evt)
    {
        base.ScrollWheel(evt);
        if (_scrollbar != null)
        {
            _scrollbar.ViewPosition -= evt.ScrollWheelValue;
            SyncMonitorOffset(); // keep RemadeChatMonitor in step with our scroll
        }
    }

    public override void RecalculateChildren()
    {
        base.RecalculateChildren();

        // 1) Measure total content height
        float totalHeight = 0f;
        for (int i = 0; i < _items.Count; i++)
        {
            _items[i].Recalculate();
            totalHeight += _items[i].GetOuterDimensions().Height;
        }

        // 2) Bottom align when content is shorter than the viewport
        float viewHeight = GetInnerDimensions().Height;
        float startY = MathF.Max(0f, viewHeight - totalHeight);

        // 3) Position items from startY downward
        float y = startY;
        for (int i = 0; i < _items.Count; i++)
        {
            UIElement element = _items[i];
            element.Top.Set(y, 0f);
            element.Recalculate();
            y += element.GetOuterDimensions().Height;
        }

        _innerListHeight = MathF.Max(totalHeight, 0f);
        _innerList.Height.Set(_innerListHeight, 0f);

        UpdateScrollbar();
    }

    public void UpdateScrollbar()
    {
        if (_scrollbar != null)
        {
            float height = GetInnerDimensions().Height;
            _scrollbar.SetView(height, _innerListHeight);
        }
    }

    public void SetScrollbar(ChatScrollbarElement scrollbar)
    {
        _scrollbar = scrollbar;
        UpdateScrollbar();
    }

    public void UpdateOrder()
    {
        ManualSortMethod?.Invoke(_items);
        UpdateScrollbar();
    }

    public int SortMethod(UIElement item1, UIElement item2)
    {
        return item1.CompareTo(item2);
    }

    public override List<SnapPoint> GetSnapPoints()
    {
        List<SnapPoint> list = new List<SnapPoint>();
        if (GetSnapPoint(out var point))
        {
            list.Add(point);
        }

        foreach (UIElement item in _items)
        {
            list.AddRange(item.GetSnapPoints());
        }

        return list;
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        Top.Set(Main.screenHeight - 247, 0);

        if (_scrollbar != null)
        {
            _innerList.Top.Set(0f - _scrollbar.GetValue(), 0f);
        }
    }

    public IEnumerator<UIElement> GetEnumerator()
    {
        return ((IEnumerable<UIElement>)_items).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable<UIElement>)_items).GetEnumerator();
    }

    public virtual void AddRange(IEnumerable<UIElement> items)
    {
        foreach (UIElement item in items)
        {
            _items.Add(item);
            _innerList.Append(item);
        }

        UpdateOrder();
        _innerList.Recalculate();
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        SyncMonitorOffset();

        if (IsMouseHovering)
        {
            PlayerInput.LockVanillaMouseScroll("ModLoader/UIList");
        }
    }

    private void SyncMonitorOffset()
    {
        // Safety checks
        if (_scrollbar == null || _items.Count == 0)
            return;

        // 1) Gather geometry
        float viewHeight = GetInnerDimensions().Height;
        float contentHeight = _innerListHeight;
        if (contentHeight <= 0f || viewHeight <= 0f)
            return;

        // Line height: derive from first item; fall back to vanilla-ish 21 px
        float lineHeight = MathF.Max(1f, _items[0].GetOuterDimensions().Height);
        if (lineHeight < 1f)
            lineHeight = 21f;

        // When content is shorter than view, there is nothing to scroll; offset must be 0.
        if (contentHeight <= viewHeight)
        {
            SetMonitorStartChatLine(0, recalc: true);
            return;
        }

        // 2) Compute our top visible line index from current view position
        //    With your layout, items start at y=0 when content > view, so this is straightforward.
        float viewPos = _scrollbar.GetValue();
        int topIndex = (int)MathF.Floor(viewPos / lineHeight);
        if (topIndex < 0) topIndex = 0;

        // 3) Derive totalLines and showCount
        int totalLines = ScrollHelper.GetTotalLineCount();
        int showCount = TryGetMonitorShowCount(out var monitorShow)
            ? monitorShow
            : Math.Max(1, (int)MathF.Floor(viewHeight / lineHeight));

        // Clamp showCount to sane bounds
        showCount = Math.Clamp(showCount, 1, Math.Max(1, totalLines));

        // 4) Map to RemadeChatMonitor's startChatLine semantics:
        //    At bottom: topIndex == totalLines - showCount  -> startChatLine = 0
        //    Scrolling up decreases topIndex -> increases startChatLine.
        int desiredStart = Math.Clamp(totalLines - showCount - topIndex, 0, Math.Max(0, totalLines - showCount));

        // 5) Write back to the monitor (and flag a recalc so vanilla rebuilds line cache)
        SetMonitorStartChatLine(desiredStart, recalc: true);
    }
    private static bool TryGetMonitorShowCount(out int showCount)
    {
        showCount = 0;
        var monitor = Main.chatMonitor;
        if (monitor == null)
            return false;

        var type = monitor.GetType();
        // Prefer a public property if it exists, otherwise private field
        var prop = type.GetProperty("ShowCount", BindingFlags.Instance | BindingFlags.Public);
        if (prop != null)
        {
            if (prop.GetValue(monitor) is int v)
            {
                showCount = v;
                return true;
            }
        }

        var field = type.GetField("_showCount", BindingFlags.Instance | BindingFlags.NonPublic);
        if (field != null)
        {
            showCount = (int)field.GetValue(monitor);
            return true;
        }

        return false;
    }

    private static void SetMonitorStartChatLine(int value, bool recalc)
    {
        var monitor = Main.chatMonitor;
        if (monitor == null)
            return;

        var type = monitor.GetType();

        // Try a public Offset/StartChatLine property first (some builds expose one)
        var prop = type.GetProperty("Offset", BindingFlags.Instance | BindingFlags.Public)
                   ?? type.GetProperty("StartChatLine", BindingFlags.Instance | BindingFlags.Public);
        if (prop != null && prop.CanWrite)
        {
            prop.SetValue(monitor, value);
        }
        else
        {
            // Fallback to private backing field in 1.4.4.x
            var field = type.GetField("_startChatLine", BindingFlags.Instance | BindingFlags.NonPublic);
            if (field != null)
                field.SetValue(monitor, value);
        }

        if (recalc)
        {
            // Nudge vanilla to rebuild wrapped lines if necessary
            var recalcField = type.GetField("_recalculateOnNextUpdate", BindingFlags.Instance | BindingFlags.NonPublic);
            if (recalcField != null)
                recalcField.SetValue(monitor, true);
        }
    }
}