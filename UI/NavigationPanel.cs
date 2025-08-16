using System;
using System.Collections.Generic;
using AdvancedChatFeatures.Common.Configs;
using AdvancedChatFeatures.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace AdvancedChatFeatures.UI
{
    /// <summary>
    /// A panel that can be navigated on with arrow keys
    /// </summary>
    public abstract class NavigationPanel<TData> : DraggablePanel
    {
        // Elements
        public readonly UIScrollbar scrollbar;
        protected UIList list;
        protected readonly List<NavigationElement> items = [];

        // Force populate
        protected abstract IEnumerable<TData> GetSource(); // The source of data to populate the panel with
        protected abstract NavigationElement BuildElement(TData data); // The method to create a new element from the data

        // Navigation
        protected int currentIndex = 0; // first item

        // Holding keys
        private double repeatTimer;
        private Keys heldKey = Keys.None;

        public NavigationPanel()
        {
            // Set width
            Width.Set(320, 0);

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

            // Populate the panel with items
            PopulatePanel();
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);

            // Deselect all
            foreach (NavigationElement e in items)
            {
                e.SetSelected(false);
            }
        }

        protected void PopulatePanel()
        {
            items.Clear();
            list.Clear();

            // Add all elements
            var source = GetSource();
            List<NavigationElement> fastList = []; 
            if (source != null)
            {
                foreach (TData data in source)
                {
                    var element = BuildElement(data);
                    if (element == null) continue;

                    items.Add(element);
                    fastList.Add(element);
                }
                // AddRange is much faster than adding one by one
                list.AddRange(fastList);
            }

            // Reset index to top item
            if (items.Count > 0)
                SetSelectedIndex(0);
        }

        public void SetHeight()
        {
            // Set height
            Height.Set(Conf.C.featureStyleConfig.ItemsPerWindow * 30, 0);
            list.Height.Set(Conf.C.featureStyleConfig.ItemsPerWindow * 30, 0);
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
            int itemsVisibleCount = Conf.C.featureStyleConfig.ItemsPerWindow;

            float view = list.ViewPosition;
            int topIndex = (int)(view / 30);
            int bottomIndex = topIndex + itemsVisibleCount - 1;

            if (currentIndex < topIndex)
                view = currentIndex * 30;
            else if (currentIndex > bottomIndex)
                view = (currentIndex - itemsVisibleCount + 1) * 30;

            float max = Math.Max(0f, items.Count * 30 - itemsVisibleCount * 30);
            if (view < 0) view = 0;
            if (view > max) view = max;
            list.ViewPosition = view;
        }

        protected bool JustPressed(Keys key) => Main.keyState.IsKeyDown(key) && Main.oldKeyState.IsKeyUp(key);

        public override void Update(GameTime gt)
        {
            base.Update(gt);

            Main.NewText(items.Count);

            Top.Set(-38, 0);

            SetHeight();

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

            // Hold key to repeat navigation
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
    }
}