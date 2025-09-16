using System;
using System.Collections.Generic;
using System.Linq;
using ChatPlus.Common.Compat.CustomTags;
using ChatPlus.Core.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace ChatPlus.Core.UI;

public abstract class DraggablePanel : UIPanel
{
    private static readonly HashSet<DraggablePanel> live = [];
    private static Vector2 sharedPos;
    private static bool sharedInitialized;

    private bool dragging;
    private bool pendingDrag; // mouse down occurred but threshold not yet exceeded
    private Vector2 dragOffset;
    private const float dragThreshold = 3f;
    private Vector2 mouseDownPos;
    public bool IsDragging => dragging;
    public static bool AnyDragging
        => live.Any(p => p.dragging);
    public static bool AnyHovering;

    public DraggablePanel ConnectedPanel { get; set; }
    protected virtual float SharedYOffset => 0f;

    public override void OnActivate()
    {
        base.OnActivate();
        live.Add(this);
        if (!sharedInitialized) 
            sharedPos = new Vector2(Left.Pixels, Top.Pixels - SharedYOffset); sharedInitialized = true; 
        
        Left.Set(sharedPos.X, 0f);
        Top.Set(sharedPos.Y + SharedYOffset, 0f);
        Recalculate();
    }

    public override void OnDeactivate()
    {
        live.Remove(this);
        base.OnDeactivate();
    }

    public override void MouseOver(UIMouseEvent evt)
    {
        base.MouseOver(evt);
        AnyHovering = true;
    }

    public override void MouseOut(UIMouseEvent evt)
    {
        base.MouseOut(evt);
        AnyHovering = false;
    }

    public override void Update(GameTime gameTime)
    {
        if (IsMouseHovering)
        {
            //Log.Debug("hover dragpanel type: " + GetType());
            //AnyHovering = true;
            Main.LocalPlayer.mouseInterface = true;
        }
        else
        {
            //AnyHovering = false;
        }
        //Log.Info("any hovering: " + AnyHovering);

        // keep snapped when not dragging
        if (!dragging && sharedInitialized)
        {
            float exX = sharedPos.X, exY = sharedPos.Y + SharedYOffset;
            if (Math.Abs(Left.Pixels - exX) > 0.5f || Math.Abs(Top.Pixels - exY) > 0.5f)
            {
                Left.Set(exX, 0f);
                Top.Set(exY, 0f);
                Recalculate();
            }
        }

        // Determine if we should enter dragging after surpassing threshold
        if (pendingDrag && !dragging)
        {
            float moved = Vector2.Distance(new Vector2(Main.mouseX, Main.mouseY), mouseDownPos);
            if (moved > dragThreshold)
                dragging = true;
        }

        if (!dragging) return;

        var r = GetDimensions().ToRectangle();
        float oldX = Left.Pixels;
        float targetX = Main.mouseX - dragOffset.X;
        float rawDx = targetX - oldX;

        float dxMin = -r.Left;
        float dxMax = Main.screenWidth - r.Right;

        if (ConnectedPanel != null)
        {
            var other = ConnectedPanel.GetDimensions().ToRectangle();
            dxMin = Math.Max(dxMin, -other.Left);
            dxMax = Math.Min(dxMax, Main.screenWidth - other.Right);
        }

        float dx = MathHelper.Clamp(rawDx, dxMin, dxMax);

        // ► Move X only during drag; never touch Top here
        Left.Set(oldX + dx, 0f);
        Recalculate();

        if (ConnectedPanel != null)
        {
            ConnectedPanel.Left.Set(ConnectedPanel.Left.Pixels + dx, 0f);
            ConnectedPanel.Recalculate();
        }

        // update shared X only; keep shared Y unchanged while dragging
        sharedPos = new Vector2(Left.Pixels, sharedPos.Y);

        foreach (var p in live)
        {
            if (p == this) continue;
            p.Left.Set(sharedPos.X, 0f);
            p.Recalculate(); // no Top changes while dragging
        }
    }

    public override void LeftMouseDown(UIMouseEvent evt)
    {
        bool leftShiftDown = Main.keyState.IsKeyDown(Keys.LeftShift);

        if (leftShiftDown) return;
        if (IsAnyScrollbarHovering()) return;

        mouseDownPos = evt.MousePosition;
        base.LeftMouseDown(evt);
        dragging = false; // will become true after threshold
        pendingDrag = true;
        dragOffset = evt.MousePosition - new Vector2(Left.Pixels, Top.Pixels);
    }

    public override void LeftMouseUp(UIMouseEvent evt)
    {
        base.LeftMouseUp(evt);
        dragging = false;
        pendingDrag = false;

        // Persist only X; Y stays whatever each panel’s layout dictates.
        sharedPos = new Vector2(Left.Pixels, sharedPos.Y);

        foreach (var p in live)
        {
            if (p == this) continue;
            p.Left.Set(sharedPos.X, 0f);
            p.Top.Set(sharedPos.Y + p.SharedYOffset, 0f); // safe to reapply Y after drag
            p.Recalculate();
        }
    }

    public static bool IsAnyScrollbarHovering()
    {
        if (CustomTagSystem.States != null && CustomTagSystem.States.Count > 0)
        {
            foreach (BaseState<CustomTag> state in CustomTagSystem.States.Values)
            {
                if (state.Panel.scrollbar.IsMouseHovering == true) return true;
            }
        }

        return
            ChatPlus.StateManager.CommandSystem?.state?.Panel?.scrollbar?.IsMouseHovering == true ||
            ChatPlus.StateManager.ColorSystem?.state?.Panel?.scrollbar?.IsMouseHovering == true ||
            ChatPlus.StateManager.EmojiSystem?.state?.Panel?.scrollbar?.IsMouseHovering == true ||
            ChatPlus.StateManager.GlyphSystem?.state?.Panel?.scrollbar?.IsMouseHovering == true ||
            ChatPlus.StateManager.ItemSystem?.state?.Panel?.scrollbar?.IsMouseHovering == true ||
            ChatPlus.StateManager.ModIconSystem?.state?.Panel?.scrollbar?.IsMouseHovering == true ||
            ChatPlus.StateManager.MentionSystem?.state?.Panel?.scrollbar?.IsMouseHovering == true ||
            ChatPlus.StateManager.PlayerIconSystem?.state?.Panel?.scrollbar?.IsMouseHovering == true ||
            ChatPlus.StateManager.UploadSystem?.state?.Panel?.scrollbar?.IsMouseHovering == true;
    }
}