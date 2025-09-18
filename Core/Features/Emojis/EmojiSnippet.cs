using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace ChatPlus.Core.Features.Emojis;

public class EmojiSnippet : TextSnippet
{
    private readonly Asset<Texture2D> emojiAsset;
    private readonly bool grayscale;

    // shader
    private static readonly Lazy<Asset<Effect>> grayscaleEffect =
        new(() => ModContent.Request<Effect>("ChatPlus/Assets/Effects/Grayscale", AssetRequestMode.ImmediateLoad));

    // smooth zoom state (0 = gray, 1 = normal)
    private float zoomLerp;

    public EmojiSnippet(Asset<Texture2D> asset, string tag, bool gs = false) : base(tag)
    {
        emojiAsset = asset;
        this.grayscale = gs;
        zoomLerp = gs ? 0f : 1f; // start gray if requested
    }

    public override bool UniqueDraw(bool justCheckingString, out Vector2 size, SpriteBatch sb,
        Vector2 pos = default, Color color = default, float scale = 1f)
    {
        var tex = emojiAsset.Value;
        if (tex == null)
        {
            size = new Vector2(0f, 20f * scale);
            return true;
        }

        float h = 20f * scale;
        float s = h / Math.Max(tex.Width, tex.Height);
        Vector2 baseSize = new(tex.Width * s, h);
        size = baseSize;

        if (justCheckingString) return true;

        // skip shadow pass
        bool isShadowPass = color.R + color.G + color.B <= 5;
        if (isShadowPass)
            return true;

        Color drawColor = color.Equals(default) ? Color.White : color;

        // detect hover → (very rough; you can improve with proper bounds checks)
        Rectangle bounds = new((int)pos.X, (int)pos.Y, (int)baseSize.X, (int)baseSize.Y);
        bool hovering = bounds.Contains(Main.mouseX, Main.mouseY);

        // update zoom lerp
        float target = hovering ? 1f : 0f;
        zoomLerp = MathHelper.Lerp(zoomLerp, target, 0.65f); // smooth transition

        // zoom factor
        float zoomFactor = MathHelper.Lerp(0.9f, 1.1f, zoomLerp);
        float drawScale = s * zoomFactor;

        // center draw
        Vector2 origin = new(tex.Width / 2f, tex.Height / 2f);
        Vector2 drawPos = pos + baseSize * 0.5f;

        // gs → color blend
        if (grayscale && grayscaleEffect.Value != null && zoomLerp < 0.99f)
        {
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend,
                SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone,
                grayscaleEffect.Value.Value);

            sb.Draw(tex, drawPos, null, drawColor, 0f, origin, drawScale, SpriteEffects.None, 0f);

            sb.End();
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
        }
        else
        {
            sb.Draw(tex, drawPos, null, drawColor, 0f, origin, drawScale, SpriteEffects.None, 0f);
        }

        return true;
    }

    public override void OnClick()
    {
        base.OnClick();
    }
}
