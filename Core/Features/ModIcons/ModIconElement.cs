using ChatPlus.Common.Configs;
using ChatPlus.Core.UI;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.UI.Chat;

namespace ChatPlus.Core.Features.ModIcons;

public class ModIconElement : BaseElement<ModIcon>
{
    public ModIconElement(ModIcon modIcon) : base(modIcon)
    {
        Height.Set(30, 0);
        Width.Set(0, 1);
    }

    public override void Draw(SpriteBatch sb)
    {
        base.Draw(sb);
        var dims = GetDimensions();
        Vector2 pos = dims.Position();

        // mod icon tag
        string tag = Data.Tag;
        float scale = 1.0f; // 150% bigger
        ChatManager.DrawColorCodedStringWithShadow(
            sb,
            FontAssets.MouseText.Value,
            tag,
            pos + new Vector2(13, 5),
            Color.White,
            0f,            // rotation
            Vector2.Zero,  // origin
            new Vector2(scale), // scale
            -1f,
            1f
        );

        // mod icon display name
        TextSnippet[] snip = [new TextSnippet(Data.Tag)];
        ChatManager.DrawColorCodedStringWithShadow(sb, FontAssets.MouseText.Value, snip, pos += new Vector2(40, 4), 0f, Vector2.Zero, Vector2.One, out _);

        // tag hover tooltip of mod name
        Rectangle bounds = new((int)pos.X - 30, (int)pos.Y, (int)30, (int)30);

        if (bounds.Contains(Main.MouseScreen.ToPoint()))
        {
            if (!Conf.C.ShowStatsWhenHovering) 
                return;

            if (Conf.C.DisableStatsWhenBossIsAlive && Main.CurrentFrameFlags.AnyActiveBossNPC)
                return;

            HoveredModOverlay.Set(Data.mod);
        }

        // debug
        //sb.Draw(TextureAssets.MagicPixel.Value, bounds, Color.Red*0.5f);
    }
}
