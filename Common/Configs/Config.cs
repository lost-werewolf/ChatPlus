using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using ChatPlus.Common.Configs.ConfigElements;
using ChatPlus.Core.Features.Mentions;
using ChatPlus.Core.Features.PlayerColors;
using ChatPlus.Core.Features.PlayerIcons.PlayerInfo.StatsPrivacy;
using ChatPlus.Core.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace ChatPlus.Common.Configs;

public class Config : ModConfig
{
    #region Enums
    public enum UserStatsPrivacy
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
    #endregion
    public override ConfigScope Mode => ConfigScope.ClientSide;

    [Header("ChatSettings")]

    [BackgroundColor(255, 192, 8)] // Golden Yellow
    [DefaultValue(true)]
    public bool Autocomplete = true;

    [BackgroundColor(255, 192, 8)] // Golden Yellow
    [Range(10f, 20f)]
    [Increment(1f)]
    [DefaultValue(10f)]
    [DrawTicks]
    public float AutocompleteItemsVisible = 10f;

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

    [Header("ChatFormat")]

    [CustomModConfigItem(typeof(EnumStringOptionElement<TimestampSettings>))]
    [BackgroundColor(85, 111, 64)] // Damp Green
    [DefaultValue(TimestampSettings.Off)]
    [JsonConverter(typeof(StringEnumConverter))]
    public TimestampSettings timestampSettings;

    [CustomModConfigItem(typeof(ModIconsConfigElement))]
    [BackgroundColor(85, 111, 64)] // Damp Green
    [DefaultValue(true)]
    public bool ModIcons;

    [CustomModConfigItem(typeof(PlayerIconsConfigElement))]
    [BackgroundColor(85, 111, 64)] // Damp Green
    [DefaultValue(true)]
    public bool PlayerIcons;

    [BackgroundColor(85, 111, 64)] // Damp Green
    [CustomModConfigItem(typeof(PlayerColorConfigElement))]
    [DefaultValue("FFFFFF")]
    public string PlayerColor;

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
    [DefaultValue(UserStatsPrivacy.Everyone)]
    [JsonConverter(typeof(StringEnumConverter))]
    public UserStatsPrivacy StatsPrivacy;

    public override void OnChanged()
    {
        base.OnChanged();

        if (Conf.C == null) return;

        UpdatePlayerColor();

        PrivacyCache.Set(Main.myPlayer, StatsPrivacy);

        if (Main.netMode == NetmodeID.MultiplayerClient)
        {
            StatsPrivacyNetHandler.Instance.SendLocalPrivacy();
        }
        else if (Main.netMode == NetmodeID.Server)
        {
            StatsPrivacyNetHandler.Instance.BroadcastSingle(Main.myPlayer, StatsPrivacy);
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
                AssignPlayerColorsSystem.PlayerColors[Main.myPlayer] = hex;
                MentionSnippet.InvalidateCachesFor(Main.LocalPlayer.name);
                Log.Info("updated player color to: " + hex + " for: " + Main.LocalPlayer.name);
            }

            if (Main.netMode == NetmodeID.MultiplayerClient && Main.LocalPlayer != null)
            {
                PlayerColorNetHandler.ClientHello(Main.myPlayer, hex);
            }
            MentionSnippet.ClearAllCaches();
        }
        catch (Exception e)
        {
            Log.Error("[AssignPlayerColor] UpdatePlayerColor exception: " + e);
        }
    }
}

public static class Conf
{
    public static Config C => ModContent.GetInstance<Config>();
}