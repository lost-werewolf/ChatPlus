using LinksInChat.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader.Config.UI;
using Terraria.UI;
using Terraria.UI.Chat;

namespace LinksInChat.Common.Configs
{
    /// <summary>
    /// Reference:
    /// <see cref="Terraria.ModLoader.Config.UI.StringOptionElement"/> 
    /// And Starlight River CustomConfigElement
    /// https://github.com/ProjectStarlight/StarlightRiver/blob/master/Content/GUI/Config/AbilityUIReposition.cs#L10
    /// </summary>
    public class PlayerFormat : StringOptionElement
    {
        // Called once when the config UI binds this element to your Width property
        public override void OnBind()
        {
            base.OnBind();
            // options = ["<PlayerName>", "PlayerName:"];
        }

        public override void OnInitialize()
        {
            base.OnInitialize();
            Height.Set(80, 0);
            Recalculate();
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
            // DrawPlayerFormat(sb);
        }

        private void DrawPlayerFormat(SpriteBatch sb)
        {
            if (Main.LocalPlayer == null)
            {
                Log.Error("oop player format no local player");
                return;
            }

            string PlayerNameText = $"{Main.LocalPlayer.name}";
            CalculatedStyle dims = this.GetDimensions();
            Vector2 pos = dims.Position();
            Vector2 textSize = FontAssets.MouseText.Value.MeasureString(PlayerNameText);
            int xOffset = 150;

            // Set the format!
            if (Conf.C.PlayerNameFormat == "PlayerName:")
            {
                PlayerNameText = PlayerNameText.Replace("<", "").Replace(">", "");
            }

            // Draw text
            Vector2 textPos = new(pos.X + 8 + xOffset, pos.Y + textSize.Y / 2 - 6);
            ChatManager.DrawColorCodedStringWithShadow(
                sb,
                FontAssets.ItemStack.Value,
                PlayerNameText,
                textPos,
                Color.Orange,
                0f,
                Vector2.Zero,
                baseScale: new Vector2(0.8f)
            );
        }


        // Called every frame while the in-game config UI is open
        // public override void Update(GameTime gameTime)
        // {
        //     base.Update(gameTime);

        //     // If you want the tooltip to reflect changes in real-time, 
        //     // update the TooltipFunction (or an internal field used by GetTooltip/TooltipFunction) here:
        //     TooltipFunction = () => GetDynamicTooltip();
        // }

        // private string GetDynamicTooltip()
        // {
        //     // For example, check some global property:
        //     DebugSystem debugSystem = ModContent.GetInstance<DebugSystem>();
        //     int count = debugSystem.state.debugPanel.currentWeapons.Count;

        //     if (count > 3)
        //     {
        //         return "triple trouble";
        //     }
        //     else
        //     {
        //         return "single trouble";
        //     }
        // }
    }
}
