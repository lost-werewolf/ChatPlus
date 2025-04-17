using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using LinksInChat.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;
using Terraria.UI.Chat;

namespace LinksInChat.Common
{
    public class CustomSnippet : TextSnippet
    {
        private readonly TextSnippet _wrappedSnippet;
        private readonly string text; // trimmed text for consistency

        // Holds the bounding rectangle from the last draw call.
        private Rectangle lastDrawRect = Rectangle.Empty;
        // Track the update frame when we last drew the underline so we only do it once per frame.
        private int lastUnderlineDrawFrame = -1;

        public CustomSnippet(TextSnippet toWrap, Mod mod)
            : base(toWrap.Text, toWrap.Color, toWrap.Scale)
        {
            _wrappedSnippet = toWrap;
            CheckForHover = toWrap.CheckForHover;
            DeleteWhole = toWrap.DeleteWhole;
            this.text = toWrap.Text.Trim();
        }

        public override Color GetVisibleColor()
        {
            // Always use a link color for link snippets.
            if (IsWholeLink(text))
            {
                // Optionally, you can choose a different color if the mouse is hovering.
                // Here we check if Main.MouseScreen is within lastDrawRect.
                bool isHovered = lastDrawRect.Contains(Main.MouseScreen.ToPoint());
                return isHovered ? new Color(7, 55, 99) : new Color(17, 85, 204);
            }
            return _wrappedSnippet.GetVisibleColor();
        }

        public override void Update()
        {
            // Nothing extra to update; rely on UniqueDraw for the rectangle.
            _wrappedSnippet.Update();
        }

        public override bool UniqueDraw(bool justCheckingString, out Vector2 size, SpriteBatch spriteBatch, Vector2 position = default, Color color = default, float scale = 1)
        {
            // First, calculate the text size.
            Vector2 textSize = FontAssets.MouseText.Value.MeasureString(text) * scale;
            // Store the bounding box of this snippet.
            lastDrawRect = new Rectangle((int)position.X + 8, (int)position.Y, (int)textSize.X, (int)textSize.Y);

            // Debug hitbox
            // spriteBatch.Draw(TextureAssets.MagicPixel.Value, lastDrawRect, new Color(255, 0, 0, 100)); // Red hitbox for debugging

            // Draw the underline only during the actual drawing pass
            // and ensure it is drawn only once per update frame.
            if (!justCheckingString && IsWholeLink(text))
            {
                if (Main.GameUpdateCount != lastUnderlineDrawFrame)
                {
                    lastUnderlineDrawFrame = (int)Main.GameUpdateCount;
                    // Define the underline rectangle. Adjust the Y offset as needed.
                    Rectangle underlineRect = new Rectangle((int)position.X + 8, (int)(position.Y + textSize.Y - 9), (int)textSize.X, 1);
                    // Use the same color that GetVisibleColor() returns.
                    Color underlineColor = GetVisibleColor();
                    spriteBatch.Draw(TextureAssets.MagicPixel.Value, underlineRect, underlineColor);
                }
            }

            // then, if this is the link and the mouse is over it, draw the fancy underline + text
            if (IsWholeLink(text) && lastDrawRect.Contains(Main.MouseScreen.ToPoint()))
                DrawLinkHover(spriteBatch);

            // Forward the draw call to the wrapped snippet.
            return _wrappedSnippet.UniqueDraw(justCheckingString, out size, spriteBatch, position, color, scale);
        }

        public override void OnHover()
        {
            _wrappedSnippet?.OnHover();
        }

        private void DrawLinkHover(SpriteBatch sb)
        {
            // 1) Use the current mouse position
            Vector2 pos = Main.MouseScreen + new Vector2(25, 10);

            string ex = "Open link";
            var font = FontAssets.MouseText.Value;
            float scale = 0.8f;

            // 2) Measure the *scaled* text
            Vector2 textSize = font.MeasureString(ex) * scale;

            // 1) Build a snippet array
            var linkSnips = ChatManager.ParseMessage(ex, Color.White).ToArray();
            // 2) Call the snippet overload with an 'out' hovered parameter
            int hovered;
            ChatManager.DrawColorCodedStringWithShadow(
                sb,
                FontAssets.MouseText.Value,
                linkSnips,         // TextSnippet[]
                pos,           // Vector2 position
                0f,                // float rotation
                Vector2.Zero,      // Vector2 origin
                new Vector2(1.0f),// Vector2 baseScale
                out hovered        // out int hoveredSnippet
            );
        }


        public override void OnClick()
        {
            _wrappedSnippet?.OnClick();
            if (IsWholeLink(text))
                URL.OpenURL(text);
        }

        // Helper method: returns true if the input exactly matches a URL pattern.
        private bool IsWholeLink(string input)
        {
            return Regex.IsMatch(input, @"^(https?://|www\.)\S+\.\S+$", RegexOptions.IgnoreCase);
        }
    }
}
