using System;
using ChatPlus.Core.Chat.MiniChatButtons.Shared;
using ChatPlus.Core.Features.Items;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace ChatPlus.Core.Chat.MiniChatButtons;

internal class ItemButton : BaseChatButton
{
    protected override ChatButtonType Type => ChatButtonType.Items;

    protected override UserInterface UI => ModContent.GetInstance<ItemSystem>().ui;
    protected override UIState State => ModContent.GetInstance<ItemSystem>().state;
    protected override Action<bool> SetOpenedByButton => flag => ItemState.WasOpenedByButton = flag;


    private static readonly Random rng = new();
    private string currentItemTag = "[i:1]"; // fallback to Copper Shortsword

    public ItemButton()
    {
        currentTex = TextureAssets.Item[1].Value;
        //PickRandomItem();
    }

    public override void MouseOut(UIMouseEvent evt)
    {
        base.MouseOut(evt);
        //PickRandomItem();
    }

    private Texture2D currentTex;

    private void PickRandomItem()
    {
        if (ItemManager.Items == null || ItemManager.Items.Count == 0)
        {
            currentItemTag = "[i:1]";
            currentTex = TextureAssets.Item[1].Value;
            return;
        }

        var pick = ItemManager.Items[rng.Next(ItemManager.Items.Count)];
        currentItemTag = $"[i:{pick.ID}]";
        currentTex = TextureAssets.Item[pick.ID].Value;
    }

    protected override void DrawCustom(SpriteBatch sb, Vector2 pos)
    {
        var dims = GetDimensions();
        bool forceNormal = IsMouseHovering || UI.CurrentState == State;
        ChatButtonRenderer.Draw(sb, Type, dims.Position(), 24, grayscale: !forceNormal);
    }

}
