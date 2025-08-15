using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using AdvancedChatFeatures.Helpers;
using Terraria.GameContent.UI.Chat;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace AdvancedChatFeatures.UI.Emojis
{
    internal class EmojiInitializer : ModSystem
    {
        public static List<Emoji> Emojis { get; private set; } = new();
        public static Dictionary<string, List<string>> EmojiMap { get; private set; } = new(StringComparer.OrdinalIgnoreCase);

        private static bool registered;
        public override void Load()
        {
            // Register tag handler once
            if (!registered)
            {
                ChatManager.Register<EmojiTagHandler>(["e", "emoji"]);
                registered = true;
            }
            EmojiTagHandler.ClearRegistry();
        }

        public override void PostSetupContent()
        {
            InitializeEmojiMap();
            InitializeEmojis();
        }

        private void InitializeEmojiMap()
        {
            EmojiMap.Clear();

            try
            {
                byte[] bytes = Mod.GetFileBytes("emojibase.raw.json");
                string jsonString = Encoding.UTF8.GetString(bytes);

                var rawMap = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(jsonString);

                foreach (var kv in rawMap)
                {
                    var list = new List<string>();

                    if (kv.Value.ValueKind == JsonValueKind.String)
                    {
                        list.Add(kv.Value.GetString() ?? "");
                    }
                    else if (kv.Value.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var item in kv.Value.EnumerateArray())
                        {
                            if (item.ValueKind == JsonValueKind.String)
                                list.Add(item.GetString() ?? "");
                        }
                    }

                    if (list.Count > 0)
                        EmojiMap[kv.Key] = list;
                }

                Log.Info($"[EmojiInitializer] Loaded {EmojiMap.Count} emoji entries from emojibase.raw.json");
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to read emojibase.raw.json: {ex}");
                EmojiMap = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
            }
        }

        private void InitializeEmojis()
        {
            Emojis.Clear();
            string modName = Mod.Name;

            foreach (string file in Mod.GetFileNames())
            {
                if (!file.StartsWith("Assets/Emojis/", StringComparison.OrdinalIgnoreCase))
                    continue;

                int dot = file.LastIndexOf('.');
                if (dot < 0)
                    continue;

                string ext = file[(dot + 1)..].ToLowerInvariant();
                if (ext != "rawimg")
                    continue;

                string noExt = file[..dot]; // without extension
                string codepoint = noExt["Assets/Emojis/".Length..]; // e.g. "1F469-1F3FB-200D-1F9AF"

                // Find display name from EmojiMap, fall back to codepoint
                string displayName = codepoint;
                if (EmojiMap.TryGetValue(codepoint, out var tags) && tags.Count > 0)
                    displayName = tags[0]; // take first tag as the name

                Emojis.Add(new Emoji
                {
                    FilePath = $"{modName}/{noExt}",
                    DisplayName = displayName,
                    Tag = EmojiTagHandler.GenerateTag(displayName)
                });
            }

            // Sort alphabetically by display name
            Emojis.Sort((a, b) => string.Compare(a.DisplayName, b.DisplayName, StringComparison.OrdinalIgnoreCase));

            Log.Info($"Indexed {Emojis.Count} emojis.");
        }
    }
}
