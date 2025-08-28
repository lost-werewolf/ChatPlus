using System;
using System.ComponentModel;
using System.Reflection;
using ChatPlus.Common.Configs.ConfigElements;
using ChatPlus.Core.Helpers;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.UI;

namespace ChatPlus.Common.Configs
{
    public class Config : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Header("Config")]

        [CustomModConfigItem(typeof(AutocompleteConfigElement))]
        [BackgroundColor(255, 192, 8)] // Golden Yellow
        [DefaultValue(true)]
        public bool Autocomplete = true;

        [BackgroundColor(255, 192, 8)] // Golden Yellow
        [CustomModConfigItem(typeof(EnableLinksConfigElement))]
        [DefaultValue(true)]
        public bool ShowLinks = true;

        [BackgroundColor(255, 192, 8)] // Golden Yellow
        [DefaultValue(true)]
        public bool BetterTextEditor = true;

        [BackgroundColor(255, 192, 8)] // Golden Yellow
        [DefaultValue(true)]
        public bool Scrollbar = true;

        [BackgroundColor(255, 192, 8)] // Golden Yellow
        [Range(10f, 10f)]
        [Increment(1f)]
        [DefaultValue(10f)]
        public float ChatItemCount = 10f;

        [BackgroundColor(255, 192, 8)] // Golden Yellow
        [Range(10f, 20f)]
        [Increment(1f)]
        [DefaultValue(10f)]
        public float AutocompleteItemCount = 10f;

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