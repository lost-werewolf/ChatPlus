using System;
using System.ComponentModel;
using ChatPlus.Common.Configs.ConfigElements;
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
        public bool ShowModPreviewWhenHovering = true;

        [BackgroundColor(255, 192, 8)] // Golden Yellow
        [DefaultValue(true)]
        public bool ShowPlayerPreviewWhenHovering = true;

        [BackgroundColor(255, 192, 8)] // Golden Yellow
        [DefaultValue(true)]
        public bool ShowUploadPreviewWhenHovering = true;

        [BackgroundColor(255, 192, 8)] // Golden Yellow
        [DefaultValue(true)]
        public bool DisableHoverWhenBossIsAlive = true;

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
                }

                // In MP, notify server so it can rebroadcast to everyone
                if (Main.netMode == NetmodeID.MultiplayerClient && Main.LocalPlayer != null)
                {
                    PlayerColorNetHandler.ClientHello(Main.myPlayer, hex);
                }
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