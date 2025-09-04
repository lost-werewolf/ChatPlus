using System.Collections.Generic;
using ChatPlus.Core.UI;
using Terraria;
using Terraria.UI;

namespace ChatPlus.Core.Features.Mentions;

public readonly record struct MentionEntry(int PlayerIndex, string PlayerName);

public class MentionPanel : BasePanel<MentionEntry>
{
    protected override IEnumerable<MentionEntry> GetSource()
    {
        for (int i = 0; i < Main.maxPlayers; i++)
        {
            var p = Main.player[i];
            if (p?.active == true && !string.IsNullOrWhiteSpace(p.name))
                yield return new MentionEntry(i, p.name);
        }
    }

    protected override BaseElement<MentionEntry> BuildElement(MentionEntry data) => new MentionElement(data);
    protected override string GetDescription(MentionEntry data) => data.PlayerName;
    protected override string GetTag(MentionEntry data) => "@" + data.PlayerName + " ";
}

public class MentionElement : BaseElement<MentionEntry>
{
    public MentionElement(MentionEntry data) : base(data)
    {
        Height.Set(24, 0);
        Width.Set(0, 1);
    }

    public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch sb)
    {
        base.Draw(sb);
        var dims = GetDimensions();
        var pos = dims.Position();
        Terraria.UI.Chat.ChatManager.DrawColorCodedStringWithShadow(sb, Terraria.GameContent.FontAssets.MouseText.Value,
            Data.PlayerName,
            pos + new Microsoft.Xna.Framework.Vector2(6, 2), Microsoft.Xna.Framework.Color.White, 0f, Microsoft.Xna.Framework.Vector2.Zero, Microsoft.Xna.Framework.Vector2.One);
    }
}