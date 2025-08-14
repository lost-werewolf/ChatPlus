using AdvancedChatFeatures.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.ModLoader.Config.UI;
using Terraria.UI;
using Terraria.UI.Chat;

namespace AdvancedChatFeatures.Common.Configs
{
    /// <summary>
    /// Reference:
    /// <see cref="StringOptionElement"/> 
    /// And Starlight River CustomConfigElement
    /// https://github.com/ProjectStarlight/StarlightRiver/blob/master/Content/GUI/Config/AbilityUIReposition.cs#L10
    /// </summary>
    public class PlayerFormat : StringOptionElement
    {
        // Called once when the config UI binds this element to your Width property
        public override void OnBind()
        {
            base.OnBind();
            TextDisplayFunction = () => "Player Format";
            // options = ["<PlayerName>", "PlayerName:"];

            //TooltipFunction = () => Language.GetTextValue(
                //"Mods.AdvancedChatFeatures.Configs.Config.Features.PlayerFormat.Tooltip");
        }

        public override void OnInitialize()
        {
            base.OnInitialize();
            TextDisplayFunction = () => "Player Format";
            Height.Set(80, 0);
            Recalculate();
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
            DrawPlayerFormat(sb);
        }

        private void DrawPlayerFormat(SpriteBatch sb)
        {
            if (Main.LocalPlayer == null)
            {
                Log.Error("DrawPlayerFormat: no local player");
                return;
            }

            // Name fallback
            string name = string.IsNullOrEmpty(Main.LocalPlayer.name) ? "PlayerName" : Main.LocalPlayer.name;

            // Respect formatting from config
            string format = getValue != null
    ? getValue() // bound getter from FormatStringOptionElement
    : (string)MemberInfo.GetValue(Item) ?? "<PlayerName>";
            string preview = format == "PlayerName:" ? $"{name}:" : $"<{name}>";

            // Respect color toggle from config
            //Color drawColor = Conf.C?.features?.PlayerColors ?? false ? ColorHelper.PlayerColors[0] : Color.White;
            Color drawColor = Color.White;

            // Layout (match PlayerColors)
            CalculatedStyle dims = GetDimensions();
            Vector2 scale = new(0.8f);
            Vector2 size = ChatManager.GetStringSize(FontAssets.ItemStack.Value, preview, scale);

            Vector2 pos = new(
                dims.X + 8 + 150f, // fixed X offset
                dims.Y + (dims.Height - size.Y) * 0.5f + 2
            );

            ChatManager.DrawColorCodedStringWithShadow(
                sb,
                FontAssets.MouseText.Value,
                preview,
                pos,
                drawColor,
                0f,
                Vector2.Zero,
                scale
            );
        }

        // Called every frame while the in-game config UI is open
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            //TextDisplayFunction = () => "Player Format";
        }
    }
}
