using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
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

    public DraggablePanel ConnectedPanel { get; set; }
    protected virtual float SharedYOffset => 0f;

    public override void OnActivate()
    {
        base.OnActivate();
        live.Add(this);
        if (!sharedInitialized) { sharedPos = new Vector2(Left.Pixels, Top.Pixels - SharedYOffset); sharedInitialized = true; }
        Left.Set(sharedPos.X, 0f);
        Top.Set(sharedPos.Y + SharedYOffset, 0f);
        Recalculate();
    }

    public override void OnDeactivate()
    {
        live.Remove(this);
        base.OnDeactivate();
    }

    public override void Update(GameTime gameTime)
    {
        if (IsMouseHovering) Main.LocalPlayer.mouseInterface = true;

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

    private static bool IsAnyScrollbarHovering()
    {
        var states = new object[]
        {
        ModContent.GetInstance<Features.Commands.CommandSystem>()?.state,
        ModContent.GetInstance<Features.Colors.ColorSystem>()?.state,
        ModContent.GetInstance<Features.Emojis.EmojiSystem>()?.state,
        ModContent.GetInstance<Features.Glyphs.GlyphSystem>()?.state,
        ModContent.GetInstance<Features.Items.ItemSystem>()?.state,
        ModContent.GetInstance<Features.ModIcons.ModIconSystem>()?.state,
        ModContent.GetInstance<Features.PlayerHeads.PlayerHeadSystem>()?.state,
        ModContent.GetInstance<Features.Uploads.UploadSystem>()?.state
        };

        foreach (var s in states)
        {
            if (s is BaseState<dynamic> baseState && baseState.Panel?.scrollbar?.IsMouseHovering == true)
                return true;
        }
        return false;
    }

}