using System;
using System.IO;
using AdvancedChatFeatures.Common.Configs;
using AdvancedChatFeatures.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;
using Terraria.UI;

namespace AdvancedChatFeatures.UI.Commands.Elements
{
    public class CommandsPanel : DraggablePanel
    {
        // Elements
        private UIScrollbar scrollbar;
        private UIList commandList;

        public CommandsPanel()
        {
            Width.Set(300, 0);
            Height.Set(240, 0);
            MaxHeight.Set(240, 0);
            HAlign = 0.05f;
            VAlign = 0.95f;

            commandList = new()
            {
                ListPadding = 4f,
                Height = { Percent = 1f },
                ManualSortMethod = _ => { },
                Width = { Pixels = 1f }
            };
            scrollbar = new()
            {
                HAlign = 1f,
                Height = { Percent = 1f },
                Width = { Pixels = 20f }
            };
            commandList.SetScrollbar(scrollbar);

            // Add Commands
            foreach (var cmdList in CommandLoader.Commands.Values)
            {
                foreach (ModCommand cmd in cmdList)
                {
                    string name = cmd.Command;
                    string usage = cmd.Description;
                    Texture2D icon = null;
                    if (cmd.Mod.Name != "Terraria")
                    {
                        TmodFile tmodFile = cmd.Mod.File;
                        if (tmodFile != null)
                        {
                            icon = CommandsHelper.GetModIcon(tmodFile);
                        }
                    }

                    commandList.Add(new CommandPanelElement(name, usage, icon));
                }
            }

            Append(commandList);
            Append(scrollbar);
            commandList.Recalculate();
        }



        public override void Update(GameTime gameTime)
        {
            commandList.Width.Set(0, 1);
            BackgroundColor = ColorHelper.DarkBlue * 1.0f;
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);
        }
    }
}