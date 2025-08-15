using AdvancedChatFeatures.Common.Configs;
using AdvancedChatFeatures.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace AdvancedChatFeatures.UI.Commands
{
    /// <summary>
    /// Represents a chat command element in the UI.
    /// </summary>
    public class CommandElement : NavigationElement
    {
        public Command Command;

        public CommandElement(Command command)
        {
            Command = command;

            // Add mod icon
            ModIconImage modIconImage = new(Ass.TerrariaIcon, command.Mod)
            {
                Left = { Pixels = 6 },
                VAlign = 0.5f,
                Width = { Pixels = 22 },
                Height = { Pixels = 22 }
            };
            Append(modIconImage);

            // Add command name
            if (command.Name != null)
            {
                UIText cmdText = new(command.Name, textScale: 1, large: false)
                {
                    Left = { Pixels = 32 },
                    VAlign = 0.5f,
                };
                Append(cmdText);
            }
        }
        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);
            Main.chatText = Command.Name; // Enter the chat command into chat
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch sb)
        {
            if (Conf.C.autocompleteConfig.ShowHoverTooltips && IsMouseHovering && !string.IsNullOrEmpty(Command.Usage))
            {
                UICommon.TooltipMouseText(Command.Usage);
            }

            base.Draw(sb);
        }
    }
}
