using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text.Json.Serialization;
using LinksInChat.Helpers;
using LinksInChat.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.UI.Chat;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace LinksInChat.Common.Configs
{
    public class Config : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Header("ChatFeatures")]

        [BackgroundColor(85, 111, 64)] // Damp Green
        [CustomModConfigItem(typeof(ShowLinks))]
        [DefaultValue(true)]
        public bool ShowLinks;

        [BackgroundColor(85, 111, 64)] // Damp Green
        [CustomModConfigItem(typeof(ShowPlayerIcon))]
        [DefaultValue(true)]
        public bool ShowPlayerIcons;

        [BackgroundColor(85, 111, 64)] // Damp Green
        [CustomModConfigItem(typeof(ShowConfigIcon))]
        [DefaultValue(true)]
        public bool ShowConfigIcon;

        [Header("ChatStyle")]

        [BackgroundColor(255, 192, 8)] // Golden Yellow
        [CustomModConfigItem(typeof(PlayerColors))]
        [DefaultValue(true)]
        public bool PlayerColors;

        [BackgroundColor(255, 192, 8)] // Golden Yellow
        [CustomModConfigItem(typeof(PlayerFormat))]
        [DrawTicks]
        [OptionStrings(["<PlayerName>", "PlayerName:"])]
        [DefaultValue("PlayerName:")]
        public string PlayerNameFormat;

        // [BackgroundColor(255, 192, 8)] // Golden Yellow
        // [DefaultValue(typeof(Color), "255,255,255,255")]
        // public Color ChatBoxColor = Color.White;

        // [BackgroundColor(255, 192, 8)] // Golden Yellow
        // [DefaultValue(typeof(Color), "255,255,255,255")]
        // public Color ChatBoxTextColor = Color.White;

        // [BackgroundColor(255, 192, 8)] // Golden Yellow
        // [DefaultValue(typeof(Vector2), "0.1, 0.9")]
        // [Increment(0.1f)]
        // public Vector2 ChatBoxPosition;

        [Header("ChatLimits")]

        [BackgroundColor(192, 54, 64)] // Calamity Red
        [Range(1, 20)]
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

        public override void OnChanged()
        {
            base.OnChanged();

            Log.Info("Config changed!");

            // Get ChatMonitor
            var chatMonitor = typeof(RemadeChatMonitor);
            var chatMonitorInstance = Main.chatMonitor as RemadeChatMonitor;

            // Update the ShowCount value.
            var showCountField = chatMonitor.GetField("_showCount", BindingFlags.NonPublic | BindingFlags.Instance);
            showCountField?.SetValue(chatMonitorInstance, ShowCount);
        }
    }

    public static class Conf
    {
        // instance getter for faster access to the config
        public static Config C => ModContent.GetInstance<Config>();
    }
}