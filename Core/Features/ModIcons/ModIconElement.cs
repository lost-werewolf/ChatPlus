using ChatPlus.Core.UI;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader.UI;
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
        Vector2 pos = dims.Position() + new Microsoft.Xna.Framework.Vector2(3,0);

        // mod icon tag
        string tag = Data.Tag;
        ChatManager.DrawColorCodedStringWithShadow(sb,FontAssets.MouseText.Value,tag,pos,Color.White,0f,Vector2.Zero,Vector2.One,-1f,1f);

        // mod icon display name
        TextSnippet[] snip = [new TextSnippet(Data.mod.DisplayName)];
        ChatManager.DrawColorCodedStringWithShadow(sb, FontAssets.MouseText.Value, snip, pos += new Vector2(32, 3), 0f, Vector2.Zero, Vector2.One, out _);

        // tag hover tooltip of mod name
        Rectangle bounds = new((int)pos.X-34, (int)pos.Y, (int)26, (int)26);
        //sb.Draw(TextureAssets.MagicPixel.Value, bounds, Color.Red);
        if (bounds.Contains(Main.MouseScreen.ToPoint()))
        {
            UICommon.TooltipMouseText(Data.mod.DisplayName);
        }
    }
}
