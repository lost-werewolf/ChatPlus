using System;
using AdvancedChatFeatures.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Chat;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
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
        // Properties
        public string name { get; private set; }
        private string usage;
        private Texture2D icon;
        private string modName;

        // Selected
        public bool isSelected;

        public CommandPanelElement(string name, string usage, Texture2D icon = null, string modName = null)
        {
            // Class variables
            this.name = name;
            this.icon = icon ?? Ass.VanillaIcon.Value;
            this.usage = usage;
            this.modName = modName;

            // Dimensions
            Height.Set(30, 0);
            Width.Set(0, 1);
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);

            Main.chatText = "/" + name;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);

            var dims = GetDimensions();
            Vector2 position = dims.Position();

            // Draw highlight of entire element
            if (isSelected)
                sb.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)dims.X, (int)dims.Y, (int)dims.Width, (int)dims.Height), new Color(60, 120, 255, 100));

            // Draw icon
            if (icon != null)
            {
                Rectangle iconPos = new((int)position.X + 4, (int)position.Y + 2, 26, 26);
                if (icon == Ass.VanillaIcon.Value)
                    iconPos = new((int)position.X + 4, (int)position.Y + 2, 20, 26);

                sb.Draw(icon, iconPos, Color.White);
            }

            // Draw text
            Utils.DrawBorderString(sb, name, position + new Vector2(36, 6), Color.White);

            // Draw tooltip of mod name if hovering icon
            if (Main.MouseScreen.Between(new Vector2((int)position.X, (int)position.Y + 2), new Vector2((int)position.X + 30, (int)position.Y + 28)))
            {
                //DrawHelper.DrawTextAtMouse(sb, modName);
                UICommon.TooltipMouseText(modName);
            }

            // Draw tooltip of usage
            if (Main.MouseScreen.Between(
                new Vector2((int)position.X + 30, (int)position.Y + 2), 
                new Vector2((int)position.X + 270, (int)position.Y + 28)))
            {
                if (name == "help")
                {
                    UICommon.TooltipMouseText("Lists all the commands you can use");
                }
                else if (!string.IsNullOrEmpty(usage))
                {
                    UICommon.TooltipMouseText(usage);
                }

                //DrawHelper.DrawTextAtMouse(sb, modName);
            }
        }
    }
}
