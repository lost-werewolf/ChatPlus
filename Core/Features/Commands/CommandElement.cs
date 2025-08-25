using ChatPlus.Core.Chat;
using ChatPlus.Core.Helpers;
using ChatPlus.Core.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.UI;
using Terraria.UI.Chat;

namespace ChatPlus.Core.Features.Commands
{
    /// <summary>
    /// Represents a chat command element in the UI.
    /// </summary>
    public class CommandElement : BaseElement<Command>
    {
        public Command Command;
        private ModIconImage modIconImage;

        public CommandElement(Command command) : base(command)
        {
            Command = command;

            // Add mod icon
            modIconImage = new(Ass.TerrariaIcon, command.Mod)
            {
                Left = { Pixels = 6 },
                VAlign = 0.5f,
                Width = { Pixels = 22 },
                Height = { Pixels = 22 }
            };
            Append(modIconImage);
        }
        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);

            Main.chatText = Command.Name; // Replace chat text with the command name

            // Set caret pos to last pos
            int chatLength = Main.chatText.Length;
            HandleChatSystem.SetCaretPos(chatLength);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);

            var dims = GetDimensions();
            var pos = dims.Position();

            // Draw command name
            TextSnippet[] snip = [new TextSnippet(Command.Name)];
            ChatManager.DrawColorCodedStringWithShadow(sb, FontAssets.MouseText.Value, snip, pos += new Vector2(32, 4), 0f, Vector2.Zero, Vector2.One, out _);
        }
    }
}
