using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.UI;

namespace ChatPlus.Core.UI;

public class CustomGrid : UIElement
{
    public int FixedColumns { get; set; } = 8;
    public int CellWidth { get; set; } = 30;
    public int CellHeight { get; set; } = 30;

    public delegate bool ElementSearchMethod(UIElement element);

    public class UIInnerList : UIElement
    {
        public override bool ContainsPoint(Vector2 point)
        {
            return true;
        }

        protected override void DrawChildren(SpriteBatch spriteBatch)
        {
            Vector2 position = base.Parent.GetDimensions().Position();
            Vector2 dimensions = new Vector2(base.Parent.GetDimensions().Width, base.Parent.GetDimensions().Height);
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
    }

    public List<UIElement> _items = new List<UIElement>();

    public UIScrollbar _scrollbar;

    public UIElement _innerList = new UIInnerList();

    public float _innerListHeight;

    public float ListPadding = 5f;

    public int Count => _items.Count;

    public CustomGrid()
    {
        _innerList.OverflowHidden = false;
        _innerList.Width.Set(0f, 1f);
        _innerList.Height.Set(0f, 1f);
        OverflowHidden = true;
        Append(_innerList);
    }

    public float GetTotalHeight()
    {
        return _innerListHeight;
    }

    public void Goto(ElementSearchMethod searchMethod, bool center = false)
    {
        for (int i = 0; i < _items.Count; i++)
        {
            if (searchMethod(_items[i]))
            {
                _scrollbar.ViewPosition = _items[i].Top.Pixels;
                if (center)
                {
                    _scrollbar.ViewPosition = _items[i].Top.Pixels - GetInnerDimensions().Height / 2f + _items[i].GetOuterDimensions().Height / 2f;
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

    public virtual void AddRange(IEnumerable<UIElement> items)
    {
        _items.AddRange(items);
        foreach (UIElement item in items)
        {
            _innerList.Append(item);
        }

        UpdateOrder();
        _innerList.Recalculate();
    }

    public virtual bool Remove(UIElement item)
    {
        _innerList.RemoveChild(item);
        UpdateOrder();
        return _items.Remove(item);
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

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        if (base.IsMouseHovering)
        {
            PlayerInput.LockVanillaMouseScroll("ModLoader/UIList");
        }
    }

    public override void ScrollWheel(UIScrollWheelEvent evt)
    {
        base.ScrollWheel(evt);
        if (_scrollbar != null)
        {
            _scrollbar.ViewPosition -= evt.ScrollWheelValue;
        }
    }

    public override void RecalculateChildren()
    {
        base.RecalculateChildren();

        int cols = Math.Max(1, FixedColumns);
        float pad = ListPadding;

        float rowHeight = CellHeight + pad;
        float colWidth = CellWidth + pad;

        for (int i = 0; i < _items.Count; i++)
        {
            UIElement e = _items[i];

            // Force fixed cell size
            e.Width.Set(CellWidth, 0f);
            e.Height.Set(CellHeight, 0f);

            int row = i / cols;
            int col = i % cols;

            float left = col * colWidth;
            float top = row * rowHeight;

            e.Left.Set(left, 0f);
            e.Top.Set(top, 0f);
        }

        int totalRows = (_items.Count + cols - 1) / cols;
        _innerListHeight = Math.Max(0f, totalRows * rowHeight - pad);
    }

    public void UpdateScrollbar()
    {
        if (_scrollbar != null)
        {
            _scrollbar.SetView(GetInnerDimensions().Height, _innerListHeight);
        }
    }

    public void SetScrollbar(UIScrollbar scrollbar)
    {
        _scrollbar = scrollbar;
        UpdateScrollbar();
    }

    public void UpdateOrder()
    {
        //_items.Sort(SortMethod);
        UpdateScrollbar();
    }

    public int SortMethod(UIElement item1, UIElement item2)
    {
        return 0;
        //return item1.CompareTo(item2);
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
        if (_scrollbar != null)
        {
            _innerList.Top.Set(0f - _scrollbar.GetValue(), 0f);
        }
    }
}
