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
using Terraria.UI.Chat;

namespace AdvancedChatFeatures.Common.Snippets
{
    /// <summary>
    /// A custom snippet that changes drawing behavior for chat messages. 
    /// Features:
    /// 1. Detects and styles links in chat messages and opens them in a web browser when clicked.
    /// 2. Draws a player head icon at the start of the link.
    /// <see cref="NewMessageHook"/>
    /// </summary>
    public class LinkSnippet : TextSnippet
    {
        private TextSnippet snippet;
        private string text;

        // Hover rectangle for the link
        private Rectangle lastDrawRect = Rectangle.Empty;
        private int lastUnderlineDrawFrame = -1;

        public LinkSnippet(TextSnippet snippet): base(snippet.Text, snippet.Color, snippet.Scale)
        {
            this.snippet = snippet;
            CheckForHover = snippet.CheckForHover;
            DeleteWhole = snippet.DeleteWhole;
            text = snippet.Text.Trim();
        }

        public override Color GetVisibleColor()
        {
            // Always use a link color for link snippets.
            if (LinkHelper.IsWholeLink(text))
            {
                // Optionally, you can choose a different color if the mouse is hovering.
                // Here we check if Main.MouseScreen is within lastDrawRect.
                bool isHovered = lastDrawRect.Contains(Main.MouseScreen.ToPoint());
                if (isHovered)
                {
                    return ColorHelper.BlueHover;
                }
                else
                {
                    return ColorHelper.Blue;
                }
            }
            return snippet.GetVisibleColor();
        }

        public override void Update()
        {
            snippet.Update();
        }

        public override bool UniqueDraw(bool justCheckingString, out Vector2 size, SpriteBatch spriteBatch, Vector2 position = default, Color color = default, float scale = 1)
        {
            // Calculate the text size for this snippet
            Vector2 textSize = FontAssets.MouseText.Value.MeasureString(text) * scale;
            lastDrawRect = new Rectangle((int)position.X, (int)position.Y, (int)textSize.X, (int)textSize.Y);

            // Offset the text so it doesn't overlap the head
            Vector2 textPosition = position + new Vector2(12f, 0f);

            // Draw link underline
            if (!justCheckingString && LinkHelper.IsWholeLink(text))
            {
                if (Main.GameUpdateCount != lastUnderlineDrawFrame)
                {
                    lastUnderlineDrawFrame = (int)Main.GameUpdateCount;
                    Rectangle underlineRect = new((int)textPosition.X, (int)(textPosition.Y + textSize.Y - 9), (int)textSize.X, 1);
                    Color underlineColor = GetVisibleColor();
                    spriteBatch.Draw(TextureAssets.MagicPixel.Value, underlineRect, underlineColor);
                }
            }

            // Draw the wrapped snippet'text text at the offset position
            return snippet.UniqueDraw(justCheckingString, out size, spriteBatch, textPosition, color, scale);
        }

        public override void OnClick()
        {
            snippet?.OnClick();
            if (LinkHelper.IsWholeLink(text))
                LinkHelper.OpenURL(text);
        }
    }
}
