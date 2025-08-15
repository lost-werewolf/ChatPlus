using System;
using System.ComponentModel;
using System.Reflection;
using System.Text.Json.Serialization;
using AdvancedChatFeatures.Helpers;
using AdvancedChatFeatures.UI.Commands;
using AdvancedChatFeatures.UI.DrawConfig;
using AdvancedChatFeatures.UI.Emojis;
using Terraria;
using Terraria.GameContent.UI.Chat;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.UI;

namespace AdvancedChatFeatures.Common.Configs
{
    public class Config : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Header("Config")]

        [Expand(false, false)]
        [BackgroundColor(255, 192, 8)] // Golden Yellow
        public Features features = new();

        [BackgroundColor(192, 54, 64)] // Calamity Red
        [Expand(false, false)]
        public ChatMessageDisplay chatMessageDisplay = new();

        [BackgroundColor(85, 111, 64)] // Damp Green
        [Expand(false, false)]
        public AutocompleteConfig autocompleteConfig = new();

        [BackgroundColor(231, 84, 128)] // Rose Pink
        [Expand(false, false)]
        public EmojisConfig emojisConfig = new();

        [Header("Preview")]

        [JsonIgnore]
        [ShowDespiteJsonIgnore]
        [CustomModConfigItem(typeof(ChatBoxPreviewElement))]
        public int ChatBoxPreviewElement; // int is just a placeholder, it doesnt matter

        //[Header("Misc")]
        //[DefaultValue(true)]
        //public bool ShowDebugMessages;

        public class EmojisConfig
        {
            [BackgroundColor(231, 84, 128)] // Rose Pink
            [DefaultValue(true)]
            public bool EnableEmojis = true;

            [BackgroundColor(231, 84, 128)] // Rose Pink
            [Range(3, 20)]
            [DrawTicks]
            [Increment(1)]
            [DefaultValue(10)]
            public int EmojisVisible = 10;

            [BackgroundColor(231, 84, 128)] // Rose Pink
            [DefaultValue(true)]
            public bool ShowUsagePanel = true;
        }

        public class Features
        {
            [BackgroundColor(255, 192, 8)] // Golden Yellow
            [CustomModConfigItem(typeof(ShowConfigIcon))]
            [DefaultValue(true)]
            public bool ConfigIcon = true;

            [BackgroundColor(255, 192, 8)] // Golden Yellow
            [CustomModConfigItem(typeof(ShowPlayerIcon))]
            [DefaultValue(true)]
            public bool PlayerIcons = true;

            [BackgroundColor(255, 192, 8)] // Golden Yellow
            [CustomModConfigItem(typeof(PlayerColors))]
            [DefaultValue(true)]
            public bool PlayerColors = true;

            [BackgroundColor(255, 192, 8)] // Golden Yellow
            [CustomModConfigItem(typeof(PlayerFormat))]
            [DrawTicks]
            [OptionStrings(["<PlayerName>", "PlayerName:"])]
            [DefaultValue("<PlayerName>")]
            public string PlayerFormat = "<PlayerName>";

            [BackgroundColor(255, 192, 8)] // Golden Yellow
            [CustomModConfigItem(typeof(ShowLinks))]
            [DefaultValue(true)]
            public bool Links = true;
        }

        public class ChatMessageDisplay
        {
            [BackgroundColor(192, 54, 64)] // Calamity Red
            [Range(1, 30)]
            [DrawTicks]
            [Increment(1)]
            [DefaultValue(10)]
            public int ChatsVisible = 10;

            [BackgroundColor(192, 54, 64)] // Calamity Red
            [Range(1, 100)]
            [DrawTicks]
            [Increment(1)]
            [DefaultValue(10)]
            public int ChatMessageShowTime = 10;
        }

        public class AutocompleteConfig
        {
            [BackgroundColor(85, 111, 64)] // Damp Green
            [DefaultValue(true)]
            public bool EnableAutocomplete = true;

            [BackgroundColor(85, 111, 64)] // Damp Green
            [Range(3, 20)]
            [DrawTicks]
            [Increment(1)]
            [DefaultValue(10)]
            public int CommandsVisible = 10;

            [BackgroundColor(85, 111, 64)] // Damp Green
            [DefaultValue(true)]
            public bool ShowUsagePanel = true;

            [BackgroundColor(85, 111, 64)] // Damp Green
            [DefaultValue(true)]
            public bool ShowHoverTooltips = true;

            [BackgroundColor(85, 111, 64)] // Damp Green
            [DefaultValue(false)]
            public bool ShowGhostText = false;

            [BackgroundColor(85, 111, 64)] // Damp Green
            [DefaultValue(true)]
            public bool DraggableWindow = true;
        }

        public override void OnChanged()
        {
            base.OnChanged();

            if (Conf.C == null)
            {
                Log.Error("Config is null in OnChanged!");
                return;
            }

            UpdateChatsVisible();
            UpdateCommandSystem();
            UpdateEmojiSystem();
            UpdateDrawConfigSystem();
        }

        private void UpdateChatsVisible()
        {
            // Get ChatMonitor
            var chatMonitor = typeof(RemadeChatMonitor);
            var chatMonitorInstance = Main.chatMonitor as RemadeChatMonitor;

            // Update the ChatsVisible value.
            var showCountField = chatMonitor.GetField("_showCount", BindingFlags.NonPublic | BindingFlags.Instance);
            showCountField?.SetValue(chatMonitorInstance, (int)Conf.C.chatMessageDisplay.ChatsVisible);
        }

        private void UpdateCommandSystem()
        {
            // Update state
            CommandSystem commandSystem = ModContent.GetInstance<CommandSystem>();
            CommandState commandState = commandSystem.commandState;
            if (Conf.C.autocompleteConfig.EnableAutocomplete)
            {
                if (commandSystem.ui?.CurrentState != commandState)
                    commandSystem.ui?.SetState(commandState);
            }
            else
            {
                commandSystem.ui?.SetState(null);
                return;
            }

            // Update item count
            commandState.commandPanel.SetCommandPanelHeight();

            // Update usage panel
            if (Conf.C.autocompleteConfig.ShowUsagePanel && !commandState.HasChild(commandState.commandUsagePanel))
                commandState.Append(commandState.commandUsagePanel);
            else if (commandState.HasChild(commandState.commandUsagePanel) && !Conf.C.autocompleteConfig.ShowUsagePanel)
                commandState.RemoveChild(commandState.commandUsagePanel);
        }

        private void UpdateEmojiSystem()
        {
            // Update state
            EmojiSystem emojiSystem = ModContent.GetInstance<EmojiSystem>();
            EmojiState emojiState = emojiSystem.emojiState;
            if (Conf.C.emojisConfig.EnableEmojis)
                emojiSystem.ui?.SetState(emojiState);
            else
            {
                emojiSystem.ui?.SetState(null);
                return;
            }

            // Update item count
            emojiState.emojiPanel.SetEmojiPanelHeight();

            // Update usage panel
            if (Conf.C.emojisConfig.ShowUsagePanel && !emojiState.HasChild(emojiState.emojiUsagePanel))
                emojiState.Append(emojiState.emojiUsagePanel);
            else if (emojiState.HasChild(emojiState.emojiUsagePanel) && !Conf.C.emojisConfig.ShowUsagePanel)
                emojiState.RemoveChild(emojiState.emojiUsagePanel);
        }

        private void UpdateDrawConfigSystem()
        {
            // Update the ConfigSystem
            DrawConfigSystem drawConfigSystem = ModContent.GetInstance<DrawConfigSystem>();
            if (Conf.C.features.ConfigIcon)
                drawConfigSystem.ui?.SetState(drawConfigSystem.drawConfigState);
            else
                drawConfigSystem.ui?.SetState(null);
        }

        #region Helpers

        public bool ExpandSection(UIElement root, string labelMatch)
        {
            foreach (var child in root.Children)
            {
                // Search recursively
                if (ExpandSection(child, labelMatch))
                    return true;

                var type = child.GetType();
                if (type.Name == "ObjectElement")
                {
                    // Try to read the "Label" field from ConfigElement<T>
                    var labelField = type.BaseType?.GetField("Label", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                    var labelValue = labelField?.GetValue(child) as string;

                    if (!string.IsNullOrEmpty(labelValue) &&
                        labelValue.IndexOf(labelMatch, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        // Set private 'expanded' field to true
                        var expandedField = type.GetField("expanded", BindingFlags.Instance | BindingFlags.NonPublic);
                        expandedField?.SetValue(child, true);

                        // Also set 'pendingChanges' so Update() will rebuild the list
                        var pendingField = type.GetField("pendingChanges", BindingFlags.Instance | BindingFlags.NonPublic);
                        pendingField?.SetValue(child, true);

                        // Force recalculation
                        child.Recalculate();
                        Log.Info($"Expanded section: {labelValue}");
                        return true;
                    }
                }
            }
            return false;
        }

        #endregion
    }

    public static class Conf
    {
        // instance getter for faster access to the config
        public static Config C => ModContent.GetInstance<Config>();
    }
}