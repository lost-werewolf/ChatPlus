using System.Diagnostics;
using ChatPlus.Core.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader.Config.UI;
using Terraria.UI;
using Terraria.UI.Chat;

namespace ChatPlus.Common.Configs.ConfigElements
{
    /// <summary>
    /// Reference <see cref="BooleanElement"/> 
    /// </summary>
    public class EnableLinksConfigElement : ConfigElement<bool>
    {
        private Asset<Texture2D> _toggleTexture;

        // Called once when the config UI binds this element
        public override void OnBind()
        {
            base.OnBind();

            _toggleTexture = Main.Assets.Request<Texture2D>("Images/UI/Settings_Toggle");

            OnLeftClick += delegate
            {
                Value = !Value;

                Conf.C.ShowLinks = Value;
            };
        }

        public override void OnInitialize()
        {
            base.OnInitialize();
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
            DrawToggleTexture(sb);
            DrawOnOffText(sb);

            DrawExampleLink(sb);
        }

        private Color linkColor;

        private void DrawExampleLink(SpriteBatch sb)
        {
            CalculatedStyle dims = GetDimensions();
            Vector2 pos = dims.Position();
            int xOffset = 225;
            Rectangle rect = new((int)dims.X + xOffset, (int)dims.Y, 225, 30);

            string exampleLink = "https://tmodloader.net/";

            // Draw chat box
            //DrawHelper.DrawInvBG(sb,rect, new Color(28, 57, 120)*0.9f);
            Utils.DrawInvBG(sb, rect);

            // 1. Draw underline
            Vector2 textSize = FontAssets.MouseText.Value.MeasureString(exampleLink);
            float x = pos.X + 8 + xOffset;
            float y = pos.Y + textSize.Y - 8;
            float scale = 0.72f;
            float w = scale * textSize.X + 20;
            Rectangle underlineRect = new Rectangle((int)x, (int)y, (int)w, 2);
            sb.Draw(TextureAssets.MagicPixel.Value, underlineRect, ColorHelper.BlueUnderline);

            // 2. Draw text in blue
            Vector2 textPos = new(pos.X + 8 + xOffset, pos.Y + textSize.Y / 2 - 9);
            ChatManager.DrawColorCodedStringWithShadow(
                sb,
                FontAssets.MouseText.Value,
                exampleLink,
                textPos,
                linkColor,
                0f,
                Vector2.Zero,
                baseScale: new Vector2(0.8f)
            );

            //// 3. If clicked, open the link
            //if (Main.MouseScreen.Between(textPos, textPos + new Vector2(w, textSize.Y)))
            //{
            //    linkColor = ColorHelper.BlueHover;
            //    if (Main.mouseLeft && Main.mouseLeftRelease)
            //    {
            //        // Open the link in the default browser
            //        Process.Start(new ProcessStartInfo
            //        {
            //            FileName = exampleLink,
            //            UseShellExecute = true
            //        });
            //    }
            //}
            //else
            {

                linkColor = new Microsoft.Xna.Framework.Color(32, 160, 255);
            }
        }

        private void DrawToggleTexture(SpriteBatch sb)
        {
            // Draw toggle uploadedTexture
            Rectangle sourceRectangle = new Rectangle(
                Value ? (_toggleTexture.Width() - 2) / 2 + 2 : 0,
                0,
                (_toggleTexture.Width() - 2) / 2,
                _toggleTexture.Height()
            );

            // Get the dimensions of the parent element
            CalculatedStyle dimensions = GetDimensions();

            // Calculate the position to draw the toggle uploadedTexture
            sb.Draw(
                _toggleTexture.Value,
                new Vector2(
                    dimensions.X + dimensions.Width - sourceRectangle.Width - 10f,
                    dimensions.Y + 8f
                ),
                sourceRectangle,
                Color.White
            );
        }

        private void DrawOnOffText(SpriteBatch sb)
        {
            CalculatedStyle dimensions = GetDimensions();

            string label = Value ? "On" : "Off";

            // Shift to a position you know is visible
            Vector2 pos = new Vector2(dimensions.X + dimensions.Width - 60f, dimensions.Y + 8f);
            ChatManager.DrawColorCodedStringWithShadow(
                sb,
                FontAssets.ItemStack.Value,
                label,
                pos,
                Color.White,
                0f,
                Vector2.Zero,
                new Vector2(0.8f)
            );
        }

        // Called every frame while the in-game config UI is open
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}

