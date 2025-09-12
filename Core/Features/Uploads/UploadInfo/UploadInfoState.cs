using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace ChatPlus.Core.Features.Uploads.UploadInfo
{
    public class UploadInfoState : UIState, ILoadable
    {
        public static UploadInfoState Instance;

        private Upload? currentUpload;
        private UIPanel mainPanel;
        private UITextPanel<string> titlePanel;

        public void Load(Mod mod)
        {
            Instance = this;
        }

        public void Unload()
        {
            Instance = null;
        }

        public override void OnInitialize()
        {
            var root = new UIElement
            {
                Width = { Percent = 0.8f },
                MaxWidth = new StyleDimension(800f, 0f),
                Top = { Pixels = 120f },
                Height = { Pixels = -120f, Percent = 1f },
                HAlign = 0.5f
            };
            Append(root);

            // Main content panel
            mainPanel = new UIPanel
            {
                Width = { Percent = 1f },
                Height = { Pixels = -110f, Percent = 1f },
                BackgroundColor = UICommon.MainPanelBackground
            };
            root.Append(mainPanel);

            // Title bar
            titlePanel = new UITextPanel<string>("", 0.8f, true)
            {
                HAlign = 0.5f,
                Top = { Pixels = -35f },
                BackgroundColor = UICommon.DefaultUIBlue
            }.WithPadding(15f);
            if (currentUpload != null)
            {
                titlePanel.SetText("Upload: " + currentUpload.Value.FileName);
            }
            root.Append(titlePanel);

            // Bottom panel for back button
            var bottom = new UIElement
            {
                Width = { Percent = 1f },
                Height = { Pixels = 40f },
                VAlign = 1f,
                Top = { Pixels = -60f }
            };
            root.Append(bottom);

            var backButton = new UITextPanel<string>(Language.GetText("UI.Back").Value)
            {
                Width = { Percent = 1f },
                Height = { Pixels = 40f }
            }.WithFadedMouseOver();
            backButton.OnLeftClick += (evt, el) => Close();
            bottom.Append(backButton);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (currentUpload == null || currentUpload.Value.Texture == null)
                return;

            var tex = currentUpload.Value.Texture;

            // get drawable area inside main panel
            var dims = mainPanel.GetInnerDimensions();
            int pad = 12;

            Rectangle inner = new Rectangle(
                (int)dims.X + pad,
                (int)dims.Y + pad,
                (int)dims.Width - pad * 2,
                (int)dims.Height - pad * 2
            );

            // fit image to inner rect, preserving aspect ratio
            float scale = Math.Min(
                (float)inner.Width / tex.Width,
                (float)inner.Height / tex.Height
            );

            int drawW = (int)(tex.Width * scale);
            int drawH = (int)(tex.Height * scale);

            Vector2 pos = new Vector2(
                inner.X + (inner.Width - drawW) / 2f,
                inner.Y + (inner.Height - drawH) / 2f
            );

            spriteBatch.Draw(tex, new Rectangle((int)pos.X, (int)pos.Y, drawW, drawH), Color.White);
        }

        public void Show(Upload upload)
        {
            currentUpload = upload;
            titlePanel?.SetText($"Upload: {upload.FileName}");
            IngameFancyUI.OpenUIState(this);
        }

        public void Close()
        {
            currentUpload = null;
            IngameFancyUI.Close();
        }
    }
}
