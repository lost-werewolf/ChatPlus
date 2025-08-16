using System.Collections.Generic;
using AdvancedChatFeatures.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.ModLoader.Config.UI;
using Terraria.UI;
using Terraria.UI.Chat;

namespace AdvancedChatFeatures.Common.Configs
{
    /// <summary>
    /// Reference:
    /// <see cref="ConfigElement"/> 
    /// </summary>
    public class PreviewElement : ConfigElement
    {
        // Blinker state
        private int textBlinkerCount;
        private bool textBlinkerVisible;

        // Called once when the config UI binds this element
        public override void OnBind()
        {
            base.OnBind();
            // Fill available width, centered in parent
            Width = StyleDimension.Fill;
            HAlign = 0.5f;
            VAlign = 0.5f;
            Top.Set(0, 0);

            MinHeight = new StyleDimension(30 + 30 * 6+5, 0);
            Label = string.Empty;
        }

        public override void Draw(SpriteBatch sb)
        {
            MinHeight = new StyleDimension(30 + 30 * 6+5, 0);

            Top.Set(0, 0);

            base.Draw(sb);

            DrawChatBox(sb);

            DrawExampleMessages(sb);

            if (Conf.C.styleConfig.ShowConfigButton)
                DrawConfigButton(sb);
        }

        private object ReadLiveConfigValue(string fieldName)
        {
            if (Item == null) return null;

            var type = Item.GetType();
            var styleConfigField = type.GetField("styleConfig");
            if (styleConfigField == null) return null;

            var styleObj = styleConfigField.GetValue(Item);
            if (styleObj == null) return null;

            var fType = styleObj.GetType();
            var field = fType.GetField(fieldName);
            if (field == null) return null;

            return field.GetValue(styleObj);
        }

        private void DrawExampleMessages(SpriteBatch sb)
        {
            bool showIcons = (bool?)ReadLiveConfigValue("ShowPlayerIcons") ?? false;
            bool showColors = (bool?)ReadLiveConfigValue("ShowPlayerColors") ?? false;
            bool showModIcon = (bool?)ReadLiveConfigValue("ShowModIcons") ?? false;
            bool enableLinks = (bool?)ReadLiveConfigValue("ShowLinks") ?? false;
            string format = (string)ReadLiveConfigValue("ShowPlayerFormat") ?? "<PlayerName>";

            var rows = new List<(string name, string message, bool isModIcon, bool isLink)>
            {
                ("Player", "Message1", false, false),
                ("Player", "Message2", false, false),
                ("Player", "Message3", false, false),
            };

            if (enableLinks) rows.Add(("Player", "https://forums.terraria.org/", false, true));
            if (showModIcon) rows.Add((null, "Example Mod Message", true, false));

            var dims = GetDimensions();
            float lineH = FontAssets.MouseText.Value.MeasureString("M").Y + 4f;
            int chatBoxH = TextureAssets.TextBack.Height();
            float topPad = 14f, gapToBox = 8f, bottomPad = 12f;
            float needed = topPad + rows.Count * lineH + gapToBox + chatBoxH + bottomPad;

            // left anchor; give a little extra room if showing player icons
            float left = dims.X + 40f + (showIcons ? 4f : 4f);
            float y = dims.Y + topPad;

            for (int i = 0; i < rows.Count; i++)
            {
                var row = rows[i];
                Vector2 rowPos = new(left, y);

                // Draw mod icon
                if (row.isModIcon)
                {
                    Mod mod = null;
                    ModLoader.TryGetMod("ModLoader", out mod);
                    DrawHelper.DrawSmallModIcon(sb, mod, new Vector2(left - 28f, rowPos.Y - 0f), 24);
                    var msgSnips = ChatManager.ParseMessage(row.message, Color.Yellow).ToArray();
                    ChatManager.DrawColorCodedStringWithShadow(sb, FontAssets.MouseText.Value, msgSnips, rowPos, 0f, Vector2.Zero, Vector2.One, out _);
                    y += lineH;
                    continue;
                }

                if (showIcons)
                    DrawHelper.DrawPlayerHead(pos: new Vector2(left - 22f, rowPos.Y + 8f), scale: 0.8f, sb: sb, color: Color.White *0.5f);

                // Draw player name
                string prefix = PlayerFormatHelper.GetFormattedName(row.name, format) + " ";
                Color nameColor = showColors ? ColorHelper.PlayerColors[i % ColorHelper.PlayerColors.Length] : Color.White;
                var preSnips = ChatManager.ParseMessage(prefix, nameColor).ToArray();
                ChatManager.DrawColorCodedStringWithShadow(sb, FontAssets.MouseText.Value, preSnips, rowPos, 0f, Vector2.Zero, Vector2.One, out _);
                float preW = FontAssets.MouseText.Value.MeasureString(prefix).X;

                // Draw link
                Vector2 msgPos = new(rowPos.X + preW, rowPos.Y);
                if (row.isLink)
                {
                    string url = row.message;
                    var linkSnips = ChatManager.ParseMessage(url, ColorHelper.Blue).ToArray();
                    ChatManager.DrawColorCodedStringWithShadow(sb, FontAssets.MouseText.Value, linkSnips, msgPos, 0f, Vector2.Zero, Vector2.One, out _);

                    // underline
                    Vector2 sz = FontAssets.MouseText.Value.MeasureString(url);
                    var underline = new Rectangle((int)msgPos.X, (int)(msgPos.Y + sz.Y - 9), (int)sz.X, 2);
                    sb.Draw(TextureAssets.MagicPixel.Value, underline, ColorHelper.BlueUnderline);
                }
                else
                {
                    // Draw message
                    var msgSnips = ChatManager.ParseMessage(row.message, Color.White).ToArray();
                    ChatManager.DrawColorCodedStringWithShadow(sb, FontAssets.MouseText.Value, msgSnips, msgPos, 0f, Vector2.Zero, Vector2.One, out _);
                }

                y += lineH;
            }
        }

        private void DrawConfigButton(SpriteBatch sb)
        {
            Vector2 pos = new(GetDimensions().X + 7, GetDimensions().Y + MinHeight.Pixels - 41);
            sb.Draw(Ass.ConfigButton.Value,pos,null,Color.White,0f, Vector2.Zero,0.8f,SpriteEffects.None,0f);
        }

        private void DrawChatBox(SpriteBatch sb)
        {
            if (!Main.drawingPlayerChat)
            {
                Main.drawingPlayerChat = true;
                PlayerInput.WritingText = true;
                Main.instance.HandleIME();
            }

            var dims = GetDimensions();

            Vector2 panelPos = new(
                dims.X + 40,
                dims.Y + dims.Height - TextureAssets.TextBack.Height() - 10
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

            var textSnips = ChatManager.ParseMessage(previewText, Color.White).ToArray();

            // Draw the panel
            sb.Draw(
                TextureAssets.TextBack.Value,panelPos,null,new Color(100, 100, 100, 100),0f,Vector2.Zero,1f,SpriteEffects.None, 0f);

            // Draw input text
            ChatManager.DrawColorCodedStringWithShadow(
                sb,FontAssets.MouseText.Value,textSnips,panelPos + new Vector2(6f, 6f),0f,Vector2.Zero,Vector2.One,out _);
        }
    }
}
