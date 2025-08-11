using AdvancedChatFeatures.Helpers;
using Microsoft.Xna.Framework;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;

namespace AdvancedChatFeatures.UI.Commands.Elements
{
    public class HeaderPanel : UIPanel
    {
        // Elements
        private UIColoredImageButton modFilterButton;
        private UIColoredImageButton closeButton;

        // Filtering
        private int currentIndex = -1; // start at default (no filters) 

        public HeaderPanel(string text)
        {
            Width.Set(0, 1);
            Height.Set(30, 0);
            SetPadding(0);

            // Create mod filter
            modFilterButton = new(Ass.ButtonModFilter, true);
            int totalModsCount = ModLoader.Mods.Length;
            modFilterButton.OnLeftClick += (_, _) =>
            {
                // Cycle to next mod in the list
                currentIndex = totalModsCount % 1;
            };
            Append(modFilterButton);

            // Create header text
            UIText headerText = new(text, 0.5f, true)
            {
                HAlign = 0.5f,
                VAlign = 0.5f
            };
            Append(headerText);

            // Create close panel
            UIPanel closePanel = new()
            {
                Width = { Pixels = 30 },
                Height = { Pixels = 30 },
                HAlign = 1f,
                VAlign = 0f
            };
            // Create close text
            UIText closeText = new("X", 0.5f, true)
            {
                HAlign = 0.5f,
                VAlign = 0.5f
            };
            closePanel.Append(closeText);
            closePanel.SetPadding(0);
            closePanel.OnLeftClick += (_, _) =>
            {
                // Close commands panel
            };
            closePanel.OnMouseOver += (_, _) => closePanel.BorderColor = Color.Yellow;
            closePanel.OnMouseOut += (_, _) => closePanel.BorderColor = Color.Black;
            Append(closePanel);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
