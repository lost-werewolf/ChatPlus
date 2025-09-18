using System;
using System.Linq;
using ChatPlus.Core.Chat.MiniChatButtons.Shared;
using ChatPlus.Core.Features.Glyphs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Chat;
using Terraria.ModLoader;
using Terraria.UI;

namespace ChatPlus.Core.Chat.MiniChatButtons;

internal class GlyphButton : BaseChatButton
{
    protected override ChatButtonType Type => ChatButtonType.Glyphs;
    protected override UserInterface UI => ModContent.GetInstance<GlyphSystem>().ui;
    protected override UIState State => ModContent.GetInstance<GlyphSystem>().state;
    protected override Action<bool> SetOpenedByButton => flag => GlyphState.WasOpenedByButton = flag;


    private static readonly Random rng = new();
    private Glyph? currentGlyph;

    private static readonly Lazy<Asset<Effect>> grayscaleEffect =
        new(() => ModContent.Request<Effect>("ChatPlus/Assets/Effects/Grayscale", AssetRequestMode.ImmediateLoad));

    public GlyphButton()
    {
        //PickRandomGlyph();
        currentGlyph = GlyphManager.Glyphs[5];
    }

    public override void MouseOut(UIMouseEvent evt)
    {
        base.MouseOut(evt);
        //PickRandomGlyph();
    }

    private void PickRandomGlyph()
    {
        if (GlyphManager.Glyphs == null || GlyphManager.Glyphs.Count == 0)
        {
            currentGlyph = null;
            return;
        }

        currentGlyph = GlyphManager.Glyphs[rng.Next(GlyphManager.Glyphs.Count)];
    }

    protected override void DrawCustom(SpriteBatch sb, Vector2 pos)
    {
        if (currentGlyph == null) return;

        // Centralized renderer uses a sample glyph; we just need consistent look/behavior here.
        var dims = GetDimensions();
        bool forceNormal = IsMouseHovering || UI.CurrentState == State;
        ChatButtonRenderer.Draw(sb, Type, dims.Position(), 24, grayscale: !forceNormal);
    }
}
