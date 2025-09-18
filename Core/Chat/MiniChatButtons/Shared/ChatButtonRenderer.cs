using System;
using ChatPlus.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Chat;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace ChatPlus.Core.Chat.MiniChatButtons.Shared;

public static class ChatButtonRenderer
{
    public static void Draw(SpriteBatch sb, ChatButtonType kind, Vector2 pos, int size, bool grayscale = false)
    {
        // background box
        var rect = new Rectangle((int)pos.X, (int)pos.Y, size, size);
        //sb.Draw(TextureAssets.MagicPixel.Value, rect, new Color(30, 30, 40, 255));

        switch (kind)
        {
            case ChatButtonType.Emojis:
                DrawTag(sb, "[e:smiling_face_with_sunglasses]", pos + new Vector2(2, 2), grayscale);
                break;

            case ChatButtonType.Uploads:
                DrawTextureOrAscii(sb, "ChatPlus/Assets/ButtonUpload", "+", pos, grayscale, 0.7f);
                break;

            case ChatButtonType.Mentions:
                DrawAscii(sb, "@", pos + new Vector2(-1, 1), grayscale);
                break;

            case ChatButtonType.Items:
                var itemTarget = new Rectangle((int)pos.X + 2, (int)pos.Y + 2, 20, 20);
                DrawWithGrayscale(sb, grayscale,
                    () => sb.Draw(TextureAssets.Item[1].Value, itemTarget, Color.White),
                    () => sb.Draw(TextureAssets.Item[1].Value, itemTarget, Color.Gray));
                break;

            case ChatButtonType.Glyphs:
                float old = GlyphTagHandler.GlyphsScale;
                GlyphTagHandler.GlyphsScale = 0.8f;

                DrawWithGrayscale(sb, grayscale,
                    () => ChatManager.DrawColorCodedStringWithShadow(
                        sb,
                        FontAssets.MouseText.Value,
                        "[g:5]",
                        pos + new Vector2(2f, 0f),
                        Color.White,
                        0f,
                        Vector2.Zero,
                        Vector2.One
                    ),
                    () => ChatManager.DrawColorCodedStringWithShadow(
                        sb,
                        FontAssets.MouseText.Value,
                        "[g:5]",
                        pos + new Vector2(2f, 0f),
                        Color.Gray,
                        0f,
                        Vector2.Zero,
                        Vector2.One
                    ));

                GlyphTagHandler.GlyphsScale = old;
                break;

            case ChatButtonType.Colors:
                DrawTextureOrAscii(sb, "ChatPlus/Assets/ButtonColor", "K", pos, grayscale, 0.6f);
                break;

            case ChatButtonType.Commands:
                DrawAscii(sb, "/", pos + new Vector2(1, 2), grayscale);
                break;

            case ChatButtonType.ModIcons:
                var mod = ModLoader.GetMod("ChatPlus");
                if (mod != null && (mod.FileExists("icon_small.png") || mod.FileExists("icon_small.rawimg")))
                {
                    var tex = mod.Assets.Request<Texture2D>("icon_small").Value;
                    DrawWithGrayscale(sb, grayscale,
                        () => sb.Draw(tex, pos + new Vector2(1f, 0f), null, Color.White, 0f, Vector2.Zero, 0.7f, SpriteEffects.None, 0f),
                        () => sb.Draw(tex, pos + new Vector2(1f, 0f), null, Color.Gray, 0f, Vector2.Zero, 0.7f, SpriteEffects.None, 0f));
                }
                else
                {
                    DrawAscii(sb, "M", pos, grayscale);
                }
                break;

            case ChatButtonType.PlayerIcons:
                if (!string.IsNullOrEmpty(Main.LocalPlayer?.name))
                {
                    DrawTag(sb, $"[p:{Main.LocalPlayer.name}]", pos + new Vector2(-2f, 1f), grayscale);
                }
                break;

            case ChatButtonType.Settings:
                DrawTextureOrAscii(sb, "ChatPlus/Assets/LastUpdatedIcon", "S", pos, grayscale, 0.6f);
                break;
        }
    }

    private static void DrawTag(SpriteBatch sb, string tag, Vector2 pos, bool grayscale)
    {
        // only emojis and player icons support /gray
        string drawTag = grayscale ? tag.Insert(tag.Length - 1, "/gray") : tag;
        ChatManager.DrawColorCodedStringWithShadow(
            sb,
            FontAssets.MouseText.Value,
            drawTag,
            pos,
            Color.White,
            0f,
            Vector2.Zero,
            Vector2.One
        );
    }

    private static void DrawTextureOrAscii(SpriteBatch sb, string path, string fallback, Vector2 pos, bool grayscale, float scale)
    {
        var tex = ModContent.Request<Texture2D>(path, AssetRequestMode.ImmediateLoad).Value;
        if (tex != null)
        {
            var target = new Rectangle((int)pos.X + 2, (int)pos.Y + 2, (int)(tex.Width * scale), (int)(tex.Height * scale));

            DrawWithGrayscale(sb, grayscale,
                () => sb.Draw(tex, target, Color.White),
                () => sb.Draw(tex, target, Color.Gray));
        }
        else
        {
            DrawAscii(sb, fallback, pos, grayscale);
        }
    }

    private static void DrawAscii(SpriteBatch sb, string text, Vector2 pos, bool grayscale)
    {
        ChatManager.DrawColorCodedStringWithShadow(
            sb,
            FontAssets.DeathText.Value,
            text,
            pos + new Vector2(6f, -3f),
            grayscale ? Color.Gray : Color.White,
            0f,
            Vector2.Zero,
            new Vector2(0.5f)
        );
    }

    private static readonly Lazy<Asset<Effect>> grayscaleEffect =
    new(() => ModContent.Request<Effect>("ChatPlus/Assets/Effects/Grayscale", AssetRequestMode.ImmediateLoad));

    private static void DrawWithGrayscale(SpriteBatch sb, bool grayscale, Action drawAction, Action? fallbackDrawAction = null)
    {
        if (!grayscale)
        {
            drawAction();
            return;
        }

        var effectAsset = grayscaleEffect.Value;
        if (effectAsset != null && effectAsset.IsLoaded && effectAsset.Value != null)
        {
            var effect = effectAsset.Value;

            // set technique if needed (your .fx file usually has "Technique1")
            effect.CurrentTechnique.Passes[0].Apply();

            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp,
                DepthStencilState.Default, RasterizerState.CullNone, effect);

            drawAction();

            sb.End();
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
        }
        else
        {
            if (fallbackDrawAction != null)
                fallbackDrawAction();
            else
                drawAction();
        }
    }
}
