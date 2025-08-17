using System;
using System.IO;
using AdvancedChatFeatures.Common.Configs;
using AdvancedChatFeatures.Helpers;
using AdvancedChatFeatures.Uploads;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace AdvancedChatFeatures.UI
{
    public class DescriptionPanel<TData> : DraggablePanel
    {
        private readonly UIText text;
        public UIText GetText() => text;

        public DescriptionPanel(string initialText = null, bool centerText=false)
        {
            // Size
            Width.Set(320, 0);
            Height.Set(60, 0);

            // Style
            BackgroundColor = ColorHelper.DarkBlue * 0.8f;

            // Position
            VAlign = 1f;
            Left.Set(80, 0);

            // Text
            text = new(initialText ?? string.Empty, 0.9f, false)
            {
                HAlign = 0.5f
            };

            if (centerText)
            {
                text.HAlign = 0.5f;
                text.VAlign = 0.5f;
            }

            Append(text);
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            if (typeof(TData) == typeof(Upload))
            {
                string fullFilePath = FileUploadHelper.OpenFileDialog();
                if (fullFilePath == null) return;

                string fileName = Path.GetFileName(fullFilePath);
                string tag = UploadTagHandler.GenerateTag(fileName);
                Texture2D texture = FileUploadHelper.CreateTextureFromPath(fullFilePath);

                UploadInitializer.Uploads.Add(new Upload(tag, fileName, fullFilePath, texture));
                //UploadInitializer.AddNewUpload(new Upload(tag, fileName, fullFilePath, texture));

                Log.Info(UploadInitializer.Uploads.Count.ToString());

                string folder = Path.Combine(Main.SavePath, "AdvancedChatFeatures", "Uploads");
                Directory.CreateDirectory(folder);
                File.Copy(fullFilePath, Path.Combine(folder, fileName), true);

                if (this is UploadPanel up)
                    up.PopulatePanel();
            }

            base.LeftClick(evt);
        }

        public override void RightClick(UIMouseEvent evt)
        {
            Conf.C.Open();

            // Expand autocomplete section in config after UI is built
            Main.QueueMainThreadAction(() =>
            {
                var state = Main.InGameUI.CurrentState;
                if (state is not UIElement root) return;

                Conf.C.ExpandSection(root, nameof(Conf.C.autocompleteWindowConfig));
            });
        }

        public void SetTextWithLinebreak(string rawText)
        {
            float scale = 0.9f;
            string tooltip = rawText;

            // Measure the text size
            if (FontAssets.MouseText == null) return;
            Vector2 textSize = FontAssets.MouseText.Value.MeasureString(tooltip);
            float scaledWidth = textSize.X * scale / Main.UIScale;

            // If the text is too wide, insert a line break at the last space before it overflows
            if (scaledWidth > Width.Pixels)
            {
                // Estimate the max number of characters that fit
                int maxChars = (int)(Width.Pixels / scale / textSize.X * tooltip.Length) - 2;
                int breakIndex = tooltip.LastIndexOf(' ', Math.Min(tooltip.Length - 1, maxChars));
                if (breakIndex > 0)
                {
                    tooltip = tooltip[..breakIndex] + "\n" + tooltip[(breakIndex + 1)..];
                }
            }

            text.SetText(tooltip);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            int pad = 2;

            Top.Set(-Conf.C.autocompleteWindowConfig.ItemsPerWindow * 30 - 38 - pad, 0);

            base.Draw(spriteBatch);
        }
    }
}
