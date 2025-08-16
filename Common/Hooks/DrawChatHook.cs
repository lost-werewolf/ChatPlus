using System;
using System.Linq;
using System.Text;
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
            if (!Conf.C.featureConfig.EnableBetterChatNavigation)
            {
                //orig(self); 
                //return;
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

            int start = Math.Clamp(sel.Value.start, 0, Main.chatText.Length);
            int end = Math.Clamp(sel.Value.end, 0, Main.chatText.Length);

            if (start >= end) return;

            string pre = Main.chatText.Substring(0, start);
            string mid = Main.chatText.Substring(start, end - start);

            Vector2 preSize = FontAssets.MouseText.Value.MeasureString(pre);
            Vector2 midSize = FontAssets.MouseText.Value.MeasureString(mid);

            Rectangle selRect = new Rectangle(
                x: 88 + (int)preSize.X,
                y: Main.screenHeight - height,
                width: (int)Math.Ceiling(midSize.X),
                height: 20
            );

            Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, selRect, ColorHelper.Blue * 0.5f);
        }

        private void DrawInputText(int height)
        {
            string text = Main.chatText ?? "";
            int pos = Math.Clamp(HandleChatHook.GetCaretPos(), 0, text.Length);

            // Parse the full message into snippets
            TextSnippet[] snippets = ChatManager.ParseMessage(text, Color.White).ToArray();

            // Measure size up to the caret
            string beforeCaret = text.Substring(0, pos);
            Vector2 beforeSize = FontAssets.MouseText.Value.MeasureString(beforeCaret);

            // Draw input text
            ChatManager.DrawColorCodedStringWithShadow(
                Main.spriteBatch,
                FontAssets.MouseText.Value,
                snippets,
                new Vector2(88f, Main.screenHeight - height),
                0f,
                Vector2.Zero,
                Vector2.One,
                out _
            );

            // Draw thick caret
            if (HandleChatHook.GetSelection() == null && Main.instance.textBlinkerState == 1)
            {
                int caretX = 88 + (int)beforeSize.X;
                int caretY = Main.screenHeight - height;

                if (pos == 0)
                {
                    // draw "|" directly at caret position
                    Utils.DrawBorderStringFourWay(Main.spriteBatch, FontAssets.MouseText.Value, "|", caretX, caretY, Color.White, Color.Black, Vector2.Zero);
                }
                else
                {
                    // Draw thin ugly caret
                    Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(caretX, caretY + 2, 1, height: 17), Color.White);
                }
            }
        }
    }
}
