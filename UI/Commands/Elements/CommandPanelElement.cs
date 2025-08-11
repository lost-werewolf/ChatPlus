using AdvancedChatFeatures.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.UI;
using Terraria.UI.Chat;

namespace AdvancedChatFeatures.UI.Commands.Elements
{
    /// <summary>
    /// An element that consists of a name, tooltip, mod icon.
    /// When hovered and clicked or enter is pressed, sends the command in the chat
    /// </summary>
    internal class CommandPanelElement : UIElement
    {
        private string name;
        private string usage;
        private Texture2D icon;

        public CommandPanelElement(string name, string usage, Texture2D icon = null)
        {
            // Class variables
            this.name = name;
            this.icon = icon ?? Ass.VanillaIcon.Value;
            this.usage = usage;

            // Dimensions
            Height.Set(30, 0);
            Width.Set(0, 1);
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            base.MouseOver(evt);
        }

        public override void MouseOut(UIMouseEvent evt)
        {
            base.MouseOut(evt);
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);
            Main.NewText("/" + name);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            Vector2 position = GetDimensions().Position();

            // Draw icon if not null
            if (icon != null)
            {
                spriteBatch.Draw(icon, new Rectangle((int)position.X + 4, (int)position.Y + 2, 26, 26), Color.White);
            }

            // Draw tooltip of mod name if hovering icon
            if (Main.MouseScreen.Between(new Vector2((int)position.X + 4, (int)position.Y + 2), new Vector2((int)position.X + 30, (int)position.Y + 28)))
            {
                Main.hoverItemName = name;
            }

            // Draw text next to icon
            Utils.DrawBorderString(spriteBatch, name, position + new Vector2(36, 6), Color.White);
        }
    }
}
