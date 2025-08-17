using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using AdvancedChatFeatures.Common.Configs;
using AdvancedChatFeatures.Helpers;
using Terraria;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace AdvancedChatFeatures.Common.Hooks
{
    internal class DrawChatHook : ModSystem
    {
        public override void Load()
        {
            On_Main.DrawPlayerChat += DrawPlayerChat;
        }

        public override void Unload()
        {
            On_Main.DrawPlayerChat -= DrawPlayerChat;
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

                int height = 32;

                DrawChatbox(height);
                DrawSelectionRectangle(height);
                DrawInputText(height);
            }
            else
            {
                // Do not force writing mode when chat is closed
                PlayerInput.WritingText = false;
            }

            Main.chatMonitor.DrawChat(Main.drawingPlayerChat); // draws chat monitor
        }

        private void DrawChatbox(int height)
        {
            int width = Main.screenWidth - 300;
            int startX = 78;
            int startY = Main.screenHeight - 4 - height;

            DrawHelper.DrawNineSlice(startX, startY, width, height, TextureAssets.TextBack.Value, new Color(100, 100, 100, 100));
        }
        private void DrawSelectionRectangle(int height)
        {
            var sel = HandleChatHook.GetSelection();
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
            int rawPos = Math.Clamp(HandleChatHook.GetCaretPos(), 0, text.Length);

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
            if (HandleChatHook.GetSelection() == null && Main.instance.textBlinkerState == 1)
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
    }
}
