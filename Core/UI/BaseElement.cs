using ChatPlus.Core.Features.Emojis;
using ChatPlus.Core.Features.Uploads;
using ChatPlus.Core.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.UI;
using static ChatPlus.Common.Configs.Config;

namespace ChatPlus.Core.UI;

/// <summary>
/// An element that can be navigated in a <see cref="BasePanel<TData>"/>.
/// </summary>
public abstract class BaseElement<TData> : UIElement
{
    private bool isSelected;
    public bool SetSelected(bool value) => isSelected = value;

    public TData Data { get; }

    private BasePanel<TData> GetParentPanel()
    {
        UIElement parent = Parent;
        while (parent != null && parent is not BasePanel<TData>)
        {
            parent = parent.Parent;
        }
        return parent as BasePanel<TData>;
    }

    protected Viewmode GetViewmode()
    {
        var parent = GetParentPanel();
        return parent != null ? parent.CurrentViewMode : Viewmode.List;  
    }
    protected bool IsGridSwitchSuppressed()
    {
        var parent = GetParentPanel();
        return parent?.IsGridSwitchSuppressed == true;
    }

    protected BaseElement(TData data)
    {
        Height.Set(30, 0);
        Width.Set(0, 1);
        Data = data;
    }

    public override void LeftClick(UIMouseEvent evt)
    {
        base.LeftClick(evt);

        bool leftShiftDown = Main.keyState.IsKeyDown(Keys.LeftShift);
        if (leftShiftDown) return;

        var panel = GetParentPanel();
        if (panel == null) return;                        

        int index = panel.items.IndexOf(this);
        if (index >= 0)
        {
            panel.SetSelectedIndex(index);
            panel.InsertSelectedTag();
        }
    }

    public override void Update(GameTime gameTime)
    {
        //base.Update(gameTime);
        Log.Info("u");
        if (IsMouseHovering)
        {
            var panel = GetParentPanel();
            Log.Info("p: " + panel);
            if (panel != null)
            {
                int index = panel.items.IndexOf(this);
                if (index >= 0 && panel.CurrentIndex != index)
                {
                    panel.SetSelectedIndex(index);
                }
            }
        }
    }

    public override void Draw(SpriteBatch sb)
    {
        // selection visuals
        if (isSelected)
        {
            DrawHelper.DrawSlices(sb, ele: this);
            DrawHelper.DrawFill(sb, ele: this);
        }
        else
        {
            Rectangle r = GetDimensions().ToRectangle();
            DrawHelper.DrawPixelatedBorder(sb, r, Color.Black * 0.75f, 2, 1);
        }

        // always call into subclass rendering
        if (this is not UploadElement)
            base.Draw(sb);

        // unified list vs grid
        bool forceGrid = IsGridSwitchSuppressed();
        if (!forceGrid && GetViewmode() == Viewmode.List)
            DrawListElement(sb);
        else
            DrawGridElement(sb);
    }

    // subclasses must provide these
    protected abstract void DrawListElement(SpriteBatch sb);
    protected abstract void DrawGridElement(SpriteBatch sb);
}


