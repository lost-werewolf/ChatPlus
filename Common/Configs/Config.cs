using System;
using System.ComponentModel;
using System.Reflection;
using System.Text.Json.Serialization;
using ChatPlus.CommandHandler;
using ChatPlus.Common.Configs.ConfigElements;
using ChatPlus.EmojiHandler;
using ChatPlus.Helpers;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.UI;

namespace ChatPlus.Common.Configs
{
    public class Config : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Header("Config")]

        [Expand(false, false)]
        [BackgroundColor(255, 192, 8)] // Golden Yellow
        public Features featuresConfig = new();

        [BackgroundColor(231, 84, 128)] // Rose Pink
        [Expand(false, false)]
        public AutocompleteWindowConfig autocompleteWindowConfig = new();

        public class Features
        {
            [CustomModConfigItem(typeof(AutocompleteTagsConfigElement))]
            [BackgroundColor(255, 192, 8)] // Golden Yellow
            [DefaultValue(true)]
            public bool AutocompleteTags = true;

            [CustomModConfigItem(typeof(AutocompleteCommandsConfigElement))]
            [BackgroundColor(255, 192, 8)] // Golden Yellow
            [DefaultValue(true)]
            public bool AutocompleteCommands = true;

            [BackgroundColor(255, 192, 8)] // Golden Yellow
            [CustomModConfigItem(typeof(EnableEmojisConfigElement))]
            [DefaultValue(true)]
            public bool EnableEmojis = true;

            [BackgroundColor(255, 192, 8)] // Golden Yellow
            [CustomModConfigItem(typeof(EnableUploadImagesConfigElement))]
            [DefaultValue(true)]
            public bool EnableUploadImages = true;

            [BackgroundColor(255, 192, 8)] // Golden Yellow
            [CustomModConfigItem(typeof(EnableLinksConfigElement))]
            [DefaultValue(true)]
            public bool EnableLinks = true;

            [BackgroundColor(255, 192, 8)] // Golden Yellow
            [DefaultValue(true)]
            public bool EnableTextEditingShortcuts = true;
        }

        public class AutocompleteWindowConfig
        {
            [BackgroundColor(231, 84, 128)] // Rose Pink
            [Range(3, 20)]
            [DrawTicks]
            [Increment(1)]
            [DefaultValue(10)]
            public int ItemsPerWindow = 10;
        }
        public override void OnChanged()
        {
            base.OnChanged();

            if (Conf.C == null)
            {
                Log.Error("Config is null in OnChanged!");
                return;
            }
        }

        #region Helpers

        public bool ExpandSection(UIElement root, string labelMatch)
        {
            foreach (var child in root.Children)
            {
                // Search recursively
                if (ExpandSection(child, labelMatch))
                    return true;

                var type = child.GetType();
                if (type.Name == "ObjectElement")
                {
                    // Try to read the "Label" field from ConfigElement<T>
                    var labelField = type.BaseType?.GetField("Label", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                    var labelValue = labelField?.GetValue(child) as string;

                    if (!string.IsNullOrEmpty(labelValue) &&
                        labelValue.IndexOf(labelMatch, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        // Set private 'expanded' field to true
                        var expandedField = type.GetField("expanded", BindingFlags.Instance | BindingFlags.NonPublic);
                        expandedField?.SetValue(child, true);

                        // Also set 'pendingChanges' so Update() will rebuild the list
                        var pendingField = type.GetField("pendingChanges", BindingFlags.Instance | BindingFlags.NonPublic);
                        pendingField?.SetValue(child, true);

                        // Force recalculation
                        child.Recalculate();
                        Log.Info($"Expanded section: {labelValue}");
                        return true;
                    }
                }
            }
            return false;
        }

        #endregion
    }

    public static class Conf
    {
        // instance getter for faster access to the config
        public static Config C => ModContent.GetInstance<Config>();
    }
}