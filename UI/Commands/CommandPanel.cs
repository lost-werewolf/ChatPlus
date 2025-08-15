using System;
using AdvancedChatFeatures.Common.Configs;
using AdvancedChatFeatures.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace AdvancedChatFeatures.UI.Commands
{
    /// <summary> Represents a command panel in the UI. </summary>
    public class CommandPanel : NavigationPanel
    {
        // Variables
        private string ghostText = "";
        public string getGhostText() => ghostText;
        public string resetGhostText() => ghostText = "";
        private string lastChatText = string.Empty;

        public CommandPanel()
        {
            SetCommandPanelHeight();
            PopulateCommandPanel();
        }

        /// <summary> Populates the command panel with available commands. </summary>
        public void PopulateCommandPanel()
        {
            // Clear all items
            items.Clear();
            list.Clear();

            // Add all items
            foreach (Command cmd in CommandInitializerSystem.Commands)
            {
                CommandElement element = new(cmd);
                items.Add(element);
                list.Add(element);
            }

            // Reset everything
            SetSelectedIndex(0);

            var sys = ModContent.GetInstance<CommandSystem>();
            //sys.commandState.commandUsagePanel.UpdateText("List of commands\nPress tab to complete");
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);

            if (Conf.C.autocompleteConfig.ShowGhostText)
                DrawGhostText(sb);
        }

        private void DrawGhostText(SpriteBatch sb)
        {
            if (string.IsNullOrEmpty(ghostText)) return;

            // Remove blinking
            Main.instance.textBlinkerState = 0;
            Main.instance.textBlinkerCount = 0;

            Vector2 chatOriginUi = new(80f, Main.screenHeight - 32f);
            Vector2 typedSizeUi = FontAssets.MouseText.Value.MeasureString(Main.chatText ?? "");
            Vector2 ghostSizeUi = FontAssets.MouseText.Value.MeasureString(ghostText);
            Vector2 posUi = chatOriginUi + new Vector2(typedSizeUi.X + 12, 3f);
            Vector2 bgSizeUi = new(ghostSizeUi.X + 4f, ghostSizeUi.Y + 4f - 10);

            posUi += new Vector2(-3, -1);

            // Draw background
            DrawHelper.DrawRectangle(sb, bgSizeUi, posUi, ColorHelper.Blue * 0.5f);

            // Draw ghost text
            Utils.DrawBorderString(sb, ghostText, posUi, Color.White);
        }

        public override void SetSelectedIndex(int index)
        {
            base.SetSelectedIndex(index);

            if (items.Count == 0) return;
            var current = (CommandElement)items[currentIndex];

            // Ghost ON: update remainder
            if (Conf.C.autocompleteConfig.ShowGhostText)
            {
                string baseText = Main.chatText ?? string.Empty;
                string selected = current.Command.Name ?? string.Empty;

                if (!string.IsNullOrEmpty(baseText) &&
                    selected.Contains(baseText, StringComparison.OrdinalIgnoreCase))
                {
                    ghostText = selected.Substring(baseText.Length);
                }
                else
                {
                    string typedNoSlash = baseText.StartsWith("/") ? baseText.Substring(1) : baseText;
                    string selectedNoSlash = selected.TrimStart('/');
                    ghostText = selectedNoSlash.StartsWith(typedNoSlash, StringComparison.OrdinalIgnoreCase)
                        ? selectedNoSlash.Substring(typedNoSlash.Length)
                        : string.Empty;
                }
            }
            else
            {
                // Ghost OFF: only write to chat when navigating with Up/Down (not during filtering)
                if (Main.keyState.IsKeyDown(Keys.Up) || Main.keyState.IsKeyDown(Keys.Down))
                {
                    string chat = Main.chatText ?? string.Empty;
                    if (chat.StartsWith("/") && chat.IndexOf(' ') < 0)
                        Main.chatText = current.Command.Name ?? string.Empty;
                }
            }

            var sys = ModContent.GetInstance<CommandSystem>();
            sys.commandState.commandUsagePanel.UpdateText(current.Command.Usage);
        }



        public override void Update(GameTime gt)
        {
            base.Update(gt);

            if (JustPressed(Keys.Tab))
                HandleTabKeyPressed();

            // Re-filter whenever the chat text actually changes (covers holding Backspace).
            // But skip while arrow keys are held, since arrow nav may set Main.chatText and we don't want to filter then.
            bool navigating =
                Main.keyState.IsKeyDown(Keys.Up) ||
                Main.keyState.IsKeyDown(Keys.Down);

            string text = Main.chatText ?? string.Empty;
            if (!string.Equals(text, lastChatText, StringComparison.Ordinal))
            {
                lastChatText = text;
                if (!navigating)
                    ApplyFilter();
            }
        }


        private void ApplyFilter()
        {
            string text = Main.chatText ?? string.Empty;
            string typed = text.StartsWith("/") ? text[1..] : text;

            items.Clear();
            list.Clear();

            foreach (var cmd in CommandInitializerSystem.Commands)
            {
                string nameNoSlash = cmd.Name.TrimStart('/');
                if (typed.Length == 0 || nameNoSlash.StartsWith(typed, StringComparison.OrdinalIgnoreCase))
                {
                    var element = new CommandElement(cmd);
                    items.Add(element);
                    list.Add(element);
                }
            }

            if (items.Count > 0)
                SetSelectedIndex(0);
            else
                currentIndex = 0;

            // Ghost remainder
            if (Conf.C.autocompleteConfig.ShowGhostText)
            {
                if (items.Count > 0 && text.Length > 0)
                {
                    string selected = ((CommandElement)items[currentIndex]).Command.Name ?? string.Empty;

                    if (selected.StartsWith(text, StringComparison.OrdinalIgnoreCase))
                    {
                        ghostText = selected[text.Length..];
                    }
                    else
                    {
                        string typedNoSlash = typed;
                        string selectedNoSlash = selected.TrimStart('/');
                        ghostText = selectedNoSlash.StartsWith(typedNoSlash, StringComparison.OrdinalIgnoreCase)
                            ? selectedNoSlash[typedNoSlash.Length..]
                            : string.Empty;
                    }
                }
                else
                {
                    ghostText = string.Empty;
                }
            }
            else
            {
                ghostText = string.Empty;
            }
        }

        private void HandleTabKeyPressed()
        {
            if (Conf.C.autocompleteConfig.ShowGhostText)
            {
                Main.chatText += ghostText;
                ghostText = string.Empty;
            }
            else
            {
                if (items.Count > 0 && currentIndex >= 0 && currentIndex <= items.Count)
                {
                    CommandElement current = (CommandElement)items[currentIndex];
                    Main.chatText = current.Command.Name;
                }
            }
        }
    }
}