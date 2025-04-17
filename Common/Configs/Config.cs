using System.ComponentModel;
using System.Text.Json.Serialization;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace LinksInChat.Common.Configs
{
    public class Config : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Header("Settings")]

        [CustomModConfigItem(typeof(ShowLinks))]
        [DefaultValue(true)]
        public bool ShowLinks;

        [CustomModConfigItem(typeof(ShowPlayerIcon))]
        [DefaultValue(true)]
        public bool ShowPlayerIcons;

        [CustomModConfigItem(typeof(ShowConfigIcon))]
        [DefaultValue(true)]
        public bool ShowConfigIcon;

        [CustomModConfigItem(typeof(PlayerColors))]
        [DefaultValue(true)]
        public bool PlayerColors;

        [DrawTicks]
        //[CustomModConfigItem(typeof(PlayerFormat))]
        [OptionStrings(["<PlayerName>", "PlayerName:"])]
        [DefaultValue("<PlayerName>")]
        public string PlayerFormat;

        [Header("Preview")]

        [JsonIgnore]
        [ShowDespiteJsonIgnore]
        [CustomModConfigItem(typeof(ChatBoxPreviewElement))]
        public int ChatBoxPreviewElement; // int is just a placeholder, it doesnt matter
    }

    public static class Conf
    {
        // instance getter for faster access to the config
        public static Config C => ModContent.GetInstance<Config>();
    }
}