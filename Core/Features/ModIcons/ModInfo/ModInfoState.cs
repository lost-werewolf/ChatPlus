using System;
using System.Reflection;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.UI; // UICommon + UI extensions
using Terraria.UI;

namespace ChatPlus.Core.Features.ModIcons.ModInfo
{
    // Minimal Mod Info UI (workshop/delete removed)
    public class ModInfoState : UIState, ILoadable
    {
        public static ModInfoState Instance; // for easy access

        public string CurrentModDescription = string.Empty;
        public string modDisplayName = string.Empty;
        private string modInternalName = string.Empty;

        private UIElement _messageBox; // reflected message box (scrollable)
        private static Type _messageBoxType;
        private static MethodInfo _setTextMethod;

        private UITextPanel<string> _titlePanel;
        private ChatSession.Snapshot? _returnSnapshot;

        public void Load(Mod mod)
        {
            Instance = this;
            var asm = typeof(UICommon).Assembly; // tML UI assembly
            _messageBoxType = asm.GetType("Terraria.ModLoader.UI.UIMessageBox");
            _setTextMethod = _messageBoxType?.GetMethod("SetText");
        }

        public void Unload() => Instance = null;

        public override void OnInitialize()
        {
            var root = new UIElement
            {
                Width = { Percent = 0.8f },
                MaxWidth = new StyleDimension(800f, 0f),
                Top = { Pixels = 220f },
                Height = { Pixels = -220f, Percent = 1f },
                HAlign = 0.5f
            };
            Append(root);

            var panel = new UIPanel
            {
                Width = { Percent = 1f },
                Height = { Pixels = -110f, Percent = 1f },
                BackgroundColor = UICommon.MainPanelBackground
            };
            root.Append(panel);

            if (_messageBoxType != null)
            {
                _messageBox = (UIElement)Activator.CreateInstance(
                    _messageBoxType,
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                    null,
                    new object[] { string.Empty },
                    null);
                _messageBox.Width.Set(-20f, 1f);
                _messageBox.Height.Set(0, 1f);
                panel.Append(_messageBox);
            }

            _titlePanel = new UITextPanel<string>("Mod Info", 0.8f, true)
            {
                HAlign = 0.5f,
                Top = { Pixels = -35f },
                BackgroundColor = UICommon.DefaultUIBlue
            }.WithPadding(15f);
            root.Append(_titlePanel);

            var bottom = new UIElement
            {
                Width = { Percent = 1f },
                Height = { Pixels = 40f },
                VAlign = 1f,
                Top = { Pixels = -60f }
            };
            root.Append(bottom);

            var backButton = new UITextPanel<string>(Language.GetText("UI.Back").Value)
            {
                Width = { Percent = 1f },
                Height = { Pixels = 40f }
            }.WithFadedMouseOver();
            backButton.OnLeftClick += BackButton_OnLeftClick;
            bottom.Append(backButton);
        }

        public override void OnActivate()
        {
            ApplyCurrentDescription();
            _titlePanel?.SetText($"Mod Info: {modDisplayName}");
        }

        public void SetModInfo(string description, string displayName, string internalName)
        {
            CurrentModDescription = description ?? string.Empty;
            modDisplayName = displayName ?? internalName ?? string.Empty;
            modInternalName = internalName ?? displayName ?? string.Empty;
            ApplyCurrentDescription();
        }

        public void SetReturnSnapshot(ChatSession.Snapshot snap) => _returnSnapshot = snap;

        private void ApplyCurrentDescription()
        {
            if (_messageBox != null && _setTextMethod != null)
                _setTextMethod.Invoke(_messageBox, new object[] { CurrentModDescription });
        }

        private void BackButton_OnLeftClick(UIMouseEvent evt, UIElement listeningElement)
        {
            IngameFancyUI.Close();
            if (_returnSnapshot.HasValue)
            {
                ChatSession.Restore(_returnSnapshot.Value);
                _returnSnapshot = null;
            }
        }
    }
}