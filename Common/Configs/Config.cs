using System;
using System.ComponentModel;
using System.Reflection;
using System.Text.Json.Serialization;
using AdvancedChatFeatures.Helpers;
using AdvancedChatFeatures.UI.Commands;
using Terraria;
using Terraria.GameContent.UI.Chat;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace AdvancedChatFeatures.Common.Configs
{
    public class Config : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Header("Features")]

        [BackgroundColor(255, 192, 8)] // Golden Yellow
        [CustomModConfigItem(typeof(ShowConfigIcon))]
        [DefaultValue(true)]
        public bool ConfigIcon;

        [BackgroundColor(255, 192, 8)] // Golden Yellow
        [CustomModConfigItem(typeof(ShowPlayerIcon))]
        [DefaultValue(true)]
        public bool PlayerIcons;

        [BackgroundColor(255, 192, 8)] // Golden Yellow
        [CustomModConfigItem(typeof(PlayerColors))]
        [DefaultValue(true)]
        public bool PlayerColors;

        [BackgroundColor(255, 192, 8)] // Golden Yellow
        [CustomModConfigItem(typeof(PlayerFormat))]
        [DrawTicks]
        [OptionStrings(["<PlayerName>", "PlayerName:"])]
        [DefaultValue("PlayerName:")]
        public string PlayerFormat;

        [BackgroundColor(255, 192, 8)] // Golden Yellow
        [CustomModConfigItem(typeof(ShowLinks))]
        [DefaultValue(true)]
        public bool Links;

        [BackgroundColor(255, 192, 8)] // Golden Yellow
        [DefaultValue(true)]
        public bool AutoCompleteCommands;

        [BackgroundColor(255, 192, 8)] // Golden Yellow
        [DefaultValue(true)]
        public bool Emojis;

        [Header("ChatMessageDisplay")]

        [BackgroundColor(192, 54, 64)] // Calamity Red
        [Range(1, 30)]
        [DrawTicks]
        [Increment(1)]
        [DefaultValue(10)]
        public int ShowCount;

        [BackgroundColor(192, 54, 64)] // Calamity Red
        [Range(1, 100)]
        [DrawTicks]
        [Increment(1)]
        [DefaultValue(10)]
        public int ChatMessageShowTime;

        [Header("Preview")]

        [JsonIgnore]
        [ShowDespiteJsonIgnore]
        [CustomModConfigItem(typeof(ChatBoxPreviewElement))]
        public int ChatBoxPreviewElement; // int is just a placeholder, it doesnt matter

        [Header("Misc")]
        [DefaultValue(true)]
        public bool ShowDebugMessages;

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

            // Update the ShowCount value.
            var showCountField = chatMonitor.GetField("_showCount", BindingFlags.NonPublic | BindingFlags.Instance);
            showCountField?.SetValue(chatMonitorInstance, (int)ShowCount);

            // Update the DrawConfigState
            var drawConfigSystem = ModContent.GetInstance<CommandsSystem>();
            var drawConfigState = drawConfigSystem.commandsListState;

            if (Conf.C.ConfigIcon)
            {
                drawConfigSystem.ui?.SetState(drawConfigState);
            }
            else
            {
                drawConfigSystem.ui?.SetState(null);
            }
        }
    }

    public static class Conf
    {
        // instance getter for faster access to the config
        public static Config C => ModContent.GetInstance<Config>();
    }
}