using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Chat;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace ChatPlus.Core.Chat
{
    /// <summary>
    /// Encapsulates a UIScrollbar for the chat log and synchronizes its position to
    /// the vanilla RemadeChatMonitor (line-based offset).
    /// </summary>
    internal sealed class ChatScrollbar : IDisposable
    {
        // UI & state
        private readonly UserInterface chatInterface;
        private readonly ChatScrollUi chatUi;

        // Mirrored chat history; used to estimate total content height
        private readonly List<ChatEntry> chatHistory = [];

        // Current desired line offset to push into vanilla just before draw
        private int currentOffsetLines;

        // Layout provider lets the owner define geometry without this class
        // knowing about uploads, extra input height, etc.
        public Func<Layout> LayoutProvider { get; set; }

        // Tuning knobs
        public int MaxVisibleLines { get; set; } = 20;

        public ChatScrollbar()
        {
            chatInterface = new UserInterface();
            chatUi = new ChatScrollUi();
            chatUi.Activate();
            chatInterface.SetState(chatUi);

            // Default layout if caller does not supply one
            LayoutProvider = () =>
            {
                int boxX = 78;
                int boxWidth = Main.screenWidth - 300;
                int inputHeight = 32; // vanilla input line height
                return new Layout(boxX, boxWidth, inputHeight);
            };
        }

        // --- Public API for your ModSystem ---

        public void AttachHooks()
        {
            On_RemadeChatMonitor.NewText += CaptureNewText;
            On_RemadeChatMonitor.NewTextMultiline += CaptureNewTextMultiline;
        }

        public void DetachHooks()
        {
            On_RemadeChatMonitor.NewText -= CaptureNewText;
            On_RemadeChatMonitor.NewTextMultiline -= CaptureNewTextMultiline;
        }

        /// <summary>
        /// Call once per frame from ModSystem.UpdateUI.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            // Only visible when the chat is open
            chatUi.IsVisible = Main.drawingPlayerChat;
            if (!chatUi.IsVisible)
                return;

            UpdateLayoutAndSizing();

            chatInterface.Update(gameTime);

            // Map pixel position to vanilla line offset
            int lineHeight = FontAssets.MouseText.Value.LineSpacing; // ~20 at UI scale 1
            int desiredLines = (int)Math.Round(chatUi.Scrollbar.ViewPosition / Math.Max(1, lineHeight));
            currentOffsetLines = Math.Max(0, desiredLines);
        }

        /// <summary>
        /// Insert a layer to draw the scrollbar on top of the vanilla chat.
        /// Call from ModSystem.ModifyInterfaceLayers.
        /// </summary>
        public void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int anchor = layers.FindIndex(l => l.Name.Equals("Vanilla: Chat", StringComparison.Ordinal));
            if (anchor == -1)
                return;

            layers.Insert(anchor + 1, new LegacyGameInterfaceLayer(
                "ChatPlus: Chat Scrollbar",
                () =>
                {
                    if (chatUi.IsVisible)
                        chatInterface.Draw(Main.spriteBatch, Main.gameTimeCache);
                    return true;
                },
                InterfaceScaleType.UI));
        }

        /// <summary>
        /// Apply the current scrollbar position to vanilla. Call this immediately
        /// before you invoke the original DrawChat in your DrawChat hook.
        /// </summary>
        public void ApplyTo(RemadeChatMonitor monitor)
        {
            monitor.ResetOffset();                  // start at bottom (latest)
            if (currentOffsetLines > 0)
                monitor.Offset(currentOffsetLines); // scroll up by N text lines
        }

        public void Dispose()
        {
            DetachHooks();
            chatHistory.Clear();
        }

        // --- Hooks: mirror new chat so we can size the knob ---

        private void CaptureNewText(On_RemadeChatMonitor.orig_NewText orig, RemadeChatMonitor self, string newText, byte r, byte g, byte b)
        {
            chatHistory.Add(new ChatEntry(newText ?? string.Empty, new Color(r, g, b), -1));
            if (chatHistory.Count > 600) chatHistory.RemoveAt(0);
            orig(self, newText, r, g, b);
        }

        private void CaptureNewTextMultiline(On_RemadeChatMonitor.orig_NewTextMultiline orig, RemadeChatMonitor self, string text, bool force, Color c, int widthLimit)
        {
            chatHistory.Add(new ChatEntry(text ?? string.Empty, c, widthLimit));
            if (chatHistory.Count > 600) chatHistory.RemoveAt(0);
            orig(self, text, force, c, widthLimit);
        }

        // --- Sizing & layout ---

        private void UpdateLayoutAndSizing()
        {
            var layout = LayoutProvider?.Invoke() ?? default;
            if (layout.BoxWidth <= 0)
                return;

            int lineHeight = FontAssets.MouseText.Value.LineSpacing;
            int viewHeight = Math.Min(MaxVisibleLines * lineHeight, (int)(Main.screenHeight * 0.45f));

            int scrollbarWidth = 16;
            int right = layout.BoxX + layout.BoxWidth;
            int viewBottom = Main.screenHeight - layout.InputHeight - 6;
            int viewTop = viewBottom - viewHeight;

            chatUi.Scrollbar.Left.Set(right - scrollbarWidth - 6, 0f);
            chatUi.Scrollbar.Top.Set(viewTop, 0f);
            chatUi.Scrollbar.Width.Set(scrollbarWidth, 0f);
            chatUi.Scrollbar.Height.Set(viewHeight, 0f);
            chatUi.Recalculate();

            // Estimate total content height (in pixels) at the current text width.
            // We approximate wrapping by dividing unwrapped width by wrapWidth
            // and counting explicit newlines.
            int wrapWidth = layout.BoxWidth - scrollbarWidth - 16;
            wrapWidth = Math.Max(50, wrapWidth);

            float totalPixels = 0f;

            foreach (var entry in chatHistory)
            {
                int effectiveWidth = entry.WidthLimit > 0 ? Math.Min(entry.WidthLimit, wrapWidth) : wrapWidth;

                var snips = ChatManager.ParseMessage(entry.Text, entry.Color).ToArray();
                var size = ChatManager.GetStringSize(FontAssets.MouseText.Value, snips, Vector2.One);

                // Base line count from unwrapped width
                int lines = Math.Max(1, (int)Math.Ceiling(size.X / Math.Max(1, effectiveWidth)));

                // Add explicit newline breaks conservatively
                int extraBreaks = CountNewlines(entry.Text);
                lines = Math.Max(lines, 1 + extraBreaks);

                totalPixels += lines * lineHeight;
            }

            if (totalPixels < 1f)
                totalPixels = 1f;

            chatUi.Scrollbar.SetView(viewHeight, Math.Max(viewHeight, (int)Math.Ceiling(totalPixels)));
        }

        private static int CountNewlines(string s)
        {
            if (string.IsNullOrEmpty(s)) return 0;
            int n = 0;
            for (int i = 0; i < s.Length; i++)
                if (s[i] == '\n') n++;
            return n;
        }

        // --- Nested types ---

        public readonly record struct Layout(int BoxX, int BoxWidth, int InputHeight);

        private readonly record struct ChatEntry(string Text, Color Color, int WidthLimit);

        private sealed class ChatScrollUi : UIState
        {
            public UIScrollbar Scrollbar { get; } = new UIScrollbar();
            public bool IsVisible { get; set; }

            public override void OnInitialize()
            {
                Append(Scrollbar);
            }
        }
    }
}
