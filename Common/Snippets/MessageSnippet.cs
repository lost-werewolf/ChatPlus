using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using AdvancedChatFeatures.Common.Hooks;
using AdvancedChatFeatures.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;
using Terraria.UI.Chat;

namespace AdvancedChatFeatures.Common.Snippets
{
    // Changes the style, behaviour and functionality of a TextSnippet.
    // In this case the MessageSnippet is extracted to be only the "message" part 
    // that is sent only when a message is sent by a PLAYER. 
    // See AddNewMessageHook for more details.
    public class MessageSnippet : TextSnippet
    {
        private TextSnippet _wrappedSnippet;
        private string text; // trimmed text for consistency

        // Holds the bounding rectangle from the last draw call.
        private Rectangle lastDrawRect = Rectangle.Empty;
        // Track the update frame when we last drew the underline so we only do it once per frame.
        private int lastUnderlineDrawFrame = -1;

        public MessageSnippet(TextSnippet toWrap, Mod mod)
            : base(toWrap.Text, toWrap.Color, toWrap.Scale)
        {
            _wrappedSnippet = toWrap;
            CheckForHover = toWrap.CheckForHover;
            DeleteWhole = toWrap.DeleteWhole;
            text = toWrap.Text.Trim();
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
            // Calculate the text size for this snippet
            Vector2 textSize = FontAssets.MouseText.Value.MeasureString(text) * scale;
            lastDrawRect = new Rectangle((int)position.X, (int)position.Y, (int)textSize.X, (int)textSize.Y);

            Vector2 headPosition = new(65 + ChatPosHook.OffsetX, lastDrawRect.Y + 6); // Draw at the start of the snippet
            DrawHelper.DrawPlayerHead(headPosition);

            // Offset the text so it doesn't overlap the head
            Vector2 textPosition = position + new Vector2(12f, 0f);

            // Draw underline for links
            if (!justCheckingString && IsWholeLink(text))
            {
                if (Main.GameUpdateCount != lastUnderlineDrawFrame)
                {
                    lastUnderlineDrawFrame = (int)Main.GameUpdateCount;
                    Rectangle underlineRect = new Rectangle((int)textPosition.X, (int)(textPosition.Y + textSize.Y - 9), (int)textSize.X, 1);
                    Color underlineColor = GetVisibleColor();
                    spriteBatch.Draw(TextureAssets.MagicPixel.Value, underlineRect, underlineColor);
                }
            }

            // Draw hover effect for links
            if (IsWholeLink(text) && lastDrawRect.Contains(Main.MouseScreen.ToPoint()))
                DrawLinkHover(spriteBatch);

            // Draw the wrapped snippet's text at the offset position
            return _wrappedSnippet.UniqueDraw(justCheckingString, out size, spriteBatch, textPosition, color, scale);
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
        private static bool IsWholeLink(string s)
            => Regex.IsMatch(s, @"^(https?://|www\.)\S+\.\S+$", RegexOptions.IgnoreCase);
    }
}
