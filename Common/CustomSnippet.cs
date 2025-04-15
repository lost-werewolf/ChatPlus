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
using Terraria.UI.Chat;

namespace LinksInChat.Common
{
    public class CustomSnippet : TextSnippet
    {
        private readonly TextSnippet _wrappedSnippet;
        private readonly Mod _mod;
        private string text;

        public CustomSnippet(TextSnippet toWrap, Mod mod)
            : base(toWrap.Text, toWrap.Color, toWrap.Scale)
        {
            _wrappedSnippet = toWrap;
            _mod = mod;
            CheckForHover = toWrap.CheckForHover;
            DeleteWhole = toWrap.DeleteWhole;

            // my variables
            this.text = toWrap.Text.Trim(); // remove leading/trailing whitespace
        }

        public override Color GetVisibleColor()
        {
            // Only color recognized links in blue; everything else uses the wrapped snippet's color
            if (URL.IsLink(text))
                return Colors.Blue;

            // Let the wrapped snippet handle color
            return _wrappedSnippet.GetVisibleColor();
        }

        public override bool UniqueDraw(
            bool justCheckingString,
            out Vector2 size,
            SpriteBatch spriteBatch,
            Vector2 position,
            Color color,
            float scale)
        {
            // If it's a link, force "blue" color for drawing
            if (URL.IsLink(text))
                color = Colors.Blue;

            // IMPORTANT: forward the draw call to the wrapped snippet, not base!
            // This ensures we keep the snippet’s original logic/behavior.
            bool result = _wrappedSnippet.UniqueDraw(justCheckingString, out size, spriteBatch, position, color, scale);

            // Draw the underline ourselves if it’s a link and not just measuring
            if (!justCheckingString && URL.IsLink(Text))
            {
                Vector2 textSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, Text, new Vector2(scale));
                float underlineY = position.Y + textSize.Y - 1f;

                Rectangle underlineRect = new Rectangle(
                    (int)position.X,
                    (int)underlineY,
                    (int)textSize.X,
                    1
                );

                // Draw underline in blue (or your snippet color)
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, underlineRect, color);
            }

            return result;
        }

        public override void OnHover()
        {
            // Let the original snippet do its hover behavior
            _wrappedSnippet?.OnHover();

            Log.Info($"Hovering: '{text}'");

            if (URL.IsLink(text))
                UICommon.TooltipMouseText("Open " + text); // TODO better tooltip
        }

        public override void OnClick()
        {
            // Original snippet's click behavior
            _wrappedSnippet?.OnClick();

            // Then open the link if recognized
            if (URL.IsLink(text))
            {
                URL.OpenURL(text);
            }
        }
    }
}
