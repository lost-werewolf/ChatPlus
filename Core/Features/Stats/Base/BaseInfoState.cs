using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace ChatPlus.Core.Features.Stats.Base;

/// <summary>
/// Parent state that displays a info overlay.
/// Contains: a UIText panel with content, a title panel, and a back panel.
/// Also handles chat session snapshot storage to return to previous chat states.
/// </summary>
public abstract class BaseInfoState : UIState
{
    protected UIElement Root;
    protected UIPanel MainPanel;
    protected UITextPanel<string> TitlePanel;
    protected UIElement BottomBar;
    protected UITextPanel<string> BackButton;

    protected ChatSession.Snapshot? _returnSnapshot;

    public override void OnInitialize()
    {
        // Positions
        float TopOffsetPx = 120f;
        float TitleOffsetPx = -35f;
        float ContentBottomPaddingPx = 110f;
        float BottomBarOffsetPx = -60f;
        string DefaultTitle = "Info";

        Root = new UIElement
        {
            Width = { Percent = 0.8f },
            MaxWidth = new StyleDimension(800f, 0f),
            Top = { Pixels = TopOffsetPx },
            Height = { Pixels = -TopOffsetPx, Percent = 1f },
            HAlign = 0.5f
        };
        Append(Root);

        MainPanel = new UIPanel
        {
            Width = { Percent = 1f },
            Height = { Pixels = -ContentBottomPaddingPx, Percent = 1f },
            BackgroundColor = UICommon.MainPanelBackground
        };
        Root.Append(MainPanel);

        // Title panel
        TitlePanel = new UITextPanel<string>(DefaultTitle, 0.8f, true)
        {
            HAlign = 0.5f,
            Top = { Pixels = TitleOffsetPx },
            BackgroundColor = UICommon.DefaultUIBlue
        }.WithPadding(15f);
        Root.Append(TitlePanel);

        // Back button
        BottomBar = new UIElement
        {
            Width = { Percent = 1f },
            Height = { Pixels = 40f },
            VAlign = 1f,
            Top = { Pixels = BottomBarOffsetPx }
        };
        Root.Append(BottomBar);

        BackButton = new UITextPanel<string>(Terraria.Localization.Language.GetTextValue("UI.Back"))
        {
            Width = { Percent = 1f },
            Height = { Pixels = 40f }
        }.WithFadedMouseOver();

        BackButton.OnLeftClick += OnBackClicked;
        BottomBar.Append(BackButton);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);

        if (MainPanel != null)
        {
            DrawContent(spriteBatch, GetInnerRect(MainPanel, 12));
        }
    }

    // ---- Hooks for children ----
    protected virtual void DrawContent(SpriteBatch sb, Rectangle inner) { }

    protected virtual void OnBackClicked(UIMouseEvent evt, UIElement listeningElement)
    {
        CloseForCurrentContext();
    }

    protected Rectangle GetInnerRect(UIElement element, int paddingPx)
    {
        var dims = element.GetInnerDimensions();
        return new Rectangle(
            (int)dims.X + paddingPx,
            (int)dims.Y + paddingPx,
            (int)dims.Width - paddingPx * 2,
            (int)dims.Height - paddingPx * 2
        );
    }

    public void OpenForCurrentContext()
    {
        _returnSnapshot = ChatSession.Capture();

        bool openedFromMap = Main.mapFullscreen;

        Main.ClosePlayerChat();
        Main.chatText = string.Empty;

        // TODO does this fix double cursor bug? prob not
        if (!openedFromMap)
            Main.inFancyUI = true;

        Main.InGameUI.SetState(this);
    }


    protected void CloseForCurrentContext()
    {
        if (Main.InGameUI.CurrentState == this)
            Main.InGameUI.SetState(null);     

        Main.inFancyUI = false; 

        if (_returnSnapshot.HasValue)
        {
            ChatSession.Restore(_returnSnapshot.Value);
            _returnSnapshot = null;
        }
    }

    protected void SetTitle(string text) => TitlePanel?.SetText(text ?? string.Empty);

    // Useful for input-capture checks in “draw-after-map” hook
    public bool IsMouseOverRoot() => Root != null && Root.ContainsPoint(Main.MouseScreen);
}
