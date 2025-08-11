using System;
using AdvancedChatFeatures.Common;
using AdvancedChatFeatures.Common.Hooks;
using log4net;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;

namespace AdvancedChatFeatures.Helpers
{
    public static class DrawHelper
    {
        public static void DrawPlayerHead(Vector2 position, float scale = 0.9f)
        {
            if (Main.LocalPlayer == null)
            {
                Log.Error("Oof no local player in DrawHelper");
                return;
            }

            // Setup
            Player player = Main.LocalPlayer; // change later
            PlayerHeadFlipHook.shouldFlipHeadDraw = player.direction == -1;

            // Call draw
            Main.MapPlayerRenderer.DrawPlayerHead(
                camera: Main.Camera,
                drawPlayer: player,
                position: position,
                alpha: 1f,
                scale: scale,
                Color.White
            );
            PlayerHeadFlipHook.shouldFlipHeadDraw = false;
        }

        /// <summary>
        /// Draws a texture at the proper scale to fit within the given UI element.
        /// /// </summary>
        public static void DrawProperScale(SpriteBatch spriteBatch, UIElement element, Texture2D tex, float scale = 1.0f, float opacity = 1.0f, bool active = false, int x = 0, int y = 0)
        {
            // Get the UI element's dimensions
            CalculatedStyle dims = element.GetDimensions();

            // Compute a scale that makes it fit within the UI element
            float scaleX = dims.Width / tex.Width;
            float scaleY = dims.Height / tex.Height;
            float drawScale = Math.Min(scaleX, scaleY) * scale;

            // Top-left anchor: just place it at dims.X, dims.Y
            Vector2 drawPosition = new Vector2(dims.X + x, dims.Y + y);

            float actualOpacity = opacity;
            if (active)
            {
                actualOpacity = 1f;
            }

            // Draw the texture anchored at top-left with the chosen scale
            spriteBatch.Draw(
                tex,
                drawPosition,
                null,
                Color.White * actualOpacity,
                0f,
                Vector2.Zero,
                drawScale,
                SpriteEffects.None,
                0f
            );
        }

        public static void DrawTextAtMouse(SpriteBatch sb, string text)
        {
            if (text == null)
            {
                Log.Error("text null!!");
                return;
            }

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
    }
}

