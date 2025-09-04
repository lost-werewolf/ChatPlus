using System;
using ChatPlus.Core.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace ChatPlus.Core.Features.Emojis
{
    public class EmojiSnippet : TextSnippet
    {
        public enum EmojiRenderRole
        {
            Normal,
            Button
        }

        public EmojiRenderRole Role { get; set; }

        private readonly Asset<Texture2D> emojiAsset;
        private readonly string tag;

        private static readonly Lazy<Asset<Effect>> GrayscaleEffect =
            new(() => ModContent.Request<Effect>("ChatPlus/Assets/Shaders/Grayscale", AssetRequestMode.ImmediateLoad));

        public static float GrayscaleIntensity = 1f;
        public bool Hovered { get; set; }

        public EmojiSnippet(Asset<Texture2D> asset, string tag) : base(tag)
        {
            emojiAsset = asset;
            this.tag = tag;
        }

        public override bool UniqueDraw(bool justCheckingString, out Vector2 size, SpriteBatch sb, Vector2 pos = default, Color color = default, float scale = 1f)
        {
            var tex = emojiAsset.Value;
            if (tex == null)
            {
                size = new Vector2(0f, 20f * scale);
                return true;
            }

            float h = 20f * scale;
            float s = h / Math.Max(tex.Width, tex.Height);
            Vector2 baseSize = new Vector2(tex.Width * s, h);

            size = baseSize;
            if (justCheckingString) return true;

            var mouse = Main.MouseScreen.ToPoint();
            bool hovered = new Rectangle((int)pos.X, (int)pos.Y, (int)baseSize.X, (int)baseSize.Y).Contains(mouse);

            float factor = 0.90f;
            if (Role == EmojiRenderRole.Button && hovered) factor = 1.10f;

            float drawScale = s * factor;
            Vector2 drawSize = baseSize * factor;
            Vector2 drawPos = pos + (baseSize - drawSize) * 0.5f;

            bool useGrayscale = Role == EmojiRenderRole.Button && !hovered && GrayscaleIntensity > 0f;
            if (useGrayscale)
            {
                sb.End();
                var fx = GrayscaleEffect.Value.Value;
                fx.Parameters["Intensity"].SetValue(MathHelper.Clamp(GrayscaleIntensity, 0f, 1f));
                sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, fx, Main.UIScaleMatrix);
            }

            Color drawColor = color;
            if (drawColor.Equals(default(Color))) drawColor = Color.White;

            sb.Draw(tex, drawPos, null, drawColor, 0f, Vector2.Zero, drawScale, SpriteEffects.None, 0f);

            if (useGrayscale)
            {
                sb.End();
                sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.UIScaleMatrix);
            }

            return true;
        }

        public override void OnClick()
        {
            if (Role == EmojiRenderRole.Button)
            {
                if (StateManager.IsAnyStateActive())
                {
                    EmojiSystem.CloseAfterCommit();
                }
                else
                {
                    EmojiSystem.OpenFromButton();
                }
            }

            base.OnClick();
        }

    }
}
