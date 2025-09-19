using System;
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

    private static readonly Lazy<Asset<Effect>> grayscaleEffect =
        new(() => ModContent.Request<Effect>("ChatPlus/Assets/Effects/Grayscale", AssetRequestMode.ImmediateLoad));

    public EmojiSnippet(Asset<Texture2D> asset, string tag, bool grayscale = false) : base(tag)
    {
        emojiAsset = asset;
        this.grayscale = grayscale;
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
        size = new Vector2(tex.Width * s, h);
        if (justCheckingString) return true;

        // skip shadow pass
        bool isShadowPass = color.R + color.G + color.B <= 5;
        if (isShadowPass) return true;

        float drawScale = s;
        Vector2 drawPos = pos;
        Color drawColor = color.Equals(default) ? Color.White : color;

        if (grayscale && grayscaleEffect.Value != null)
        {
            sb.End();
            sb.Begin(
                SpriteSortMode.Immediate,
                BlendState.AlphaBlend,
                SamplerState.LinearClamp,
                DepthStencilState.Default,
                RasterizerState.CullNone,
                grayscaleEffect.Value.Value,
                Main.UIScaleMatrix // <-- keep UI transform
            );

            sb.Draw(tex, drawPos, null, drawColor, 0f, Vector2.Zero, drawScale, SpriteEffects.None, 0f);

            sb.End();
            sb.Begin(
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.LinearClamp,
                DepthStencilState.Default,
                RasterizerState.CullNone,
                null,
                Main.UIScaleMatrix // <-- restore UI transform
            );
        }
        else
        {
            sb.Draw(tex, drawPos, null, drawColor, 0f, Vector2.Zero, drawScale, SpriteEffects.None, 0f);
        }

        return true;
    }

}

