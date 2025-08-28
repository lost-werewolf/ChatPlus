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
    /// Reference:
    /// <see cref="BooleanElement"/> 
    /// </summary>
    public class AutocompleteConfigElement : ConfigElement<bool>
    {
        private Asset<Texture2D> _toggleTexture = Asset<Texture2D>.Empty;

        // Called once when the config UI binds this element to your Width property
        public override void OnBind()
        {
            base.OnBind();

            _toggleTexture = Main.Assets.Request<Texture2D>("Images/UI/Settings_Toggle");

            // OpenSystem the value when clicked
            OnLeftClick += delegate
            {
                Value = !Value;

                Conf.C.Autocomplete = Value;
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

            DrawTagExample(sb);
        }
        private void DrawTagExample(SpriteBatch sb)
        {
            CalculatedStyle dims = GetDimensions();
            Rectangle r = new((int)dims.X + 225, (int)dims.Y, 225, 30);
            Vector2 pos = new(r.X + 8, r.Y + 4);

            Utils.DrawInvBG(sb, r, ColorHelper.UIPanelBlue);

            string text = "/, [c, [e, [g, [i, [m, [p, [u";
            TextSnippet[] snip = [new TextSnippet(text)];
            ChatManager.DrawColorCodedStringWithShadow(sb, FontAssets.MouseText.Value, snip, pos += new Vector2(0, 0), 0f, Vector2.Zero, Vector2.One, out _);
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

            Vector2 pos = new Vector2(dimensions.X + dimensions.Width - 60f, dimensions.Y + 8f);
            ChatManager.DrawColorCodedStringWithShadow(
                sb,FontAssets.ItemStack.Value,label, pos,Color.White,0f,Vector2.Zero,new Vector2(0.8f));
        }

        // Called every frame while the in-game config UI is open
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}

