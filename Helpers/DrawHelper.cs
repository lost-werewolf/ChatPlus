using System;
using AdvancedChatFeatures.Common.Systems;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace AdvancedChatFeatures.Helpers
{
    public static class DrawHelper
    {
        public static void DrawSmallModIcon(SpriteBatch sb, Mod mod, Vector2 pos, int size = 16)
        {
            Texture2D tex = null;

            // Default fallback for unknown mods
            if (mod == null)
            {
                tex = Ass.TerrariaIcon.Value;
            }
            else if (mod.Name == "ModLoader")
            {
                tex = Ass.tModLoaderIcon.Value;
            }
            else
            {
                string path = $"{mod.Name}/icon_small";
                if (ModContent.HasAsset(path))
                    tex = ModContent.Request<Texture2D>(path).Value;
            }

            Rectangle target = new((int)pos.X - 3, (int)pos.Y - 2, size, size);

            if (tex != null)
            {
                DrawTextureScaledToFit(sb, tex, target);
            }
            else if (mod != null)
            {
                // fallback to initials
                string initials = mod.DisplayName.Length >= 2 ? mod.DisplayName[..2] : mod.DisplayName;
                Vector2 initialsPos = target.Center.ToVector2();
                initialsPos += new Vector2(0, 5);
                Utils.DrawBorderString(sb, initials, initialsPos, Color.White, scale: 1.0f, 0.5f, 0.5f);
            }
        }
        public static void DrawTextAtMouse(SpriteBatch sb, string text)
        {
            // This method is used for drawing tooltips in main menu
            // Inspired by UICharacterCreation::Draw()
            float x = FontAssets.MouseText.Value.MeasureString(text).X;
            Vector2 vector = new Vector2(Main.mouseX, Main.mouseY) + new Vector2(16f);
            if (vector.Y > (float)(Main.screenHeight - 15))
            {
                vector.Y = Main.screenHeight - 15;
            }
            if (vector.X > (float)Main.screenWidth - x + 40)
            {
                vector.X = Main.screenWidth - 460;
            }
            Utils.DrawBorderStringFourWay(
                sb, FontAssets.MouseText.Value, text, vector.X, vector.Y, new Color(Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor), Color.Black, Vector2.Zero);
        }

        public static void DrawTextureScaledToFit(SpriteBatch sb, Texture2D tex, Rectangle target)
        {
            if (tex == null)
                return;

            float scale = Math.Min(
                target.Width / (float)tex.Width,
                target.Height / (float)tex.Height
            );

            sb.Draw(
                tex,
                target.Center.ToVector2(),
                null,
                Color.White,
                0f,
                tex.Size() / 2f,
                scale,
                SpriteEffects.None,
                0f
            );
        }
        public static void DrawGhostText(SpriteBatch sb, string ghostText)
        {
            if (string.IsNullOrEmpty(ghostText)) return;

            // Remove blinking
            Main.instance.textBlinkerState = 0;
            Main.instance.textBlinkerCount = 0;

            Vector2 chatOriginUi = new(80f, Main.screenHeight - 32f);
            Vector2 typedSizeUi = FontAssets.MouseText.Value.MeasureString(Main.chatText ?? "");
            Vector2 ghostSizeUi = FontAssets.MouseText.Value.MeasureString(ghostText);
            Vector2 posUi = chatOriginUi + new Vector2(typedSizeUi.X + 12, 3f);
            Vector2 bgSizeUi = new(ghostSizeUi.X + 4f, ghostSizeUi.Y + 4f - 10);

            posUi += new Vector2(-4, -4);

            // Draw background
            DrawHelper.DrawRectangle(sb, bgSizeUi, posUi, ColorHelper.Blue * 0.5f);

            // Draw ghost text
            Utils.DrawBorderString(sb, ghostText, posUi, Color.White);
        }

        /// <summary>
        /// ...Isn't it obvious?
        /// </summary>
        public static void DrawRectangle(SpriteBatch sb, Vector2 size, Vector2 pos, Color color)
        {
            sb.Draw(
                texture: TextureAssets.MagicPixel.Value,
                destinationRectangle: new Rectangle((int)pos.X, (int)pos.Y, (int)size.X, (int)size.Y),
                color: color);
        }

        /// <summary>
        /// Draws a nine slice for the chat
        /// </summary>
        public static void DrawNineSlice(int x, int y, int w, int h, Texture2D tex, Color color)
        {
            int c = 10;
            int ew = tex.Width - c * 2;
            int eh = tex.Height - c * 2;

            Main.spriteBatch.Draw(tex, new Vector2(x, y), new Rectangle(0, 0, c, c), color);
            Main.spriteBatch.Draw(tex, new Rectangle(x + c, y, w - c * 2, c), new Rectangle(c, 0, ew, c), color);
            Main.spriteBatch.Draw(tex, new Vector2(x + w - c, y), new Rectangle(tex.Width - c, 0, c, c), color);

            Main.spriteBatch.Draw(tex, new Rectangle(x, y + c, c, h - c * 2), new Rectangle(0, c, c, eh), color);
            Main.spriteBatch.Draw(tex, new Rectangle(x + c, y + c, w - c * 2, h - c * 2), new Rectangle(c, c, ew, eh), color);
            Main.spriteBatch.Draw(tex, new Rectangle(x + w - c, y + c, c, h - c * 2), new Rectangle(tex.Width - c, c, c, eh), color);

            Main.spriteBatch.Draw(tex, new Vector2(x, y + h - c), new Rectangle(0, tex.Height - c, c, c), color);
            Main.spriteBatch.Draw(tex, new Rectangle(x + c, y + h - c, w - c * 2, c), new Rectangle(c, tex.Height - c, ew, c), color);
            Main.spriteBatch.Draw(tex, new Vector2(x + w - c, y + h - c), new Rectangle(tex.Width - c, tex.Height - c, c, c), color);
        }
    }
}

