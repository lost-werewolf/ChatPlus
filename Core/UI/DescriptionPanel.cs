using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ChatPlus.Common.Configs;
using ChatPlus.Core.Features.ModIcons;
using ChatPlus.Core.Features.ModIcons.ModInfo;
using ChatPlus.Core.Features.PlayerHeads;
using ChatPlus.Core.Features.PlayerHeads.PlayerInfo;
using ChatPlus.Core.Features.Uploads;
using ChatPlus.Core.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;
using Terraria.UI;
using Terraria.Utilities.FileBrowser;
using static nativefiledialog;

namespace ChatPlus.Core.UI
{
    public class DescriptionPanel<TData> : DraggablePanel
    {
        private readonly UIText text;
        public UIText GetText() => text;

        protected override float SharedYOffset
        {
            get
            {
                int pad = 2;
                int itemCount = 10;
                if (Conf.C != null) itemCount = (int)Conf.C.AutocompleteItemCount;
                return -itemCount * 30 - pad; // sit above the base panel
            }
        }

        public DescriptionPanel(string initialText = null)
        {
            // Size
            Width.Set(320, 0);
            Height.Set(60, 0);

            // Style
            BackgroundColor = ColorHelper.DarkBlue * 1.0f;

            // Position
            VAlign = 1f;
            Left.Set(190, 0);

            // Text
            text = new(initialText ?? string.Empty, 0.9f, false)
            {
                HAlign = 0.0f
            };

            Append(text);
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            // Uploads
            if (typeof(TData) == typeof(Upload) && ConnectedPanel is UploadPanel up)
            {
                up.UploadImage();
                return;
            }

            // Mods: open the “view more” page
            if (typeof(TData) == typeof(ModIcon) && ConnectedPanel is ModIconPanel mp)
            {
                mp.OpenModInfoForSelectedMod();
                return;
            }

            // Players: open the “view more” page
            if (typeof(TData) == typeof(PlayerHead) && ConnectedPanel is PlayerHeadPanel ph)
            {
                ph.OpenPlayerInfoForSelected();
                return;
            }
        }
        
        public override void RightClick(UIMouseEvent evt)
        {
            // Uploads
            if (typeof(TData) == typeof(Upload) && ConnectedPanel is UploadPanel up)
            {
                up.OpenUploadsFolder();
                return;
            }
        }

        public void SetText(string rawText)
        {
            var font = FontAssets.MouseText.Value;
            float maxWidth = Width.Pixels;

            // Special case: uploads
            if (typeof(TData) == typeof(Upload))
            {
                string t ="[c/FFF014:Uploads]: Click to upload images \nRight click to open folder";
                text.SetText(t);
                text.VAlign = 0f;
                Height.Set(62, 0);
                Recalculate();
                return;
            }

            // If caller already gave us newlines (e.g., "Name\nClick to view more"), just use them.
            if (!string.IsNullOrEmpty(rawText) && rawText.IndexOf('\n') >= 0)
            {
                text.SetText(rawText);

                // Decide height from the number of lines
                int lines = 1;
                for (int i = 0; i < rawText.Length; i++)
                    if (rawText[i] == '\n') lines++;

                text.VAlign = lines >= 2 ? 0f : 0.5f;
                int h = lines switch { 1 => 38, 2 => 62, _ => 90 };
                Height.Set(h, 0);
                Recalculate();
                return;
            }

            // Single-line input without explicit newline: fit or split into two lines.
            if (font.MeasureString(rawText).X <= maxWidth || rawText.Contains('['))
            {
                text.SetText(rawText);
                text.VAlign = 0.5f;
                Height.Set(38, 0);
                Recalculate();
                return;
            }

            // Split at the last space that still fits on the first line (closest to the right edge).
            int bestSplit = -1;
            for (int i = 0; i < rawText.Length; i++)
            {
                if (rawText[i] == ' ')
                {
                    float w = font.MeasureString(rawText[..i]).X;
                    if (w <= maxWidth) bestSplit = i; else break;
                }
            }
            if (bestSplit == -1) bestSplit = rawText.Length / 2; // naive fallback

            string first = rawText[..bestSplit].TrimEnd();
            string second = rawText[(bestSplit + 1)..].TrimStart();

            // Always force two lines
            text.SetText(first + "\n" + second);
            text.VAlign = 0f;
            Height.Set(62, 0);
            Recalculate();
        }

        public override void Draw(SpriteBatch sb)
        {
            if (typeof(TData) == typeof(Upload))
            {
                Height.Set(62, 0);
                text.VAlign = 0f;
            }

            base.Draw(sb);
        }
    }
}
