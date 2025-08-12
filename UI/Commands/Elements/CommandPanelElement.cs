using AdvancedChatFeatures.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader.UI;
using Terraria.UI;

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

        // Selection
        private bool isSelected;
        public bool SetSelected(bool val) => isSelected = val;

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

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);

            // Clicking the command makes it appear in the chat
            Main.chatText = "/" + name;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);

            // Draw background color if selected
            var dims = GetDimensions();
            if (isSelected)
                DrawHelper.DrawCurrentlyHighlightedCommandElement(sb, dims);

            // Draw icon
            Vector2 position = dims.Position();
            if (icon != null)
            {
                Rectangle iconPos = new((int)position.X + 4, (int)position.Y + 2, 26, 26);
                if (icon == Ass.VanillaIcon.Value)
                    iconPos = new((int)position.X + 4, (int)position.Y + 2, 20, 26);

                sb.Draw(icon, iconPos, Color.White);
            }

            // Draw text
            Utils.DrawBorderString(sb, name, position + new Vector2(36, 6), Color.White);

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
