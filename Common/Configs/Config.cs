using System;
using System.ComponentModel;
using System.Reflection;
using ChatPlus.Common.Configs.ConfigElements;
using ChatPlus.Common.Configs.ConfigElements.Base;
using ChatPlus.Core.Features.Mentions;
using ChatPlus.Core.Features.PlayerColors;
using ChatPlus.Core.Features.Stats.PlayerStats.StatsPrivacy;
using ChatPlus.Core.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Terraria;
using Terraria.GameContent.UI.Chat;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace ChatPlus.Common.Configs;

public class Config : ModConfig
{
    #region Enums
    public enum Privacy
    {
        NoOne,
        Team,
        Everyone
    }
    public enum TimestampSettings
    {
        Off,
        HourAndMinute12Hours,
        HourAndMinuteAndSeconds12Hours,
        HourAndMinute24Hours,
        HourAndMinuteAndSeconds24Hours,
    }
    public enum Viewmode
    {
        ListView,
        GridView
    }

    #endregion
    public override ConfigScope Mode => ConfigScope.ClientSide;

    [Header("ChatSettings")]

    [Expand(false,false)]
    [BackgroundColor(255, 192, 8)] // Golden Yellow
    public AutocompleteSettings autocompleteSettings;

    [BackgroundColor(255, 192, 8)] // Golden Yellow
    [DefaultValue(true)]
    public bool Scrollbar = true;

    [BackgroundColor(255, 192, 8)] // Golden Yellow
    [DefaultValue(true)]
    public bool TextEditor = true;

    [BackgroundColor(255, 192, 8)] // Golden Yellow
    [Range(10f, 20f)]
    [Increment(1f)]
    [DefaultValue(10f)]
    [DrawTicks]
    public float ChatsVisible = 10f;

    // -----------------------------------------------------------

    [Header("ChatFormat")]

    [CustomModConfigItem(typeof(EnumStringOptionElement<TimestampSettings>))]
    [BackgroundColor(128, 255, 128)] // Grass Green
    [DefaultValue(TimestampSettings.Off)]
    [JsonConverter(typeof(StringEnumConverter))]
    public TimestampSettings timestampSettings;

    [CustomModConfigItem(typeof(TypingIndicatorsConfigElement))]
    [BackgroundColor(128, 255, 128)] // Grass Green
    [DefaultValue(true)]
    public bool TypingIndicators = true;

    [CustomModConfigItem(typeof(ModIconsConfigElement))]
    [BackgroundColor(128, 255, 128)] // Grass Green

    [DefaultValue(true)]
    public bool ModIcons;

    [CustomModConfigItem(typeof(PlayerIconsConfigElement))]
    [BackgroundColor(128, 255, 128)] // Grass Green
    [DefaultValue(true)]
    public bool PlayerIcons;

    [BackgroundColor(128, 255, 128)] // Grass Green
    [CustomModConfigItem(typeof(PlayerColorConfigElement))]
    [DefaultValue("FFFFFF")]
    public string PlayerColor;

    // -----------------------------------------------------------

    [Header("StatsViewer")]

    [BackgroundColor(192, 54, 64)] // Calamity Red
    [DefaultValue(true)]
    public bool ShowStatsWhenHovering;

    [BackgroundColor(192, 54, 64)] // Calamity Red
    [DefaultValue(true)]
    public bool ShowUploadsWhenHovering;

    [BackgroundColor(192, 54, 64)] // Calamity Red
    [DefaultValue(false)]
    public bool ShowStatsWhenBossIsAlive;

    [BackgroundColor(192, 54, 64)] // Calamity Red
    [DefaultValue(Privacy.Everyone)]
    [JsonConverter(typeof(StringEnumConverter))]
    public Privacy StatsPrivacy;

    // -----------------------------------------------------------

    public class AutocompleteSettings
    {
        [BackgroundColor(255, 192, 8)]
        [DefaultValue(true)]
        public bool Autocomplete = true;

        [BackgroundColor(255, 192, 8)]
        [Range(10f, 20f)]
        [Increment(1f)]
        [DefaultValue(10f)]
        [DrawTicks]
        public float AutocompleteItemsVisible = 10f;

        [CustomModConfigItem(typeof(EnumStringOptionElement<Viewmode>))]
        [BackgroundColor(255, 192, 8)]
        [DefaultValue(Viewmode.ListView)]
        [JsonConverter(typeof(StringEnumConverter))]
        public Viewmode View_Commands;

        [CustomModConfigItem(typeof(EnumStringOptionElement<Viewmode>))]
        [BackgroundColor(255, 192, 8)]
        [DefaultValue(Viewmode.GridView)]
        [JsonConverter(typeof(StringEnumConverter))]
        public Viewmode View_Colors;

        [CustomModConfigItem(typeof(EnumStringOptionElement<Viewmode>))]
        [BackgroundColor(255, 192, 8)]
        [DefaultValue(Viewmode.GridView)]
        [JsonConverter(typeof(StringEnumConverter))]
        public Viewmode View_Emojis;

        [CustomModConfigItem(typeof(EnumStringOptionElement<Viewmode>))]
        [BackgroundColor(255, 192, 8)]
        [DefaultValue(Viewmode.GridView)]
        [JsonConverter(typeof(StringEnumConverter))]
        public Viewmode View_Glyphs;

        [CustomModConfigItem(typeof(EnumStringOptionElement<Viewmode>))]
        [BackgroundColor(255, 192, 8)]
        [DefaultValue(Viewmode.GridView)]
        [JsonConverter(typeof(StringEnumConverter))]
        public Viewmode View_Items;

        [CustomModConfigItem(typeof(EnumStringOptionElement<Viewmode>))]
        [BackgroundColor(255, 192, 8)]
        [DefaultValue(Viewmode.ListView)]
        [JsonConverter(typeof(StringEnumConverter))]
        public Viewmode View_ModIcons;

        [CustomModConfigItem(typeof(EnumStringOptionElement<Viewmode>))]
        [BackgroundColor(255, 192, 8)]
        [DefaultValue(Viewmode.ListView)]
        [JsonConverter(typeof(StringEnumConverter))]
        public Viewmode View_Mentions;

        [CustomModConfigItem(typeof(EnumStringOptionElement<Viewmode>))]
        [BackgroundColor(255, 192, 8)]
        [DefaultValue(Viewmode.ListView)]
        [JsonConverter(typeof(StringEnumConverter))]
        public Viewmode View_PlayerIcons;

        [CustomModConfigItem(typeof(EnumStringOptionElement<Viewmode>))]
        [BackgroundColor(255, 192, 8)]
        [DefaultValue(Viewmode.ListView)]
        [JsonConverter(typeof(StringEnumConverter))]
        public Viewmode View_Uploads;
    }

    public override void OnChanged()
    {
        base.OnChanged();

        if (Conf.C == null) return;

        var field = typeof(RemadeChatMonitor)
        .GetField("_showCount", BindingFlags.Instance | BindingFlags.NonPublic);

        if (field != null)
        {
            field.SetValue(Main.chatMonitor, (int) Conf.C.ChatsVisible);
        }

        UpdatePlayerColor();

        PrivacyCache.Set(Main.myPlayer, StatsPrivacy);

        if (Main.netMode == NetmodeID.MultiplayerClient)
        {
            PrivacyNetHandler.Instance.SendLocalPrivacy();
        }
        else if (Main.netMode == NetmodeID.Server)
        {
            PrivacyNetHandler.Instance.BroadcastSingle(Main.myPlayer, StatsPrivacy);
        }
    }

    private void UpdatePlayerColor()
    {
        try
        {
            string hex = (Conf.C.PlayerColor ?? "FFFFFF")
                .Trim()
                .TrimStart('#')
                .ToUpperInvariant();

            if (Main.LocalPlayer != null)
            {
                PlayerColorSystem.PlayerColors[Main.myPlayer] = hex;
                MentionSnippet.InvalidateCachesFor(Main.LocalPlayer.name);
                Log.Info("Player color updated to: " + hex + " for: " + Main.LocalPlayer.name);
            }

            if (Main.netMode == NetmodeID.MultiplayerClient && Main.LocalPlayer != null)
            {
                PlayerColorNetHandler.ClientHello(Main.myPlayer, hex);
            }
            MentionSnippet.ClearAllCaches();
        }
        catch (Exception e)
        {
            Log.Error("UpdatePlayerColor Exception: " + e);
        }
    }
}

public static class Conf
{
    public static Config C => ModContent.GetInstance<Config>();
}