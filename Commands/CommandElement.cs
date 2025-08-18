using AdvancedChatFeatures.Common.Configs;
using AdvancedChatFeatures.Common.Hooks;
using AdvancedChatFeatures.Helpers;
using AdvancedChatFeatures.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using Terraria.UI.Chat;

namespace AdvancedChatFeatures.Commands
{
    /// <summary>
    /// Represents a chat command element in the UI.
    /// </summary>
    public class CommandElement : BaseElement<Command>
    {
        public Command Command;
        private ModIconImage modIconImage;
        private UIText cmdText;

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

            // Add command name
            //if (command.Name != null)
            //{
            //    cmdText = new(command.Name, textScale: 1, large: false)
            //    {
            //        Left = { Pixels = 32 },
            //        VAlign = 0.5f,
            //    };
            //    Append(cmdText);
            //}
        }
        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);

            Main.chatText = Command.Name; // Replace chat text with the command name

            // Set caret pos to last pos
            int chatLength = Main.chatText.Length;
            HandleChatHook.SetCaretPos(chatLength);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);

            if (cmdText != null) RemoveChild(cmdText);

            var dims = GetDimensions();
            var pos = dims.Position();

            // Draw file name
            var imgName = new[] { new TextSnippet(Command.Name) };
            pos += new Vector2(32, 5);
            ChatManager.DrawColorCodedStringWithShadow(sb, FontAssets.MouseText.Value, imgName, pos, 0f, Vector2.Zero, Vector2.One, out _);
        }
    }
}
