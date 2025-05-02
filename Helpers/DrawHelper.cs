using System;
using LinksInChat.Common;
using LinksInChat.Common.Hooks;
using log4net;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace LinksInChat.Helpers
{
    public static class DrawHelper
    {
        public static void DrawPlayerHead(Vector2 position, float scale=0.9f)
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
    }
}

