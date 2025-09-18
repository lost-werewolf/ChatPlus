using System;
using ChatPlus.Core.UI;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI;
using Terraria.UI.Chat;
using static ChatPlus.Common.Configs.Config;

namespace ChatPlus.Core.Features.Uploads
{
    public class UploadElement : BaseElement<Upload>
    {
        public Upload Element;

        private UIText img;

        public UploadElement(Upload data) : base(data)
        {
            Element = data;
            Height.Set(60, 0);
            Width.Set(0, 1);

            img = new(Element.Tag);
            img._textScale = 0.3f;
            img.Left.Set(5, 0);
            img.Top.Set(8, 0);
            Append(img);
        }

        public override void Draw(SpriteBatch sb)
        {
            Height.Set(60, 0);
            // When drawing uploads inside this element, suppress the global hover overlay
            bool prev = UploadSnippet.SuppressHoverUI;
            UploadSnippet.SuppressHoverUI = true;
            base.Draw(sb);
            UploadSnippet.SuppressHoverUI = prev;

            if (GetViewmode() == Viewmode.ListView)
                DrawListElement(sb);
            else
                DrawGridElement(sb);
        }

        private void DrawGridElement(SpriteBatch sb)
        {
            var dims = GetDimensions();
            Vector2 pos = dims.Position();

            img._textScale = 0.3f;
            img.Left.Set(5, 0);
            img.Top.Set(8, 0);
        }

        private void DrawListElement(SpriteBatch sb)
        {
            var dims = GetDimensions();
            Vector2 pos = dims.Position();

            img._textScale = 0.3f;
            img.Left.Set(5, 0);
            img.Top.Set(8, 0);

            static string ToHashLabel(string tag)
            {
                if (string.IsNullOrEmpty(tag))
                    return "#";

                if (tag.StartsWith("[u:") && tag.EndsWith("]"))
                {
                    int colon = tag.IndexOf(':');
                    int close = tag.LastIndexOf(']');
                    if (colon >= 0 && close > colon + 1)
                        return "#" + tag.Substring(colon + 1, close - colon - 1);
                }

                if (tag.StartsWith("#"))
                    return tag;

                return "#" + tag.Trim('[', ']');
            }

            string display = UploadSystem.OpenedFromHash ? ToHashLabel(Data.Tag) : Data.Tag;

            TextSnippet[] snip = [new TextSnippet(display)];
            Vector2 textPos = pos + new Vector2(65, 5);

            ChatManager.DrawColorCodedStringWithShadow(sb, FontAssets.MouseText.Value, snip, textPos, 0f, Vector2.Zero, Vector2.One, out _);
        }
    }
}
