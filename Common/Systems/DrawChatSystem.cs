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
            if (ModLoader.TryGetMod("ChatImprover", out Mod _))
            {
                return;
            }

            On_Main.DrawPlayerChat += DrawPlayerChat;
            On_RemadeChatMonitor.DrawChat += DrawMonitor;
        }

        public override void Unload()
        {
            if (ModLoader.TryGetMod("ChatImprover", out Mod _))
            {
                return;
            }

            On_Main.DrawPlayerChat -= DrawPlayerChat;
            On_RemadeChatMonitor.DrawChat -= DrawMonitor;
        }

        private void DrawMonitor(On_RemadeChatMonitor.orig_DrawChat orig, RemadeChatMonitor self, bool drawingPlayerChat)
        {
            bool hasUpload =
                drawingPlayerChat &&
                !string.IsNullOrEmpty(Main.chatText) &&
                System.Text.RegularExpressions.Regex.IsMatch(Main.chatText, @"\[u:[^\]]+\]", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            if (!hasUpload)
            {
                orig(self, drawingPlayerChat);
                return;
            }

            const int lift = 200; // 10 lines * 20px

            var sb = Main.spriteBatch;

            sb.End();
            var lifted = Main.UIScaleMatrix * Microsoft.Xna.Framework.Matrix.CreateTranslation(0f, -lift, 0f);
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, lifted);

            // draw the monitor with the lifted matrix so its first line starts higher
            orig(self, true);

            // restore the normal UI batch for everything else (your input box, caret, etc.)
            sb.End();
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.UIScaleMatrix);
        }

        private void DrawPlayerChat(On_Main.orig_DrawPlayerChat orig, Main self)
        {
            if (!Conf.C.featuresConfig.EnableTextEditingShortcuts)
            {
                orig(self);
                return;
            }

            bool hasUpload = !string.IsNullOrEmpty(Main.chatText) &&
                             System.Text.RegularExpressions.Regex.IsMatch(Main.chatText, @"\[u:[^\]]+\]", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            int extraLines = hasUpload ? 10 : 0;
            const int baseHeight = 32;
            const int lineStep = 20;
            int height = baseHeight + extraLines * lineStep;

            if (Main.drawingPlayerChat)
            {
                Terraria.GameInput.PlayerInput.WritingText = true;
                Main.instance.HandleIME();

                Main.instance.textBlinkerCount++;
                if (Main.instance.textBlinkerCount >= 20)
                {
                    Main.instance.textBlinkerState = Main.instance.textBlinkerState == 0 ? 1 : 0;
                    Main.instance.textBlinkerCount = 0;
                }

                if (hasUpload)
                {
                    Main.chatMonitor.DrawChat(true);
                }

                DrawChatbox(height);

                int inputX = 88;
                int inputY = Main.screenHeight - height;

                int textOffsetX = 0;

                if (hasUpload && TryGetFirstUploadTexture(Main.chatText, out var tex))
                {
                    float targetH = 200f; // 10 lines * 20px
                    float s = targetH / System.Math.Max(tex.Width, tex.Height);
                    float drawnW = tex.Width * s;

                    Main.spriteBatch.Draw(tex, new Vector2(inputX, inputY + 2), null, Color.White, 0f, Vector2.Zero, s, SpriteEffects.None, 0f);

                    textOffsetX = (int)System.Math.Ceiling(drawnW) + 8;
                }

                DrawSelectionRectangle(height, inputY, textOffsetX);
                DrawInputText(height, inputY, textOffsetX);
            }
            else
            {
                Terraria.GameInput.PlayerInput.WritingText = false;
            }

            // when no upload is active, keep vanilla order (monitor after input draw)
            if (!hasUpload)
            {
                Main.chatMonitor.DrawChat(Main.drawingPlayerChat);
            }

            static bool TryGetFirstUploadTexture(string s, out Texture2D tex)
            {
                tex = null;
                var m = System.Text.RegularExpressions.Regex.Match(s ?? "", @"\[u:([^\]\|]+)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                if (!m.Success) return false;
                string key = m.Groups[1].Value;
                return UploadHandler.UploadTagHandler.TryGet(key, out tex);
            }
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
        private void DrawSelectionRectangle(int height, int baselineY, int textOffsetX)
        {
            var sel = HandleChatSystem.GetSelection();
            if (sel == null) return;

            string raw = Main.chatText ?? "";
            var tag = System.Text.RegularExpressions.Regex.Match(raw, @"\[u:[^\]]+\]", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            int start = sel.Value.start;
            int end = sel.Value.end;

            if (tag.Success)
            {
                int tagStart = tag.Index;
                int tagEnd = tag.Index + tag.Length;

                if (end <= tagStart)
                {
                    start = 0;
                    end = 0;
                }
                else if (start < tagEnd && end > tagStart)
                {
                    start = tagEnd;
                }

                if (start < tagEnd) start = tagEnd;
                if (end < start) end = start;
            }

            string cleaned = tag.Success ? raw.Remove(tag.Index, tag.Length) : raw;

            int adjStart = tag.Success ? System.Math.Max(0, start - tag.Length) : start;
            int adjEnd = tag.Success ? System.Math.Max(0, end - tag.Length) : end;

            adjStart = System.Math.Clamp(adjStart, 0, cleaned.Length);
            adjEnd = System.Math.Clamp(adjEnd, 0, cleaned.Length);
            if (adjStart >= adjEnd) return;

            Vector2 preSize = MeasureSnippets(cleaned.Substring(0, adjStart));
            Vector2 midSize = MeasureSnippets(cleaned.Substring(adjStart, adjEnd - adjStart));

            var rect = new Rectangle(
                88 + textOffsetX + (int)System.Math.Floor(preSize.X),
                baselineY,
                (int)System.Math.Ceiling(midSize.X),
                20
            );

            Main.spriteBatch.Draw(Terraria.GameContent.TextureAssets.MagicPixel.Value, rect, ColorHelper.Blue * 0.5f);

            static Vector2 MeasureSnippets(string s)
            {
                var snips = ChatManager.ParseMessage(s ?? "", Color.White).ToArray();
                return ChatManager.GetStringSize(Terraria.GameContent.FontAssets.MouseText.Value, snips, Vector2.One);
            }
        }

        private void DrawInputText(int height, int baselineY, int textOffsetX)
        {
            string raw = Main.chatText ?? "";
            int caretRaw = System.Math.Clamp(HandleChatSystem.GetCaretPos(), 0, raw.Length);

            var tag = System.Text.RegularExpressions.Regex.Match(raw, @"\[u:[^\]]+\]", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            string cleaned;
            int caretClean;

            if (!tag.Success)
            {
                cleaned = raw;
                caretClean = caretRaw;
            }
            else
            {
                cleaned = raw.Remove(tag.Index, tag.Length);

                if (caretRaw <= tag.Index + tag.Length) caretClean = 0;
                else caretClean = caretRaw - tag.Length;
            }

            var snips = ChatManager.ParseMessage(cleaned, Color.White).ToArray();
            ChatManager.DrawColorCodedStringWithShadow(
                Main.spriteBatch, Terraria.GameContent.FontAssets.MouseText.Value,
                snips, new Vector2(88f + textOffsetX, baselineY),
                0f, Vector2.Zero, Vector2.One, out _
            );

            if (HandleChatSystem.GetSelection() == null && Main.instance.textBlinkerState == 1)
            {
                Vector2 beforeSize = MeasureSnippets(cleaned.Substring(0, System.Math.Clamp(caretClean, 0, cleaned.Length)));
                int caretX = 88 + textOffsetX + (int)beforeSize.X;
                Main.spriteBatch.Draw(Terraria.GameContent.TextureAssets.MagicPixel.Value, new Rectangle(caretX, baselineY + 2, 1, 17), Color.White);
            }

            static Vector2 MeasureSnippets(string s)
            {
                var ss = ChatManager.ParseMessage(s ?? "", Color.White).ToArray();
                return ChatManager.GetStringSize(Terraria.GameContent.FontAssets.MouseText.Value, ss, Vector2.One);
            }
        }
    }
}
