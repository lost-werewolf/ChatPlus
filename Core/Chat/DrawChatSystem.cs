using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Chat;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace ChatPlus.Core.Chat
{
    internal class DrawChatSystem : ModSystem
    {
        const int BaseHeight = 32;   // vanilla input line height
        const int ExtraH = 180;      // image height = 10 lines * 20px
        const int Expanded = BaseHeight + ExtraH; // 232

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
            bool hasUpload = drawingPlayerChat && HasUpload(Main.chatText);

            if (!hasUpload)
            {
                orig(self, drawingPlayerChat);
                return;
            }

            // Lift monitor so it renders above the taller input area
            var sb = Main.spriteBatch;
            sb.End();
            var lifted = Main.UIScaleMatrix * Matrix.CreateTranslation(0f, -ExtraH, 0f);
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp,
                     DepthStencilState.None, RasterizerState.CullCounterClockwise, null, lifted);

            orig(self, true);

            sb.End();
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp,
                     DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.UIScaleMatrix);
        }

        private void DrawPlayerChat(On_Main.orig_DrawPlayerChat orig, Main self)
        {
            bool hasUpload = HasUpload(Main.chatText);
            int height = hasUpload ? Expanded - 20 : BaseHeight;

            if (!Main.drawingPlayerChat)
            {
                orig(self);
                return;
            }

            PlayerInput.WritingText = true;
            Main.instance.HandleIME();

            // caret blink
            if (++Main.instance.textBlinkerCount >= 20)
            {
                Main.instance.textBlinkerState = 1 - Main.instance.textBlinkerState;
                Main.instance.textBlinkerCount = 0;
            }

            DrawChatbox(height);

            if (!hasUpload)
            {
                DrawInputLine(height, textOffsetX: 0, hasUpload: false);
                DrawSelectionRectangle(height, Main.screenHeight - height, 0);
            }
            else
            {
                int textOffsetX = DrawUploadAndGetTextOffset(height);
                DrawInputLine(height, textOffsetX, hasUpload: true);
            }

            Main.chatMonitor.DrawChat(true);
        }


        // --- helpers ---

        private static bool HasUpload(string s) =>
            !string.IsNullOrEmpty(s) && Regex.IsMatch(s, @"\[u:[^\]]+\]", RegexOptions.IgnoreCase);

        private static int DrawUploadAndGetTextOffset(int totalHeight)
        {
            // Draw the first upload image at 200px tall, return text X offset (image width + padding)
            if (!TryGetFirstUploadTexture(Main.chatText, out var tex)) return 0;

            int inputX = 88;
            int inputY = Main.screenHeight - totalHeight;

            float targetH = ExtraH; // 200px
            float s = targetH / System.Math.Max(tex.Width, tex.Height);
            float drawnW = tex.Width * s;

            Main.spriteBatch.Draw(tex, new Vector2(inputX, inputY + 2), null, Color.White, 0f, Vector2.Zero, s, SpriteEffects.None, 0f);
            return (int)System.Math.Ceiling(drawnW) + 8;
        }

        private static bool TryGetFirstUploadTexture(string s, out Texture2D tex)
        {
            tex = null;
            var m = Regex.Match(s ?? "", @"\[u:([^\]]+)\]", RegexOptions.IgnoreCase);
            if (!m.Success) return false;
            string key = m.Groups[1].Value.Trim();
            return UploadHandler.UploadTagHandler.TryGet(key, out tex);
        }

        private static void DrawChatbox(int height)
        {
            int w = Main.screenWidth - 300;
            int x = 78;
            int y = Main.screenHeight - 4 - height;
            DrawNineSlice(x, y, w, height, TextureAssets.TextBack.Value, new Color(100, 100, 100, 100));
        }

        private static void DrawInputLine(int height, int textOffsetX, bool hasUpload)
        {
            int baseX = 88;
            int baseY = Main.screenHeight - height;

            // Strip upload tag from rendered text so input appears to the right of the image.
            string raw = Main.chatText ?? "";
            var tag = Regex.Match(raw, @"\[u:[^\]]+\]", RegexOptions.IgnoreCase);
            string cleaned = tag.Success ? raw.Remove(tag.Index, tag.Length) : raw;

            // Draw text
            var snips = ChatManager.ParseMessage(cleaned, Color.White).ToArray();
            ChatManager.DrawColorCodedStringWithShadow(
                Main.spriteBatch, FontAssets.MouseText.Value,
                snips, new Vector2(baseX + textOffsetX, baseY),
                0f, Vector2.Zero, Vector2.One, out _
            );

            // Caret
            if (Main.instance.textBlinkerState == 1)
            {
                int caretRaw = System.Math.Clamp(HandleChatSystem.GetCaretPos(), 0, raw.Length);
                int caretClean = caretRaw;

                if (tag.Success)
                {
                    int tagStart = tag.Index;
                    int tagEnd = tag.Index + tag.Length;
                    if (caretRaw <= tagEnd) caretClean = 0;                  // inside/at tag → snap to start of cleaned
                    else caretClean = caretRaw - tag.Length;                 // after tag → shift left by tag length
                }

                caretClean = System.Math.Clamp(caretClean, 0, cleaned.Length);
                var before = ChatManager.ParseMessage(cleaned.Substring(0, caretClean), Color.White).ToArray();
                Vector2 beforeSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, before, Vector2.One);

                int caretX = baseX + textOffsetX + (int)beforeSize.X;
                Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(caretX, baseY + 2, 1, 17), Color.White);
            }
        }

        private void DrawSelectionRectangle(int height, int baselineY, int textOffsetX)
        {
            var sel = HandleChatSystem.GetSelection();
            if (sel == null) return;

            string raw = Main.chatText ?? "";
            var tag = Regex.Match(raw, @"\[u:[^\]]+\]", RegexOptions.IgnoreCase);

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

            Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, rect, ColorHelper.Blue * 0.5f);

            static Vector2 MeasureSnippets(string s)
            {
                var snips = ChatManager.ParseMessage(s ?? "", Color.White).ToArray();
                return ChatManager.GetStringSize(FontAssets.MouseText.Value, snips, Vector2.One);
            }
        }

        private static void DrawNineSlice(int x, int y, int w, int h, Texture2D tex, Color color)
        {
            int c = 10;
            int ew = tex.Width - c * 2;
            int eh = tex.Height - c * 2;

            var sb = Main.spriteBatch;
            sb.Draw(tex, new Vector2(x, y), new Rectangle(0, 0, c, c), color);
            sb.Draw(tex, new Rectangle(x + c, y, w - c * 2, c), new Rectangle(c, 0, ew, c), color);
            sb.Draw(tex, new Vector2(x + w - c, y), new Rectangle(tex.Width - c, 0, c, c), color);

            sb.Draw(tex, new Rectangle(x, y + c, c, h - c * 2), new Rectangle(0, c, c, eh), color);
            sb.Draw(tex, new Rectangle(x + c, y + c, w - c * 2, h - c * 2), new Rectangle(c, c, ew, eh), color);
            sb.Draw(tex, new Rectangle(x + w - c, y + c, c, h - c * 2), new Rectangle(tex.Width - c, c, c, eh), color);

            sb.Draw(tex, new Vector2(x, y + h - c), new Rectangle(0, tex.Height - c, c, c), color);
            sb.Draw(tex, new Rectangle(x + c, y + h - c, w - c * 2, c), new Rectangle(c, tex.Height - c, ew, c), color);
            sb.Draw(tex, new Vector2(x + w - c, y + h - c), new Rectangle(tex.Width - c, tex.Height - c, c, c), color);
        }
    }
}
