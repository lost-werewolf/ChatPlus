using AdvancedChatFeatures.Common.Configs;
using AdvancedChatFeatures.Helpers;
using AdvancedChatFeatures.UI.Commands.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace AdvancedChatFeatures.UI.Commands
{
    /// <summary>
    /// An element that consists of a name, tooltip, mod icon.
    /// When hovered and clicked or enter is pressed, sends the command in the chat
    /// </summary>
    public class CommandElement : UIElement
    {
        // UI
        private ModIconImage modIconImage;

        // Variables
        public Command Command;
        private bool isSelected;
        public bool SetSelected(bool value) => isSelected = value;

        public CommandElement(Command command)
        {
            Command = command;

            Height.Set(30, 0);
            Width.Set(0, 1);

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

        /// <summary>
        /// // Clicking the command makes it appear in the chat
        /// </summary>
        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);

            // Set index
            var sys = ModContent.GetInstance<CommandSystem>();
            CommandPanel commandPanel = sys.commandState.commandPanel;
            int thisElementsIndex = commandPanel.items.FindIndex(e => e.Command.Name == Command.Name);
            if (thisElementsIndex >= 0)
            {
                commandPanel.SetSelectedIndex(thisElementsIndex);
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch sb)
        {
            if (isSelected)
            {
                DrawSlices(sb, this);
                DrawFill(sb, this);
            }
            if (Conf.C.autocompleteConfig.ShowHoverTooltips && IsMouseHovering && !string.IsNullOrEmpty(Command.Usage))
            {
                UICommon.TooltipMouseText(Command.Usage);
            }

            base.Draw(sb);
        }

        private static void DrawFill(SpriteBatch sb, UIElement ele)
        {
            CalculatedStyle dims = ele.GetDimensions();
            Texture2D pixel = TextureAssets.MagicPixel.Value;
            Rectangle r = new((int)dims.X+4, (int)dims.Y+4, (int)dims.Width-8, (int)dims.Height-6);

            // fill (slightly brighter blue, semi-transparent)
            sb.Draw(pixel, r, new Color(70, 120, 220, 140));

            // white border
            const int b = 0;
            sb.Draw(pixel, new Rectangle(r.X, r.Y, r.Width, b), Color.White);                 // top
            sb.Draw(pixel, new Rectangle(r.X, r.Bottom - b, r.Width, b), Color.White);        // bottom
            sb.Draw(pixel, new Rectangle(r.X, r.Y, b, r.Height), Color.White);                // left
            sb.Draw(pixel, new Rectangle(r.Right - b, r.Y, b, r.Height), Color.White);        // right
        }

        private static void DrawSlices(SpriteBatch sb, UIElement ele, bool fill = true, float fillOpacity = 0.3f)
        {
            Rectangle t = ele.GetDimensions().ToRectangle();

            var tex = Ass.Hitbox.Value;
            int c = 5;                        
            Rectangle sc = new(0, 0, c, c),
                      eh = new(c, 0, 30 - 2 * c, c),
                      ev = new(0, c, c, 30 - 2 * c),
                      ce = new(c, c, 30 - 2 * c, 30 - 2 * c);
            
            Color color = Color.White;

            if (fill)
                sb.Draw(tex, new Rectangle(t.X + c, t.Y + c, t.Width - 2 * c, t.Height - 2 * c), ce, color * 0.3f);

            sb.Draw(tex, new Rectangle(t.X + c, t.Y, t.Width - 2 * c, c), eh, color);                                       // top
            sb.Draw(tex, new Rectangle(t.X + c, t.Bottom - c, t.Width - 2 * c, c), eh, color, 0, Vector2.Zero, SpriteEffects.FlipVertically, 0); // bottom
            sb.Draw(tex, new Rectangle(t.X, t.Y + c, c, t.Height - 2 * c), ev, color);                                       // left
            sb.Draw(tex, new Rectangle(t.Right - c, t.Y + c, c, t.Height - 2 * c), ev, color, 0, Vector2.Zero, SpriteEffects.FlipHorizontally, 0); // right

            sb.Draw(tex, new Rectangle(t.X, t.Y, c, c), sc, color);                                                          // TL
            sb.Draw(tex, new Rectangle(t.Right - c, t.Y, c, c), sc, color, 0, Vector2.Zero, SpriteEffects.FlipHorizontally, 0); // TR
            sb.Draw(tex, new Rectangle(t.Right - c, t.Bottom - c, c, c), sc, color, 0, Vector2.Zero, SpriteEffects.FlipVertically | SpriteEffects.FlipHorizontally, 0); // BR
            sb.Draw(tex, new Rectangle(t.X, t.Bottom - c, c, c), sc, color, 0, Vector2.Zero, SpriteEffects.FlipVertically, 0); // BL
        }

    }
}
