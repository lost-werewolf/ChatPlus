using System;
using System.ComponentModel;
using ChatPlus.Common.Configs.ConfigElements;
using ChatPlus.Core.Features.Mentions;
using ChatPlus.Core.Features.PlayerColors;
using ChatPlus.Core.Helpers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

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
        public bool Emojis = true;

        [BackgroundColor(255, 192, 8)] // Golden Yellow
        [DefaultValue(true)]
        public bool TextEditor = true;

        [BackgroundColor(255, 192, 8)] // Golden Yellow
        [DefaultValue(true)]
        public bool Scrollbar = true;

        [BackgroundColor(255, 192, 8)] // Golden Yellow
        [Range(10f, 20f)]
        [Increment(1f)]
        [DefaultValue(10f)]
        public float ChatsVisible = 10f;

        [BackgroundColor(255, 192, 8)] // Golden Yellow
        [Range(10f, 20f)]
        [Increment(1f)]
        [DefaultValue(10f)]
        public float AutocompleteItemsVisible = 10f;

        [Header("ChatFormat")]

        [CustomModConfigItem(typeof(ModIconsConfigElement))]
        [BackgroundColor(85, 111, 64)] // Damp Green
        [DefaultValue(true)]
        public bool ModIcons = true;

        [CustomModConfigItem(typeof(PlayerIconsConfigElement))]
        [BackgroundColor(85, 111, 64)] // Damp Green
        [DefaultValue(true)]
        public bool PlayerIcons = true;

        [BackgroundColor(85, 111, 64)] // Damp Green
        [CustomModConfigItem(typeof(PlayerColorConfigElement))]
        [DefaultValue("FFFFFF")]
        public string PlayerColor = "FFFFFF";

        [Header("Stats")]

        [BackgroundColor(192, 54, 64)] // Calamity Red
        [DefaultValue(true)]
        public bool ShowModStatsWhenHovering = true;

        [BackgroundColor(192, 54, 64)] // Calamity Red
        [DefaultValue(true)]
        public bool ShowPlayerStatsWhenHovering = true;

        [BackgroundColor(192, 54, 64)] // Calamity Red
        [DefaultValue(true)]
        public bool ShowUploadWhenHovering = true;

        [BackgroundColor(192, 54, 64)] // Calamity Red
        [DefaultValue(true)]
        public bool DisableStatsWhenBossIsAlive = true;

        public override void OnChanged()
        {
            base.OnChanged();

            if (Conf.C == null) return;

            try
            {
                var hex = (Conf.C.PlayerColor ?? "FFFFFF").Trim().TrimStart('#').ToUpperInvariant();

                // Update local table immediately
                if (Main.LocalPlayer != null)
                {
                    AssignPlayerColorsSystem.PlayerColors[Main.myPlayer] = hex;
                    MentionSnippet.InvalidateCachesFor(Main.LocalPlayer.name);
                }

                // In MP, notify server so it can rebroadcast to everyone
                if (Main.netMode == NetmodeID.MultiplayerClient && Main.LocalPlayer != null)
                {
                    PlayerColorNetHandler.ClientHello(Main.myPlayer, hex);
                }
                MentionSnippet.ClearAllCaches();
            }
            catch (Exception e)
            {
                Log.Error($"[AssignPlayerColor] OnChanged exception: {e}");
            }
        }

    }

    public static class Conf
    {
        // instance getter for faster access to the config
        public static Config C => ModContent.GetInstance<Config>();
    }
}