using System;
using System.Collections.Generic;
using AdvancedChatFeatures.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

            if (!Conf.C.features.PlayerIcons)
            {
                chatXOffset = 40f;
                return;
            }

            // Update chat X offset
            chatXOffset = 40f + 20f;

            // Draw player head icon at top-left of this element
            CalculatedStyle dims = GetDimensions();
            int count = 2 + (Conf.C.features.Links ? 1 : 0);
            const int maxSlots = 3; // total possible icon lines

            for (int i = 0; i < count; i++)
            {
                // bottom‑align: skip the top (maxSlots - count) slots
                float y = 30 * (maxSlots - count + i);
                Vector2 pos = new(dims.X + 56 - 15f, dims.Y + 20f + y);
                DrawHelper.DrawPlayerHead(pos, 0.8f, sb: sb);
            }
        }
        private void DrawConfigIfOn(SpriteBatch sb)
        {
            if (!Conf.C.features.ConfigIcon)
                return;

            // Draw settings cog to left of preview box chat
            DrawHelper.DrawProperScale(
                sb,
                element: this,
                tex: Ass.ConfigButton.Value,
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

        private List<string> playerNames = [];

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
            float lineHeight = FontAssets.MouseText.Value.MeasureString("M").Y + 4f;

            bool linksEnabled = false;

            if (Item != null)
            {
                var type = Item.GetType();
                var featuresField = type.GetField("features");
                object featuresObj = featuresField?.GetValue(Item) ?? type.GetProperty("features")?.GetValue(Item);

                if (featuresObj != null)
                {
                    var linksField = featuresObj.GetType().GetField("Links");
                    if (linksField != null && linksField.GetValue(featuresObj) is bool lf)
                        linksEnabled = lf;

                    var linksProp = featuresObj.GetType().GetProperty("Links");
                    if (linksProp != null && linksProp.GetValue(featuresObj) is bool lp)
                        linksEnabled = lp;
                }
            }

            float topY = linksEnabled ? -10f : -10f;
            Vector2 panelPos = new(dims.X + paddingX, dims.Y + dims.Height - panelH + topY);

            // Get first 3 loaded player names (fallback to TestPlayer if null/empty)
            if (playerNames.Count < 3)
            {
                playerNames.Clear();
                Main.LoadPlayers();

                if (Main.PlayerList.Count >= 3)
                {
                    for (int i = 0; i < Math.Min(3, Main.PlayerList.Count); i++)
                    {
                        string name = Main.PlayerList[i]?.Name;
                        if (string.IsNullOrEmpty(name))
                            name = "PlayerName";
                        playerNames.Add(name);
                    }
                }
            }

            if (playerNames.Count == 0)
                playerNames.Add("PlayerName");

            // Build a list of (prefix, message) pairs up-front:
            var chatLines = new List<KeyValuePair<string, string>>
            {
                new(FormatPrefix(playerNames[0]), " How are you?"),
                new(FormatPrefix(playerNames[Math.Min(1, playerNames.Count-1)]), " Good, thank you!")
            };

            if (Conf.C.features.Links)
            {
                chatLines.Add(new KeyValuePair<string, string>(
                    FormatPrefix(playerNames[Math.Min(2, playerNames.Count - 1)]),
                    ""
                ));
            }

            // Calc total height of all lines
            float totalMsgHeight = chatLines.Count * lineHeight;
            float startY = panelPos.Y - totalMsgHeight - 4f;

            // Loop through them and draw prefix (always white) + message (colored/underlined if needed)
            for (int i = 0; i < chatLines.Count; i++)
            {
                var (prefix, msg) = chatLines[i];
                Vector2 pos = new(panelPos.X + 2f, startY + i * lineHeight);

                // Draw player color
                Color msgColor = Conf.C.features.PlayerColors
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

                // If this is the link line, underline the URLHelper portion
                if (i == 2 && Conf.C.features.Links)
                {
                    DrawLinkExample(sb, msgPos);
                }
            }
        }

        private string FormatPrefix(string rawName)
        {
            string format = null;

            if (Item != null)
            {
                var type = Item.GetType();

                // Step 1: Get the "features" object
                object featuresObj = null;

                var featuresField = type.GetField("features");
                if (featuresField != null)
                {
                    featuresObj = featuresField.GetValue(Item);
                }
                else
                {
                    var featuresProp = type.GetProperty("features");
                    if (featuresProp != null)
                        featuresObj = featuresProp.GetValue(Item);
                }

                // Step 2: Get "PlayerFormat" from featuresObj
                if (featuresObj != null)
                {
                    var fType = featuresObj.GetType();

                    var pfField = fType.GetField("PlayerFormat");
                    if (pfField != null && pfField.GetValue(featuresObj) is string fs)
                        format = fs;

                    if (format == null)
                    {
                        var pfProp = fType.GetProperty("PlayerFormat");
                        if (pfProp != null && pfProp.GetValue(featuresObj) is string ps)
                            format = ps;
                    }
                }
            }

            // Step 3: Fallback to saved config
            if (string.IsNullOrEmpty(format))
                format = Conf.C?.features?.PlayerFormat;

            // Step 4: Final fallback
            if (string.IsNullOrEmpty(format))
                format = "<PlayerName>";

            // Step 5: Apply format
            if (format == "PlayerName:")
            {
                rawName = rawName.Replace("<", "").Replace(">", "");
                return $"{rawName}:";
            }

            return $"<{rawName}>";
        }

        private void DrawLinkExample(SpriteBatch sb, Vector2 position)
        {
            string ex = "https://forums.terraria.org/";
            var font = FontAssets.MouseText.Value;
            float scale = 0.9f;

            // measure scaled text
            Vector2 textSize = font.MeasureString(ex) * scale;

            // modify pos
            position += new Vector2(10, 0);

            // 1) underline in dark blue
            var underlineRect = new Rectangle(
                (int)position.X,
                (int)(position.Y + textSize.Y - 7),
                (int)textSize.X, 2
            );
            sb.Draw(TextureAssets.MagicPixel.Value, underlineRect, new Color(10, 15, 154));

            // 2) draw text in blue
            ChatManager.DrawColorCodedStringWithShadow(
                sb,
                font,
                ex,
                position,
                ColorHelper.Blue,
                0f,
                Vector2.Zero,
                new Vector2(scale)
            );

            // 4) build hit rectangle & handle click
            // NOTE: DOESNT WORK! but not a priority to fix
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
                    URLHelper.OpenURL(ex);
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            //TooltipFunction = () => "";
        }
    }
}
