using System;
using ChatPlus.Core.Chat.MiniChatButtons.Shared;
using ChatPlus.Core.Features.Uploads;
using ChatPlus.Core.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;

namespace ChatPlus.Core.Chat.MiniChatButtons;

internal class UploadButton : BaseChatButton
{
    // Overrides
    protected override ChatButtonType Type => ChatButtonType.Uploads;
    protected override UserInterface UI => ModContent.GetInstance<UploadSystem>().ui;
    protected override UIState State => ModContent.GetInstance<UploadSystem>().state;
    protected override Action<bool> SetOpenedByButton => flag => UploadState.WasOpenedByButton = flag;

    // Texture

    private static readonly Random rng = new();

    private Texture2D currentTex;

    // lazy grayscale effect
    private static readonly Lazy<Asset<Effect>> grayscaleEffect =
        new(() => ModContent.Request<Effect>("ChatPlus/Assets/Effects/Grayscale", AssetRequestMode.ImmediateLoad));

    public UploadButton()
    {
        currentTex = Ass.ButtonUpload.Value;
        //PickRandomUpload();
    }

    public override void MouseOut(UIMouseEvent evt)
    {
        base.MouseOut(evt);
        //PickRandomUpload();
    }

    private void PickRandomUpload()
    {
        if (UploadManager.Uploads == null || UploadManager.Uploads.Count == 0)
        {
            currentTex = TextureAssets.InventoryBack.Value; // fallback
            return;
        }

        var pick = UploadManager.Uploads[rng.Next(UploadManager.Uploads.Count)];
        currentTex = pick.Texture ?? TextureAssets.InventoryBack.Value;
    }

    protected override void DrawCustom(SpriteBatch sb, Vector2 pos)
    {
        var dims = GetDimensions();
        bool forceNormal = IsMouseHovering || UI.CurrentState == State;
        ChatButtonRenderer.Draw(sb, Type, dims.Position(), 24, grayscale: !forceNormal);
    }
}
