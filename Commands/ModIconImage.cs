using AdvancedChatFeatures.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;

namespace AdvancedChatFeatures.Commands
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

            DrawHelper.DrawSmallModIcon(sb, mod, target, size: 26);

            if (IsMouseHovering && !string.IsNullOrEmpty(mod.Name))
            {
                UICommon.TooltipMouseText(mod.Name);
            }
        }
    }
}