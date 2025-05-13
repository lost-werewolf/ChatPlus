using System;
using System.Collections.Generic;
using AdvancedChatFeatures.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Localization.IME;
using Terraria;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.ModLoader.Config.UI;
using Terraria.UI;
using Terraria.UI.Chat;

namespace AdvancedChatFeatures.Common.Configs
{
    /// <summary>
    /// Reference:
    /// <see cref="Terraria.ModLoader.Config.UI.ConfigElement"/> 
    /// </summary>
    public class ChatBoxPreviewElement : ConfigElement
    {
        // Blinker state
        private int textBlinkerCount;
        private bool textBlinkerVisible;

        private float chatXOffset = 40;

        // Called once when the config UI binds this element
        public override void OnBind()
        {
            base.OnBind();
            // Fill available width, centered in parent
            Width = StyleDimension.Fill;
            HAlign = 0.5f;
            VAlign = 0.5f;
            // Enough height for 3 messages + chat box
            MinHeight = new StyleDimension(30 + 30 * 4, 0);
            Label = string.Empty;
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
            DrawPlayersIfOn(sb);
            DrawConfigIfOn(sb);
            DrawChatMessagePreview(sb);
            DrawChatBoxPreview(sb);
        }

        private void DrawPlayersIfOn(SpriteBatch sb)
        {
            if (Main.LocalPlayer == null)
            {
                Log.Error("err localplayer null players!");
                return;
            }

            if (!Conf.C.ShowPlayerIcons)
            {
                chatXOffset = 40f;
                return;
            }

            // Update chat X offset
            chatXOffset = 40f + 20f;

            // Draw player head icon at top-left of this element
            Player player = Main.LocalPlayer;
            CalculatedStyle dims = GetDimensions();

            int count = 2 + (Conf.C.ShowLinks ? 1 : 0);
            const int maxSlots = 3; // total possible icon lines
            // PlayerHeadFlipSystem.shouldFlipHeadDraw = player.direction == -1;
            for (int i = 0; i < count; i++)
            {
                // bottom‑align: skip the top (maxSlots - count) slots
                float y = 30 * (maxSlots - count + i);
                Vector2 pos = new(dims.X + 56 - 10f, dims.Y + 18f + y);
                Main.MapPlayerRenderer.DrawPlayerHead(
                Main.Camera,
                player,
                pos,
                1f,
                0.6f,
                Color.White
                        );
            }
            // PlayerHeadFlipSystem.shouldFlipHeadDraw = false;
        }

        private void DrawConfigIfOn(SpriteBatch sb)
        {
            if (!Conf.C.ShowConfigIcon)
                return;

            // Draw settings cog to left of preview box chat
            DrawHelper.DrawProperScale(
                sb,
                element: this,
                tex: Ass.ButtonModConfig.Value,
                scale: 0.15f,
                x: 8,
                y: 112
            );
        }

        private void DrawChatBoxPreview(SpriteBatch sb)
        {
            var dims = GetDimensions();
            int panelW = TextureAssets.TextBack.Width();
            int panelH = TextureAssets.TextBack.Height();

            // Left‑padding inside this element:
            const float paddingX = 10f + 30f;
            const float paddingY = 10f;

            // Position the chat box flush to the left + bottom‑padding:
            Vector2 panelPos = new Vector2(
                dims.X + paddingX,
                dims.Y + dims.Height - panelH - paddingY
            );

            // Draw the panel
            sb.Draw(
                TextureAssets.TextBack.Value,
                panelPos,
                null,
                new Color(100, 100, 100, 100),
                0f,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                0f
            );

            // Blinker update
            textBlinkerCount++;
            if (textBlinkerCount >= 20)
            {
                textBlinkerVisible = !textBlinkerVisible;
                textBlinkerCount = 0;
            }

            // Build preview text + cursor
            string previewText = Main.chatText ?? string.Empty;
            if (textBlinkerVisible)
                previewText += "|";

            if (!IsMouseHovering)
            {
                Main.drawingPlayerChat = false;
                PlayerInput.WritingText = false;
                return;
            }

            var textSnips = ChatManager.ParseMessage(previewText, Color.White).ToArray();

            // Draw the input text inside the box with a small inset:
            ChatManager.DrawColorCodedStringWithShadow(
                sb,
                FontAssets.MouseText.Value,
                textSnips,
                panelPos + new Vector2(6f, 6f),
                0f,
                Vector2.Zero,
                Vector2.One,
                out _
            );
        }

        private void DrawChatMessagePreview(SpriteBatch sb)
        {
            if (Main.LocalPlayer == null)
            {
                Log.Error("err localplayer null!");
                return;
            }

            var dims = GetDimensions();
            int panelH = TextureAssets.TextBack.Height();
            float paddingX = chatXOffset;
            float paddingY = 10f;
            Vector2 panelPos = new(dims.X + paddingX, dims.Y + dims.Height - panelH - paddingY);
            float lineHeight = FontAssets.MouseText.Value.MeasureString("M").Y + 4f;

            // Build a list of (prefix, message) pairs up‑front:
            var chatLines = new List<KeyValuePair<string, string>>
            {
                new(FormatPrefix(Main.LocalPlayer.name + "1"),      " How are you?"),
                new(FormatPrefix(Main.LocalPlayer.name + "2"), " Good, thank you!")
            };

            // Add link
            if (Conf.C.ShowLinks)
                chatLines.Add(new KeyValuePair<string, string>(
                    FormatPrefix(Main.LocalPlayer.name + "3"),
                    ""
                ));

            // Calc total height of all lines
            float totalMsgHeight = chatLines.Count * lineHeight;
            float startY = panelPos.Y - totalMsgHeight - 4f;

            // Loop through them and draw prefix (always white) + message (colored/underlined if needed)
            for (int i = 0; i < chatLines.Count; i++)
            {
                var (prefix, msg) = chatLines[i];
                Vector2 pos = new Vector2(panelPos.X + 2f, startY + i * lineHeight);

                // Draw player color
                Color msgColor = Conf.C.PlayerColors
                    ? ColorHelper.PlayerColors[i % 3]
                    : Color.White;

                var preSnips = ChatManager.ParseMessage(prefix, msgColor).ToArray();
                ChatManager.DrawColorCodedStringWithShadow(
                    sb, FontAssets.MouseText.Value, preSnips, pos, 0f, Vector2.Zero, Vector2.One, out _);

                // Measure prefix width to offset the message
                float preWidth = FontAssets.MouseText.Value.MeasureString(prefix).X;

                // Draw the message text in white
                var msgSnips = ChatManager.ParseMessage(msg, Color.White).ToArray();
                Vector2 msgPos = pos + new Vector2(preWidth, 0f);
                ChatManager.DrawColorCodedStringWithShadow(
                    sb, FontAssets.MouseText.Value, msgSnips, msgPos, 0f, Vector2.Zero, Vector2.One, out _);

                // If this is the link line, underline the URL portion
                if (i == 2 && Conf.C.ShowLinks)
                {
                    DrawLinkExample(sb, msgPos);
                }
            }
        }

        // rewrites the player name to match the format in the config
        private string FormatPrefix(string rawName)
        {
            // Conf.C.PlayerFormat is either "<PlayerName>" or "PlayerName:"
            // so replace the placeholder with the actual name
            if (Conf.C.PlayerNameFormat == "PlayerName:")
            {
                // Find all instances of < and > and remove them
                rawName = rawName.Replace("<", "").Replace(">", "");
                return $"{rawName}:"; // "PlayerName"
            }
            return $"<{rawName}>"; // "<PlayerName>"
        }

        private void DrawLinkExample(SpriteBatch sb, Vector2 position)
        {
            string ex = " https://forums.terraria.org/";
            var font = FontAssets.MouseText.Value;
            float scale = 0.8f;

            // measure scaled text
            Vector2 textSize = font.MeasureString(ex) * scale;

            // 1) underline in dark blue
            var underlineRect = new Rectangle(
                (int)position.X,
                (int)(position.Y + textSize.Y - 6),
                (int)textSize.X, 2
            );
            sb.Draw(TextureAssets.MagicPixel.Value, underlineRect, new Color(10, 15, 154));

            // 2) draw text in blue
            ChatManager.DrawColorCodedStringWithShadow(
                sb,
                font,
                ex,
                position,
                new Color(17, 85, 204),
                0f,
                Vector2.Zero,
                new Vector2(scale)
            );

            // 4) build hit rectangle & handle click
            var hitbox = new Rectangle(
                (int)position.X,
                (int)position.Y,
                (int)textSize.X,
                (int)textSize.Y
            );
            if (hitbox.Contains(Main.MouseScreen.ToPoint()))
            {
                Main.LocalPlayer.mouseInterface = true;
                if (Main.mouseLeft && Main.mouseLeftRelease)
                    URL.OpenURL(ex);
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            TooltipFunction = () => GetDynamicTooltip();
        }

        private string GetDynamicTooltip()
        {
            //return "Preview Area";
            return "";
        }
    }
}
