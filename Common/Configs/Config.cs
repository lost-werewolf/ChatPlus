using System;
using System.ComponentModel;
using System.Reflection;
using System.Text.Json.Serialization;
using AdvancedChatFeatures.Commands;
using AdvancedChatFeatures.Common.Configs.FeatureConfigs;
using AdvancedChatFeatures.Common.Configs.StyleConfigs;
using AdvancedChatFeatures.Emojis;
using AdvancedChatFeatures.Helpers;
using AdvancedChatFeatures.UI.DrawConfig;
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
        public Features featureConfig = new();

        [BackgroundColor(231, 84, 128)] // Rose Pink
        [Expand(false, false)]
        public FeatureStyleConfig featureStyleConfig = new();

        [Expand(false, false)]
        [BackgroundColor(192, 54, 64)] // Calamity Red
        public StyleConfig styleConfig = new();

        [Header("Preview")]

        [JsonIgnore]
        [ShowDespiteJsonIgnore]
        [CustomModConfigItem(typeof(PreviewElement))]
        public int ChatBoxPreviewElement; // int is just a placeholder, it doesnt matter

        public class Features
        {
            [BackgroundColor(255, 192, 8)] // Golden Yellow
            [DefaultValue(true)]
            public bool EnableBetterChatNavigation = true;

            [CustomModConfigItem(typeof(EnableCommands))]
            [BackgroundColor(255, 192, 8)] // Golden Yellow
            [DefaultValue(true)]
            public bool EnableCommands = true;

            [CustomModConfigItem(typeof(EnableEmojis))]
            [BackgroundColor(255, 192, 8)] // Golden Yellow
            [DefaultValue(true)]
            public bool EnableEmojis = true;

            [CustomModConfigItem(typeof(EnableGlyphs))]
            [BackgroundColor(255, 192, 8)] // Golden Yellow
            [DefaultValue(true)]
            public bool EnableGlyphs = true;

            [CustomModConfigItem(typeof(EnableItemBrowser))]
            [BackgroundColor(255, 192, 8)] // Golden Yellow
            [DefaultValue(true)]
            public bool EnableItemBrowser = true;

            [CustomModConfigItem(typeof(EnableColorPicker))]
            [BackgroundColor(255, 192, 8)] // Golden Yellow
            [DefaultValue(true)]
            public bool EnableColorPicker = true;
        }

        public class FeatureStyleConfig
        {
            [BackgroundColor(231, 84, 128)] // Rose Pink
            [Range(3, 20)]
            [DrawTicks]
            [Increment(1)]
            [DefaultValue(10)]
            public int ItemsPerWindow = 10;

            [BackgroundColor(231, 84, 128)] // Rose Pink
            [DefaultValue(true)]
            public bool ShowDescriptionPanel = true;

            [BackgroundColor(231, 84, 128)] // Rose Pink
            [DefaultValue(true)]
            public bool MakeWindowDraggable = true;
        }

        public class StyleConfig
        {
            [BackgroundColor(192, 54, 64)] // Calamity Red
            [CustomModConfigItem(typeof(ShowConfigButtonElement))]
            [DefaultValue(true)]
            public bool ShowConfigButton = true;

            [BackgroundColor(192, 54, 64)] // Calamity Red
            [CustomModConfigItem(typeof(ShowPlayerIconElement))]
            [DefaultValue(true)]
            public bool ShowPlayerIcons = true;

            [BackgroundColor(192, 54, 64)] // Calamity Red
            [CustomModConfigItem(typeof(ShowModIconElement))]
            [DefaultValue(true)]
            public bool ShowModIcons = true;

            [BackgroundColor(192, 54, 64)] // Calamity Red
            [CustomModConfigItem(typeof(ShowLinksElement))]
            [DefaultValue(true)]
            public bool ShowLinks = true;

            [BackgroundColor(192, 54, 64)] // Calamity Red
            [CustomModConfigItem(typeof(ShowPlayerColorsElement))]
            [DefaultValue(true)]
            public bool ShowPlayerColors = true;

            [BackgroundColor(192, 54, 64)] // Calamity Red
            [CustomModConfigItem(typeof(ShowPlayerFormatElement))]
            [DrawTicks]
            [OptionStrings(["<PlayerName>", "PlayerName:", "(PlayerName)"])]
            [DefaultValue("<PlayerName>")]
            public string ShowPlayerFormat = "<PlayerName>";
        }
        
        public override void OnChanged()
        {
            base.OnChanged();

            if (Conf.C == null)
            {
                Log.Error("Config is null in OnChanged!");
                return;
            }

            UpdateCommandSystem();
            UpdateEmojiSystem();
            UpdateDrawConfigSystem();
        }

        private void UpdateCommandSystem()
        {
            // Update state
            CommandSystem commandSystem = ModContent.GetInstance<CommandSystem>();
            CommandState commandState = commandSystem.commandState;
            if (Conf.C.featureConfig.EnableCommands)
            {
                if (commandSystem.ui?.CurrentState != commandState)
                    commandSystem.ui?.SetState(commandState);
            }
            else
            {
                commandSystem.ui?.SetState(null);
                return;
            }

            // Update usage panel
            if (Conf.C.featureStyleConfig.ShowDescriptionPanel && !commandState.HasChild(commandState.commandDescriptionPanel))
                commandState.Append(commandState.commandDescriptionPanel);
            else if (commandState.HasChild(commandState.commandDescriptionPanel) && !Conf.C.featureStyleConfig.ShowDescriptionPanel)
                commandState.RemoveChild(commandState.commandDescriptionPanel);
        }

        private void UpdateEmojiSystem()
        {
            // Update state
            EmojiSystem emojiSystem = ModContent.GetInstance<EmojiSystem>();
            EmojiState emojiState = emojiSystem.emojiState;
            if (Conf.C.featureConfig.EnableEmojis)
                emojiSystem.ui?.SetState(emojiState);
            else
            {
                emojiSystem.ui?.SetState(null);
                return;
            }

            // Update usage panel
            if (Conf.C.featureStyleConfig.ShowDescriptionPanel && !emojiState.HasChild(emojiState.emojiDescriptionPanel))
                emojiState.Append(emojiState.emojiDescriptionPanel);
            else if (emojiState.HasChild(emojiState.emojiDescriptionPanel) && !Conf.C.featureStyleConfig.ShowDescriptionPanel)
                emojiState.RemoveChild(emojiState.emojiDescriptionPanel);
        }

        private void UpdateGlyphSystem()
        {
            // Update state
            CommandSystem commandSystem = ModContent.GetInstance<CommandSystem>();
            CommandState commandState = commandSystem.commandState;
            if (Conf.C.featureConfig.EnableCommands)
            {
                if (commandSystem.ui?.CurrentState != commandState)
                    commandSystem.ui?.SetState(commandState);
            }
            else
            {
                commandSystem.ui?.SetState(null);
                return;
            }

            // Update usage panel
            if (Conf.C.featureStyleConfig.ShowDescriptionPanel && !commandState.HasChild(commandState.commandDescriptionPanel))
                commandState.Append(commandState.commandDescriptionPanel);
            else if (commandState.HasChild(commandState.commandDescriptionPanel) && !Conf.C.featureStyleConfig.ShowDescriptionPanel)
                commandState.RemoveChild(commandState.commandDescriptionPanel);
        }

        private void UpdateDrawConfigSystem()
        {
            // Update the ConfigSystem
            DrawConfigSystem drawConfigSystem = ModContent.GetInstance<DrawConfigSystem>();
            if (Conf.C.styleConfig.ShowConfigButton)
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