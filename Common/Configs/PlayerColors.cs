using LinksInChat.Helpers;
using Microsoft.CodeAnalysis.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader.Config.UI;
using Terraria.UI;
using Terraria.UI.Chat;

namespace LinksInChat.Common.Configs
{
    /// <summary>
    /// Reference:
    /// <see cref="Terraria.ModLoader.Config.UI.BooleanElement"/> 
    /// </summary>
    public class PlayerColors : ConfigElement<bool>
    {
        private Asset<Texture2D> _toggleTexture = Asset<Texture2D>.Empty;

        // Called once when the config UI binds this element to your Width property
        public override void OnBind()
        {
            base.OnBind();

            _toggleTexture = Main.Assets.Request<Texture2D>("Images/UI/Settings_Toggle");

            // Toggle the value when clicked
            OnLeftClick += delegate
            {
                Value = !Value;

                Conf.C.PlayerColors = Value;
            };
        }

        public override void OnInitialize()
        {
            base.OnInitialize();
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
            DrawPlayerColors(sb);
            DrawToggleTexture(sb);
            DrawOnOffText(sb);
        }

        private void DrawPlayerColors(SpriteBatch sb)
        {
            if (Main.LocalPlayer == null)
            {
                Log.Error("oop player color no local player");
                return;
            }

            string PlayerNameText = $"{Main.LocalPlayer.name}";
            CalculatedStyle dims = this.GetDimensions();
            Vector2 pos = dims.Position();
            Vector2 textSize = FontAssets.MouseText.Value.MeasureString(PlayerNameText);
            int xOffset = 150;

            // Draw text
            Vector2 textPos = new(pos.X + 8 + xOffset, pos.Y + textSize.Y / 2 - 6);
            ChatManager.DrawColorCodedStringWithShadow(
                sb,
                FontAssets.ItemStack.Value,
                PlayerNameText,
                textPos,
                ColorHelper.PlayerColors[0],
                0f,
                Vector2.Zero,
                baseScale: new Vector2(0.8f)
            );
        }

        private void DrawToggleTexture(SpriteBatch sb)
        {
            // Draw toggle texture
            Rectangle sourceRectangle = new Rectangle(
                Value ? ((_toggleTexture.Width() - 2) / 2 + 2) : 0,
                0,
                (_toggleTexture.Width() - 2) / 2,
                _toggleTexture.Height()
            );

            // Get the dimensions of the parent element
            CalculatedStyle dimensions = GetDimensions();

            // Calculate the position to draw the toggle texture
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

            // If you want the tooltip to reflect changes in real-time, 
            // update the TooltipFunction (or an internal field used by GetTooltip/TooltipFunction) here:
            TooltipFunction = () => GetDynamicTooltip();
        }

        private string GetDynamicTooltip()
        {
            return "";
        }
    }
}

