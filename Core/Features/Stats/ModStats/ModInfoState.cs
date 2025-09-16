using System;
using System.Reflection;
using ChatPlus.Core.Features.Stats.Base;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace ChatPlus.Core.Features.Stats.ModStats;

public class ModInfoState : BaseInfoState, ILoadable
{
    public static ModInfoState Instance;

    public string CurrentModDescription = string.Empty;
    public string modDisplayName = string.Empty;

    private UIElement _messageBox; // reflected message box (scrollable)
    private static Type _messageBoxType;
    private static MethodInfo _setTextMethod;

    private ChatSession.Snapshot? _returnSnapshot;

    public void Load(Mod mod)
    {
        Instance = this;
        var asm = typeof(UICommon).Assembly; // tML UI assembly
        _messageBoxType = asm.GetType("Terraria.ModLoader.UI.UIMessageBox");
        _setTextMethod = _messageBoxType?.GetMethod("SetText");
    }

    public void Unload() => Instance = null;

    protected override string DefaultTitle => "Mod Info";
    protected override float TopOffsetPx => 220f;              // keep your original layout
    protected override float ContentBottomPaddingPx => 110f;   // matches panel Height = -110

    public override void OnInitialize()
    {
        // Build base chrome (Root, MainPanel, TitlePanel, BottomBar, BackButton)
        base.OnInitialize();

        // Add the scrollable message box into MainPanel, like before
        if (_messageBoxType != null)
        {
            _messageBox = (UIElement)Activator.CreateInstance(
                _messageBoxType,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                binder: null,
                args: [string.Empty],
                culture: null
            );
            _messageBox.Width.Set(-20f, 1f);
            _messageBox.Height.Set(0, 1f);
            MainPanel.Append(_messageBox);
        }
    }

    public override void OnActivate()
    {
        ApplyCurrentDescription();
        SetTitle($"Mod Info: {modDisplayName}");
    }

    public void SetModInfo(string description, string displayName, string internalName)
    {
        CurrentModDescription = description ?? string.Empty;
        modDisplayName = displayName ?? internalName ?? string.Empty;
        ApplyCurrentDescription();
        SetTitle($"Mod Info: {modDisplayName}");
    }

    public void SetReturnSnapshot(ChatSession.Snapshot snap) => _returnSnapshot = snap;

    private void ApplyCurrentDescription()
    {
        if (_messageBox != null && _setTextMethod != null)
            _setTextMethod.Invoke(_messageBox, [CurrentModDescription]);
    }

    protected override void OnBackClicked(UIMouseEvent evt, UIElement listeningElement)
    {
        CloseForCurrentContext();           

        if (_returnSnapshot.HasValue)
        {
            ChatSession.Restore(_returnSnapshot.Value);
            _returnSnapshot = null;
        }
    }
}
