using System;
using System.ComponentModel;
using System.Reflection;
using System.Text.Json.Serialization;
using AdvancedChatFeatures.Helpers;
using AdvancedChatFeatures.UI.Commands;
using AdvancedChatFeatures.UI.DrawConfig;
using Terraria;
using Terraria.GameContent.UI.Chat;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace AdvancedChatFeatures.Common.Configs
{
    public class Config : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Header("Config")]

        [Expand(true, false)]
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
            public bool Emojis = true;
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
                Log.Error("Config is null!");
                return;
            }
            else
            {
                Log.Info("Config changed!");
            }

            // Get ChatMonitor
            var chatMonitor = typeof(RemadeChatMonitor);
            var chatMonitorInstance = Main.chatMonitor as RemadeChatMonitor;

            // Update the ChatsVisible value.
            var showCountField = chatMonitor.GetField("_showCount", BindingFlags.NonPublic | BindingFlags.Instance);
            showCountField?.SetValue(chatMonitorInstance, (int)Conf.C.chatMessageDisplay.ChatsVisible);

            // Update the ConfigSystem
            var drawConfigSystem = ModContent.GetInstance<DrawConfigSystem>();
            if (drawConfigSystem == null)
            {
                Log.Error("DrawConfigSystem null!!");
                return;
            }

            if (Conf.C.features.ConfigIcon)
            {
                drawConfigSystem.ui?.SetState(drawConfigSystem.drawConfigState);
            }
            else
            {
                drawConfigSystem.ui?.SetState(null);
            }

            // Update the CommandSystem
            var commandSystem = ModContent.GetInstance<CommandSystem>();
            if (commandSystem == null)
            {
                Log.Error("commandSystem null!!");
                return;
            }

            if (Conf.C.features.ConfigIcon)
            {
                commandSystem.ui?.SetState(commandSystem.commandState);
            }
            else
            {
                commandSystem.ui?.SetState(null);
            }

            // Update autocomplete
            commandSystem.commandState.commandPanel.UpdateItemCount(Conf.C.autocompleteConfig.CommandsVisible);

            if (Conf.C.autocompleteConfig.ShowUsagePanel)
            {
                if (!commandSystem.commandState.HasChild(commandSystem.commandState.tooltipPanel))
                {
                    commandSystem.commandState.Append(commandSystem.commandState.tooltipPanel);
                }
            }
            else
            {
                if (commandSystem.commandState.HasChild(commandSystem.commandState.tooltipPanel))
                {
                    commandSystem.commandState.RemoveChild(commandSystem.commandState.tooltipPanel);
                }
            }
        }
    }

    public static class Conf
    {
        // instance getter for faster access to the config
        public static Config C => ModContent.GetInstance<Config>();
    }
}