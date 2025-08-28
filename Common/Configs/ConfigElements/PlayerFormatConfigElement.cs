using System;
using ChatPlus.Common.Configs;
using ChatPlus.Core.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
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
    public class PlayerFormatConfigElement : StringOptionElement
    {
        // Called once when the config UI binds this element to your Width property
        public override void OnBind()
        {
            TextDisplayFunction = () => "Player Format";
            base.OnBind();
        }

        public override void OnInitialize()
        {
            TextDisplayFunction = () => "Player Format";
            base.OnInitialize();
            Height.Set(80, 0);
            Recalculate();
        }

        public override void Draw(SpriteBatch sb)
        {
            TextDisplayFunction = () => "Player Format";

            base.Draw(sb);

            if (Main.LocalPlayer == null) return;

            // read current option from the element (this is what the UI toggles)
            string current = getValue != null ? getValue() : (string)MemberInfo.GetValue(Item) ?? "<PlayerName>";

            // expose it to runtime (adjust the property name if yours differs)
            Conf.C.PlayerFormat = current;

            // preview
            string name = string.IsNullOrEmpty(Main.LocalPlayer.name) ? "PlayerName" : Main.LocalPlayer.name;
            string preview = string.Equals(current, "PlayerName:", StringComparison.Ordinal) ? name + ":" : "<" + name + ">";
            var dims = GetDimensions();
            var scale = new Vector2(0.8f);
            var size = ChatManager.GetStringSize(FontAssets.MouseText.Value, preview, scale);
            var pos = new Vector2(dims.X + 8 + 150f, dims.Y + (dims.Height - size.Y) * 0.5f + 2f);

            ChatManager.DrawColorCodedStringWithShadow(sb, FontAssets.MouseText.Value, preview, pos, Color.White, 0f, Vector2.Zero, scale);
        }

        // Called every frame while the in-game config UI is open
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
