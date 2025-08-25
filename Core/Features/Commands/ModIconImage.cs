using System;
using ChatPlus.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;

namespace ChatPlus.CommandHandler
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

            DrawSmallModIcon(sb, mod, target, size: 26);

            if (IsMouseHovering && mod != null && !string.IsNullOrEmpty(mod.Name))
            {
                UICommon.TooltipMouseText(mod.Name);
                //Main.hoverItemName = mod.Name;
                //DrawTextAtMouse(sb, mod.Name);
            }
        }

        private static void DrawSmallModIcon(SpriteBatch sb, Mod mod, Vector2 pos, int size = 16)
        {
            Texture2D tex = null;

            // Default fallback for unknown mods
            if (mod == null)
            {
                tex = Ass.TerrariaIcon.Value;
            }
            else if (mod.Name == "ModLoader")
            {
                tex = Ass.tModLoaderIcon.Value;
            }
            else
            {
                string path = $"{mod.Name}/icon_small";
                if (ModContent.HasAsset(path))
                    tex = ModContent.Request<Texture2D>(path).Value;
            }

            Rectangle target = new((int)pos.X - 3, (int)pos.Y - 2, size, size);

            if (tex != null)
            {
                DrawTextureScaledToFit(sb, tex, target);
            }
            else if (mod != null)
            {
                // fallback to initials
                string initials = mod.DisplayName.Length >= 2 ? mod.DisplayName[..2] : mod.DisplayName;
                Vector2 initialsPos = target.Center.ToVector2();
                initialsPos += new Vector2(0, 5);
                Utils.DrawBorderString(sb, initials, initialsPos, Color.White, scale: 1.0f, 0.5f, 0.5f);
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

            sb.Draw(
                tex,
                target.Center.ToVector2(),
                null,
                Color.White,
                0f,
                tex.Size() / 2f,
                scale,
                SpriteEffects.None,
                0f
            );
        }
    }
}