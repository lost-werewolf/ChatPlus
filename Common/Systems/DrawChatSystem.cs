using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ChatPlus.Common.Configs;
using ChatPlus.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Chat;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace ChatPlus.Common.Systems
{
    internal class DrawChatSystem : ModSystem
    {
        public override void Load()
        {
            On_Main.DrawPlayerChat += DrawPlayerChat;
            On_RemadeChatMonitor.DrawChat += DrawMonitor;
        }

        public override void Unload()
        {
            On_Main.DrawPlayerChat -= DrawPlayerChat;
            On_RemadeChatMonitor.DrawChat -= DrawMonitor;
        }

        private void DrawMonitor(On_RemadeChatMonitor.orig_DrawChat orig, RemadeChatMonitor self, bool drawingPlayerChat)
        {
            self.Offset(1);

            orig(self, drawingPlayerChat);
        }

        private void DrawPlayerChat(On_Main.orig_DrawPlayerChat orig, Main self)
        {
            if (!Conf.C.featuresConfig.EnableTextEditingShortcuts)
            {
                orig(self);
                return;
            }

            if (Main.drawingPlayerChat)
            {
                PlayerInput.WritingText = true;
                Main.instance.HandleIME();

                // Update blinker
                Main.instance.textBlinkerCount++;
                if (Main.instance.textBlinkerCount >= 20)
                {
                    Main.instance.textBlinkerState = Main.instance.textBlinkerState == 0 ? 1 : 0;
                    Main.instance.textBlinkerCount = 0;
                }

                // 🔹 Compute extra lines from any [u:...] tags in the input
                //int extraLines = GetExtraUploadLinesFromInput(Main.chatText ?? string.Empty);
                int extraLines = 0;
                const int baseHeight = 32;
                const int lineStep = 20;      // one text line height
                int height = baseHeight + extraLines * lineStep;

                DrawChatbox(height);
                DrawSelectionRectangle(height);
                DrawInputText(height);
            }
            else
            {
                // Do not force writing mode when chat is closed
                PlayerInput.WritingText = false;
            }

            if (Main.chatMonitor is RemadeChatMonitor remade)
            {
                //remade.Offset(0);
            }
            Main.chatMonitor.DrawChat(Main.drawingPlayerChat); // draws chat monitor

        }

        private void DrawChatbox(int height)
        {
            int width = Main.screenWidth - 300;
            int startX = 78;
            int startY = Main.screenHeight - 4 - height;

            DrawNineSlice(startX, startY, width, height, TextureAssets.TextBack.Value, new Color(100, 100, 100, 100));
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
        private void DrawSelectionRectangle(int height)
        {
            var sel = HandleChatSystem.GetSelection();
            if (sel == null) return;

            string text = Main.chatText ?? "";
            int start = Math.Clamp(sel.Value.start, 0, text.Length);
            int end = Math.Clamp(sel.Value.end, 0, text.Length);
            if (start >= end) return;

            // Measure pre and mid with ChatManager so tags get proper collapsed width
            Vector2 preSize = MeasureSnippets(text.Substring(0, start));
            Vector2 midSize = MeasureSnippets(text.Substring(start, end - start));

            var rect = new Rectangle(
                x: 88 + (int)Math.Floor(preSize.X),
                y: Main.screenHeight - height,
                width: (int)Math.Ceiling(midSize.X),
                height: 20
            );

            Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, rect, ColorHelper.Blue * 0.5f);
        }
        private void DrawInputText(int height)
        {
            string text = Main.chatText ?? "";
            int rawPos = Math.Clamp(HandleChatSystem.GetCaretPos(), 0, text.Length);

            // Snap caret if it’s inside a completed tag so visuals match collapsed glyphs
            int pos = SnapCaretInsideClosedTag(text, rawPos);

            // Draw the whole line (this respects tags/glyphs)
            var fullSnips = ChatManager.ParseMessage(text, Color.White).ToArray();
            ChatManager.DrawColorCodedStringWithShadow(
                Main.spriteBatch, FontAssets.MouseText.Value,
                fullSnips, new Vector2(88f, Main.screenHeight - height),
                0f, Vector2.Zero, Vector2.One, out _
            );

            // Measure up to (possibly snapped) caret using parsed snippets too
            Vector2 beforeSize = MeasureSnippets(text.Substring(0, pos));

            // Blinked caret only when no selection
            if (HandleChatSystem.GetSelection() == null && Main.instance.textBlinkerState == 1)
            {
                int caretX = 88 + (int)beforeSize.X;
                int caretY = Main.screenHeight - height;

                // Thin caret (1px) like Terraria
                Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value,
                    new Rectangle(caretX, caretY + 2, 1, 17), Color.White);
            }
        }

        private static int SnapCaretInsideClosedTag(string text, int pos)
        {
            // If caret sits inside a closed [ ... ] tag, snap it to the tag's end.
            foreach (Match m in Regex.Matches(text ?? "", @"\[[^\]]+\]"))
            {
                int s = m.Index, e = s + m.Length;
                if (pos > s && pos <= e) return e;
            }
            return pos;
        }

        private static Vector2 MeasureSnippets(string s)
        {
            var snips = ChatManager.ParseMessage(s ?? "", Color.White).ToArray();
            return ChatManager.GetStringSize(FontAssets.MouseText.Value, snips, Vector2.One);
        }

        #region helpers
        private static int GetExtraUploadLinesFromInput(string input)
        {
            if (string.IsNullOrEmpty(input))
                return 0;

            // find every [u: ... ] tag
            var matches = Regex.Matches(input, @"\[u:[^\]]+\]");
            if (matches.Count == 0)
                return 0;

            int maxExtra = 0;
            foreach (Match m in matches)
            {
                string tag = m.Value;
                float size = ExtractUploadSize(tag);

                // rule: 0–20 => 0, 20–40 => 1, 40–60 => 2, ...
                // i.e. ceil((size - 20) / 20), but never below 0
                int extra = (int)Math.Ceiling(Math.Max(0f, size - 20f) / 20f);

                // optional cap (keep things sane)
                if (extra > 8) extra = 8;

                if (extra > maxExtra)
                    maxExtra = extra;
            }
            return maxExtra;
        }

        private static float ExtractUploadSize(string tag)
        {
            // default size if none specified
            float size = 20f;

            // prefer explicit size=XX
            var m = Regex.Match(tag, @"size\s*=\s*([0-9]+(?:\.[0-9]+)?)", RegexOptions.IgnoreCase);
            if (m.Success)
            {
                if (float.TryParse(m.Groups[1].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var v))
                    return v;
            }

            // fallback: shorthand [u:key|NN]
            var m2 = Regex.Match(tag, @"\[u:[^|\]]+\|([0-9]+(?:\.[0-9]+)?)", RegexOptions.IgnoreCase);
            if (m2.Success)
            {
                if (float.TryParse(m2.Groups[1].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var v))
                    return v;
            }

            return size;
        }
        #endregion
    }
}
