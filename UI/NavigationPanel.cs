using System;
using System.Collections.Generic;
using AdvancedChatFeatures.Common.Configs;
using AdvancedChatFeatures.Helpers;
using AdvancedChatFeatures.UI.Commands;
using AdvancedChatFeatures.UI.Emojis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace AdvancedChatFeatures.UI
{
    /// <summary>
    /// A panel that can be navigated on with arrow keys
    /// </summary>
    public abstract class NavigationPanel : DraggablePanel
    {
        // Elements
        private readonly UIScrollbar scrollbar;
        protected UIList list;
        protected readonly List<NavigationElement> items = [];

        // Navigation
        protected int currentIndex = 0; // first item
        protected int itemCount;

        // Holding keys
        private double repeatTimer;
        private Keys heldKey = Keys.None;

        public NavigationPanel()
        {
            // Set width
            Width.Set(220, 0);

            // Set position just above the chat
            VAlign = 1f;
            Top.Set(-38, 0);
            Left.Set(80, 0);

            // Style
            OverflowHidden = true;
            BackgroundColor = ColorHelper.DarkBlue * 1.0f;
            SetPadding(0);

            // Initialize elements
            list = new UIList
            {
                ListPadding = 0f,
                Width = { Pixels = -20f, Percent = 1f },
                Top = { Pixels = 3f },
                Left = { Pixels = 3f },
                ManualSortMethod = _ => { },
            };

            scrollbar = new UIScrollbar
            {
                HAlign = 1f,
                Height = { Pixels = -14f, Percent = 1f },
                Top = { Pixels = 7f },
                Width = { Pixels = 20f },
            };
            list.SetScrollbar(scrollbar);

            Append(list);
            Append(scrollbar);
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);

            // Deselect all
            foreach (NavigationElement e in items)
            {
                e.SetSelected(false);
            }

            var sys = ModContent.GetInstance<CommandSystem>();
            if (sys.ui.CurrentState != null)
            {
                sys.commandState.commandPanel.resetGhostText();
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
        }

        public virtual void SetSelectedIndex(int index)
        {
            if (items.Count == 0) return;

            // wrap around
            if (index < 0) index = items.Count - 1;
            else if (index >= items.Count) index = 0;

            // set all to false
            for (int i = 0; i < items.Count; i++)
                items[i].SetSelected(false);

            // update current index
            currentIndex = index;
            var current = items[currentIndex];
            current.SetSelected(true);

            // update view position
            float view = list.ViewPosition;
            int topIndex = (int)(view / 30);
            int bottomIndex = topIndex + itemCount - 1;

            if (currentIndex < topIndex)
                view = currentIndex * 30;
            else if (currentIndex > bottomIndex)
                view = (currentIndex - itemCount + 1) * 30;

            float max = Math.Max(0f, items.Count * 30 - itemCount * 30);
            if (view < 0) view = 0;
            if (view > max) view = max;
            list.ViewPosition = view;
        }

        protected bool JustPressed(Keys key) => Main.keyState.IsKeyDown(key) && Main.oldKeyState.IsKeyUp(key);

        public override void Update(GameTime gt)
        {
            base.Update(gt);

            // Tap key
            if (JustPressed(Keys.Up))
            {
                SetSelectedIndex(currentIndex - 1);
                heldKey = Keys.Up;
                repeatTimer = 0.35;
            }
            else if (JustPressed(Keys.Down))
            {
                SetSelectedIndex(currentIndex + 1);
                heldKey = Keys.Down;
                repeatTimer = 0.35;
            }

            // Hold key
            double dt = gt.ElapsedGameTime.TotalSeconds;
            if (Main.keyState.IsKeyDown(heldKey))
            {
                repeatTimer -= dt;
                if (repeatTimer <= 0)
                {
                    repeatTimer += 0.06; // repeat speed
                    if (Main.keyState.IsKeyDown(Keys.Up)) SetSelectedIndex(currentIndex - 1);
                    else if (Main.keyState.IsKeyDown(Keys.Down)) SetSelectedIndex(currentIndex + 1);
                }
            }
        }

        #region Sizing

        public void SetCommandPanelHeight()
        {
            if (this is not CommandPanel)
            {
                Log.Error("this is not commandpanel!");
                return;
            }

            // Update height
            itemCount = Conf.C == null ? 10 : Conf.C.autocompleteConfig.CommandsVisible;
            Height.Set(30 * itemCount, 0);
            list.Height.Set(30 * itemCount, 0);

            var sys = ModContent.GetInstance<CommandSystem>();
            if (sys?.commandState?.commandUsagePanel != null)
            {
                sys.commandState.commandUsagePanel.UpdateTopPosition();
            }
        }

        public void SetEmojiPanelHeight()
        {
            if (this is not EmojiPanel)
            {
                Log.Error("this is not emojipanel!");
                return;
            }

            // Update height
            itemCount = Conf.C == null ? 10 : Conf.C.emojisConfig.EmojisVisible;
            Height.Set(30 * itemCount, 0);
            list.Height.Set(30 * itemCount, 0);

            var sys = ModContent.GetInstance<EmojiSystem>();
            if (sys?.emojiState?.emojiUsagePanel != null)
            {
                sys.emojiState.emojiUsagePanel.UpdateTopPosition();
            }
        }

        #endregion
    }
}