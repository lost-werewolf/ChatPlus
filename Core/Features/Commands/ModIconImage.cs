using System;
using System.Drawing;
using ChatPlus.Core.Features.ModIcons;
using ChatPlus.Core.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI.Chat;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ChatPlus.Core.Features.Commands
{
    internal class ModIconImage : UIImage
    {
        private readonly Mod mod;

        public ModIconImage(Asset<Texture2D> texture, Mod mod = null) : base(texture)
        {
            this.mod = mod;
        }

        public override void Draw(SpriteBatch sb)
        {
            // Dimensions
            Vector2 target = GetDimensions().Position();

            if (mod == null)
            {
                var tex = Ass.TerrariaIcon.Value;
                Rectangle rect = new Rectangle((int)target.X - 3, (int)target.Y - 1, 26, 26);
                DrawTextureScaledToFit(sb, tex, rect);
            }
            else
            {
                string tag = ModIconTagHandler.GenerateTag(mod.Name);
                ChatManager.DrawColorCodedStringWithShadow(
                    sb,
                    FontAssets.MouseText.Value,
                    tag,
                    target + new Vector2(-2, -1),
                    Color.White,
                    0f,            // rotation
                    Vector2.Zero,  // origin
                    Vector2.One, // scale
                    -1f,
                    1f
                );
            }

            //DrawSmallModIcon(sb, mod, target, size: 26);

            if (IsMouseHovering && mod == null)
            {
                UICommon.TooltipMouseText("Terraria");
            }

            if (IsMouseHovering && mod != null && !string.IsNullOrEmpty(mod.Name))
            {
                UICommon.TooltipMouseText(mod.DisplayName);
            }
        }

        private static void DrawSmallModIcon(SpriteBatch sb, Mod mod, Vector2 pos, int size)
        {
            Texture2D tex = null;
            if (mod == null) 
                tex = Ass.TerrariaIcon.Value;
            else if (mod.Name == "ModLoader") 
                tex = Ass.tModLoaderIcon.Value;
            else
            {
                string smallPath = $"{mod.Name}/icon_small";
                string normalPath = $"{mod.Name}/icon";

                if (ModContent.HasAsset(smallPath))
                    tex = ModContent.Request<Texture2D>(smallPath, AssetRequestMode.ImmediateLoad).Value;
                else if (ModContent.HasAsset(normalPath))
                    tex = ModContent.Request<Texture2D>(normalPath, AssetRequestMode.ImmediateLoad).Value;
            }

            var target = new Rectangle((int)pos.X - 3, (int)pos.Y - 2, size, size);
            if (tex != null) {
                DrawTextureScaledToFit(sb, tex, target); 
                return; 
            }

            if (mod != null)
            {
                string initials = string.IsNullOrEmpty(mod.DisplayName) ? mod.Name : mod.DisplayName;
                initials = initials.Length >= 2 ? initials[..2] : initials;
                Vector2 p = target.Center.ToVector2() + new Vector2(0, 5);
                Utils.DrawBorderString(sb, initials, p, Color.White, 1f, 0.5f, 0.5f);
            }
        }

        private static void DrawTextureScaledToFit(SpriteBatch sb, Texture2D tex, Rectangle target)
        {
            if (tex == null)
                return;

            float scale = Math.Min(
                target.Width / (float)tex.Width,
                target.Height / (float)tex.Height
            );

            sb.Draw(tex,target.Center.ToVector2(),null,Color.White,0f,tex.Size() / 2f, scale,SpriteEffects.None, 0f);
        }
    }
}