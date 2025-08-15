using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Terraria;

namespace AdvancedChatFeatures.UI.Glyphs
{
    public class GlyphPanel : NavigationPanel
    {
        private string lastChatText = string.Empty;

        public GlyphPanel()
        {
            SetGlyphPanelHeight();
            PopulateGlyphPanel();
        }

        public void PopulateGlyphPanel()
        {
            items.Clear();
            list.Clear();

            foreach (var g in GlyphInitializerSystem.Glyphs)
            {
                var el = new GlyphElement(g);
                items.Add(el);
                list.Add(el);
            }

            SetSelectedIndex(0);
        }

        // Holding keys
        private double repeatTimer;
        private Keys heldKey = Keys.None;

        public override void Update(GameTime gt)
        {
            base.Update(gt);

            if (JustPressed(Keys.Tab) || Main.keyState.IsKeyDown(Keys.Tab)) 

            // Tap key
            if (JustPressed(Keys.Tab))
            {
                HandleTabKeyPressed();
                repeatTimer = 0.55;
                heldKey = Keys.Tab;
            }

            // Hold key
            double dt = gt.ElapsedGameTime.TotalSeconds;
            if (Main.keyState.IsKeyDown(heldKey))
            {
                repeatTimer -= dt;
                if (repeatTimer <= 0)
                {
                    repeatTimer += 0.06; // repeat speed
                    if (Main.keyState.IsKeyDown(Keys.Tab)) HandleTabKeyPressed();
                }
            }
        }

        private void HandleTabKeyPressed()
        {
            if (items.Count > 0 && currentIndex >= 0 && currentIndex <= items.Count)
            {
                var current = (GlyphElement)items[currentIndex];

                if (Main.chatText.Length <= 3)
                    Main.chatText = current.Glyph.Tag; // "[g:0]"
                else
                    Main.chatText += current.Glyph.Tag; // "[g:0]"
            }
        }
    }
}
