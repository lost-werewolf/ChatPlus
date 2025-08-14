using AdvancedChatFeatures.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ModLoader.UI;

namespace AdvancedChatFeatures.UI.Commands
{
    internal class ModIcon(Asset<Texture2D> texture, Mod mod = null) : UIImage(texture: texture)
    {
        public override void Draw(SpriteBatch spriteBatch)
        {
            // Dimensions
            Rectangle target = GetDimensions().ToRectangle();

            if (mod is null)
            {
                // Draw default tree icon for vanilla commands or unknown mods
                Texture2D texVal = texture.Value;
                int widest = texVal.Width > texVal.Height ? texVal.Width : texVal.Height;
                spriteBatch.Draw(texVal, target.Center.ToVector2(), null, Color.White, 0, texture.Size() / 2f, target.Width / (float)widest, 0, 0);

                if (IsMouseHovering)
                {
                    UICommon.TooltipMouseText("Terraria");
                }
                return;
            }

            Texture2D tex = null;

            string path = $"{mod.Name}/icon_small";

            if (mod.Name == "ModLoader")
                tex = Ass.tModLoaderIcon.Value;
            else if (ModContent.HasAsset(path))
                tex = ModContent.Request<Texture2D>(path).Value;

            // Draw icon
            if (tex != null)
            {
                int widest = tex.Width > tex.Height ? tex.Width : tex.Height;
                spriteBatch.Draw(tex, target.Center.ToVector2(), null, Color.White, 0, tex.Size() / 2f, target.Width / (float)widest, 0, 0);
            }
            else
            {
                // Draw first two letters of the mod
                Utils.DrawBorderString(spriteBatch, mod.DisplayName[..2], target.Center.ToVector2(), Color.White, 1, 0.5f, 0.4f);
            }

            if (IsMouseHovering && !string.IsNullOrEmpty(mod.Name))
            {
                UICommon.TooltipMouseText(mod.Name);
            }
        }
    }
}