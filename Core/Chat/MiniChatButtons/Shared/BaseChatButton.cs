using System;
using System.Linq;
using ChatPlus.Core.Helpers;
using ChatPlus.Core.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.UI;
using Terraria.UI.Chat;

namespace ChatPlus.Core.Chat.MiniChatButtons.Shared;

internal abstract class BaseChatButton : UIElement
{
    protected abstract ChatButtonType Type { get; }
    protected abstract UserInterface UI { get; }
    protected abstract UIState State { get; }
    protected abstract Action<bool> SetOpenedByButton { get; }
    protected virtual Color ColorWhenEnabled => Color.White;
    protected virtual Color ColorWhenDisabled => new Color(180, 180, 180);

    protected virtual void DrawCustom(SpriteBatch sb, Vector2 pos) { } // per-button drawing

    protected BaseChatButton()
    {
        Width.Set(24, 0f);
        Height.Set(24, 0f);
    }
    protected virtual DraggablePanel GetPanel()
    {
        if (State != null && State.Children.Any())
        {
            foreach (var child in State.Children)
            {
                if (child is DraggablePanel dp)
                    return dp;
            }
        }

        return null;
    }

    public override void LeftClick(UIMouseEvent evt)
    {
        if (!Main.drawingPlayerChat)
        {
            return;
        }

        if (UI.CurrentState == null)
        {
            ChatPlus.StateManager.ResetAllWasOpenedByButton();
            ChatPlus.StateManager.CloseOthers(UI);

            SetOpenedByButton(true);
            UI.SetState(State);

            // Snap panel to button
            var panel = GetPanel();
            if (panel != null)
            {
                var dims = GetDimensions();
                var buttonPos = dims.Position();
                int buttonSize = (int)dims.Width;
                panel.SnapRightAlignedTo(buttonPos, buttonSize);
            }
            return;
        }

        SetOpenedByButton(false);
        UI.SetState(null);
    }

    public override void Draw(SpriteBatch sb)
    {
        if (!Main.drawingPlayerChat)
            return;

        var pos = ChatButtonLayout.ComputeTopLeft(Type);
        Left.Set(pos.X, 0f);
        Top.Set(pos.Y, 0f);

        if (!ChatButtonLayout.IsEnabled(Type))
            return;

        if (IsMouseHovering)
        {
            Main.LocalPlayer.mouseInterface = true;

            var r = GetDimensions().ToRectangle();
            r.Inflate(2, 2);
            DrawHelper.DrawPixelatedBorder(sb, r, Color.Gray, 2, 2);
        }

        var dims = GetDimensions();
        var pos2 = dims.Position();
        DrawCustom(sb, pos2 + new Vector2(2, 2));
    }

    public override bool ContainsPoint(Vector2 point)
    {
        if (!ChatButtonLayout.IsEnabled(Type))
            return false;

        return base.ContainsPoint(point);
    }
}
