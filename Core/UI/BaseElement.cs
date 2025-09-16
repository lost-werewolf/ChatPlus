using ChatPlus.Core.Features.Uploads;
using ChatPlus.Core.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.GameContent;
using Terraria.UI;

namespace ChatPlus.Core.UI
{
    /// <summary>
    /// An element that can be navigated in a <see cref="BasePanel<TData>"/>.
    /// </summary>
    public abstract class BaseElement<TData> : UIElement
    {
        // Variables
        private bool isSelected;
        public bool SetSelected(bool value) => isSelected = value;

        // Properties
        public TData Data { get; }

        protected BaseElement(TData data)
        {
            Height.Set(30, 0);
            Width.Set(0, 1);
            Data = data;
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);

            // If left shift is clicked, do not insert the tag
            // Left shift is used for deleting images 
            // so we dont want to insert the tag when we delete an image
            bool leftShiftDown = Main.keyState.IsKeyDown(Keys.LeftShift);
            if (leftShiftDown) return;
               
           // Walk up until we find the panel, usually 3 steps: from InnerList -> List -> EmojiPanel
                UIElement parent = Parent;
            while (parent != null && parent is not BasePanel<TData>)
                parent = parent.Parent;

            if (parent is BasePanel<TData> panel)
            {
                int index = panel.items.IndexOf(this);
                if (index >= 0)
                {
                    panel.SetSelectedIndex(index);
                    panel.InsertSelectedTag();
                }
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            if (isSelected)
            {
                // DrawSystems selection rectangle
                DrawHelper.DrawSlices(sb, ele: this);
                DrawHelper.DrawFill(sb, ele: this);
            }

            base.Draw(sb);
        }
        
    }
}
