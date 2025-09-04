using System;
using System.Reflection;
using System.Text.RegularExpressions;
using ChatPlus.Common.Compat;
using ChatPlus.Core.Features.Emojis;
using ChatPlus.Core.Features.ModIcons;
using ChatPlus.Core.Features.Uploads;
using ChatPlus.Core.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Chat;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace ChatPlus.Core.Chat;

internal class DrawChatSystem : ModSystem
{
    public override void Load()
    {
        if (ModLoader.TryGetMod("ChatImprover", out Mod _))
        {
            return;
        }

        On_Main.DrawPlayerChat += DrawChat;
        On_RemadeChatMonitor.DrawChat += DrawMonitor;
    }

    public override void Unload()
    {
        if (ModLoader.TryGetMod("ChatImprover", out Mod _))
        {
            return;
        }

        On_Main.DrawPlayerChat -= DrawChat;
        On_RemadeChatMonitor.DrawChat -= DrawMonitor;
    }

    private void DrawMonitor(On_RemadeChatMonitor.orig_DrawChat orig, RemadeChatMonitor self, bool drawingPlayerChat)
    {
        bool hasUpload = drawingPlayerChat && UploadTagHandler.ContainsUploadTag(Main.chatText);
        if (!hasUpload)
        {
            orig(self, drawingPlayerChat);
            return;
        }
        PlayerInput.WritingText = false;

        int oldH = Main.screenHeight;
        try
        {
            float ui = Main.UIScaleMatrix.M11;
            if (ui <= 0f) ui = 1f;
            int delta = (int)Math.Round(147f / ui);

            Main.screenHeight = Math.Max(0, oldH - delta);
            orig(self, drawingPlayerChat);
        }
        finally
        {
            Main.screenHeight = oldH;
        }
    }

    private void DrawChat(On_Main.orig_DrawPlayerChat orig, Main self)
    {
        if (!Main.drawingPlayerChat) { orig(self); return; }

        PlayerInput.WritingText = true;
        Main.instance.HandleIME();
        if (++Main.instance.textBlinkerCount >= 20) { Main.instance.textBlinkerState = 1 - Main.instance.textBlinkerState; Main.instance.textBlinkerCount = 0; }

        bool hasUpload = UploadTagHandler.ContainsUploadTag(Main.chatText);
        int height = hasUpload ? 147 + 21 : 32; // 147 height + 21 input line height

        int x = 0;
        int y = 0;
        
        DrawChatbox(height, x, y);
        DrawEmojiButton(height, x, y);

        if (!hasUpload)
        {
            DrawInputLine(height, hasUpload, x, y);
            DrawSelectionRectangle(height, x, y);
        }
        else
        {
            int textOffsetX = DrawUploadAndGetTextOffset(height);
            DrawInputLine(height, hasUpload, x, y);
        }

        Main.chatMonitor.DrawChat(true);
    }

    private static float animationSpeed;

    private static void DrawEmojiButton(int height, int xOffset = 0, int yOffset = 0)
    {
        const float animDurationSeconds = 0.1f;
        const float framesPerSecond = 60f;
        const float stepPerFrame = 1f / (animDurationSeconds * framesPerSecond);
        const float minScale = 0.9f;
        const float maxScale = 1.0f;
        const int logicalSize = 24;

        int boxX = 78 + xOffset - 8;
        int boxW = Main.screenWidth - 300;
        int inputY = Main.screenHeight - height + yOffset;
        int pad = 8;

        Vector2 pos = new Vector2(boxX + boxW - pad - logicalSize, inputY + 2);
        Rectangle baseRect = new Rectangle((int)pos.X, (int)pos.Y, logicalSize, logicalSize);
        bool hover = baseRect.Contains(Main.MouseScreen.ToPoint()) && !PlayerInput.IgnoreMouseInterface;

        if (hover) { animationSpeed += stepPerFrame; if (animationSpeed > 1f) animationSpeed = 1f; }
        else { animationSpeed -= stepPerFrame; if (animationSpeed < 0f) animationSpeed = 0f; }

        float scale = MathHelper.Lerp(minScale, maxScale, animationSpeed);

        string tag = "[e:slightly_smiling_face]";
        var snippets = ChatManager.ParseMessage(tag, Color.White).ToArray();
        Vector2 baseSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, snippets, Vector2.One);
        Vector2 scaledSize = baseSize * scale;
        Vector2 offset = (baseSize - scaledSize) * 0.5f;

        ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, 
            snippets, pos + offset, 0f, Vector2.Zero, new Vector2(scale, scale), out int hovered);

        if (hovered >= 0 && Main.mouseLeft && Main.mouseLeftRelease)
        {
            // Open/close emoji state
            if (StateManager.IsAnyStateActive())
                EmojiSystem.CloseAfterCommit();
            else
                EmojiSystem.OpenFromButton();
        }
    }

    private static int DrawUploadAndGetTextOffset(int totalHeight)
    {
        if (!TryGetFirstUploadTexture(Main.chatText, out var tex)) return 0;

        int inputX = 88;
        int inputY = Main.screenHeight - totalHeight;

        float targetH = 147;
        float s = targetH / tex.Height;
        float drawnW = tex.Width * s;

        Main.spriteBatch.Draw(tex, new Vector2(inputX, inputY), null, Color.White, 0f, Vector2.Zero, s, SpriteEffects.None, 0f);
        return (int)Math.Ceiling(drawnW) + 8;
    }

    private static bool TryGetFirstUploadTexture(string s, out Texture2D tex)
    {
        tex = null;
        var m = Regex.Match(s ?? "", @"\[u:([^\]]+)\]", RegexOptions.IgnoreCase);
        if (!m.Success) return false;
        string key = m.Groups[1].Value.Trim();
        return UploadTagHandler.TryGet(key, out tex);
    }

    private static void DrawChatbox(int height, int xOffset=0, int yOffset=0)
    {
        int w = Main.screenWidth - 300;
        int y = Main.screenHeight - 4 - height + yOffset;
        int x = 78 + xOffset;
        DrawNineSlice(x, y, w, height, TextureAssets.TextBack.Value, new Color(100, 100, 100, 100));
    }

    private static void DrawInputLine(int height, bool hasUpload, int xOffset=0, int yOffset = 0)
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
            snips, new Vector2(baseX + xOffset, baseY + yOffset),
            0f, Vector2.Zero, Vector2.One, out _
        );

        // Caret
        if (Main.instance.textBlinkerState == 1)
        {
            int caretRaw = Math.Clamp(HandleChatSystem.GetCaretPos(), 0, raw.Length);
            int caretClean = caretRaw;

            if (tag.Success)
            {
                int tagStart = tag.Index;
                int tagEnd = tag.Index + tag.Length;
                if (caretRaw <= tagEnd) caretClean = 0;                  // inside/at tag → snap to start of cleaned
                else caretClean = caretRaw - tag.Length;                 // after tag → shift left by tag length
            }

            caretClean = Math.Clamp(caretClean, 0, cleaned.Length);
            var before = ChatManager.ParseMessage(cleaned.Substring(0, caretClean), Color.White).ToArray();
            Vector2 beforeSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, before, Vector2.One);

            int caretX = baseX + xOffset + (int)beforeSize.X;
            Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(caretX, baseY + 2+yOffset, 1, 17), Color.White);
        }
    }

    private void DrawSelectionRectangle(int height, int xOffset=0, int yOffset=0)
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
            88 + xOffset + (int)System.Math.Floor(preSize.X),
            Main.screenHeight-height+yOffset,
            (int)System.Math.Ceiling(midSize.X),
            20
        );

        Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, rect, new Color(17, 85, 204) * 0.5f);

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

    private static void DrawDebugRect(Rectangle r) => Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, r, Color.Red * 0.5f);
}
