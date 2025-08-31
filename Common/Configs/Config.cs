using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using AdvancedChatFeatures.Common.Configs;
using ChatPlus.Common.Configs.ConfigElements;
using ChatPlus.Core.Helpers;
using Newtonsoft.Json;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.UI;

namespace ChatPlus.Common.Configs
{
    public class Config : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Header("Config")]

        [BackgroundColor(255, 192, 8)] // Golden Yellow
        [DefaultValue(true)]
        public bool Autocomplete = true;

        [BackgroundColor(255, 192, 8)] // Golden Yellow
        [DefaultValue(true)]
        public bool BetterTextEditor = true;

        [BackgroundColor(255, 192, 8)] // Golden Yellow
        [DefaultValue(true)]
        public bool Scrollbar = true;
        

        [BackgroundColor(255, 192, 8)] // Golden Yellow
        [Range(10f, 20f)]
        [Increment(1f)]
        [DefaultValue(10f)]
        public float ChatItemCount = 10f;

        [BackgroundColor(255, 192, 8)] // Golden Yellow
        [Range(10f, 20f)]
        [Increment(1f)]
        [DefaultValue(10f)]
        public float AutocompleteItemCount = 10f;

        [BackgroundColor(255, 192, 8)] // Golden Yellow
        [DefaultValue(true)]
        public bool ShowPlayerPreviewWhenHovering = true;

        [Header("ChatFormat")]

        [CustomModConfigItem(typeof(ModIconsConfigElement))]
        [BackgroundColor(255, 192, 8)] // Golden Yellow
        [DefaultValue(true)]
        public bool ModIcons = true;

        [CustomModConfigItem(typeof(PlayerIconsConfigElement))]
        [BackgroundColor(255, 192, 8)] // Golden Yellow
        [DefaultValue(true)]
        public bool PlayerIcons = true;

        [Header("PlayerFormat")]

        [BackgroundColor(255, 192, 8)]
        [CustomModConfigItem(typeof(PlayerColorConfigElement))]
        [DefaultValue("FFFFFF")]
        public string PlayerColor;

        public override void OnChanged()
        {
            base.OnChanged();

            if (Conf.C == null)
            {
                Log.Error("Config is null in OnChanged!");
                return;
            }
        }
    }

    public static class Conf
    {
        // instance getter for faster access to the config
        public static Config C => ModContent.GetInstance<Config>();
    }
}