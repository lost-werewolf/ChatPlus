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

        // Selection functionality
        private bool isSelected;
        public event Action<CommandPanelElement> OnHovered;

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

        public void SetSelected(bool selected)
        {
            isSelected = selected;
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            base.MouseOver(evt);
            OnHovered?.Invoke(this);
        }

        public override void MouseOut(UIMouseEvent evt)
        {
            base.MouseOut(evt);
        }


        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);

            string commandText = "/" + name;

            // Run through command processor
            var message = new ChatMessage(commandText);
            var caller = new ChatCommandCaller();

            if (CommandLoader.HandleCommand(message.Text, caller))
            {
                // Command executed
            }
            else
            {
                // Fallback: show it in chat
                //Main.NewText(commandText, Color.White);
            }
        }


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);

            var dims = GetDimensions();
            var rect = new Rectangle((int)dims.X, (int)dims.Y, (int)dims.Width, (int)dims.Height);

            // Background highlight for selected/hovered
            if (isSelected)
                sb.Draw(TextureAssets.MagicPixel.Value, rect, new Color(60, 120, 255, 100));
            //else if (IsMouseHovering)
                //sb.Draw(TextureAssets.MagicPixel.Value, rect, new Color(255, 255, 255, 30));

            Vector2 position = dims.Position();

            // Draw icon
            if (icon != null)
            {
                Rectangle iconPos = new((int)position.X + 4, (int)position.Y + 2, 26, 26);
                if (icon == Ass.VanillaIcon.Value)
                    iconPos = new((int)position.X + 4, (int)position.Y + 2, 20, 26);

                sb.Draw(icon, iconPos, Color.White);
            }

            // Draw text next to icon
            Utils.DrawBorderString(sb, name, position + new Vector2(36, 6), Color.White);

            // Draw tooltip of mod name if hovering icon
            if (Main.MouseScreen.Between(new Vector2((int)position.X, (int)position.Y + 2), new Vector2((int)position.X + 30, (int)position.Y + 28)))
            {
                //DrawHelper.DrawTextAtMouse(sb, modName);
                UICommon.TooltipMouseText(modName);
            }

            // Draw tooltip of usage if hovering icon
            if (Main.MouseScreen.Between(
                new Vector2((int)position.X + 30, (int)position.Y + 2), 
                new Vector2((int)position.X + 270, (int)position.Y + 28)))
            {
                //DrawHelper.DrawTextAtMouse(sb, modName);
                if (!string.IsNullOrEmpty(usage))
                {
                    UICommon.TooltipMouseText(usage);
                }
            }
        }
    }
}
