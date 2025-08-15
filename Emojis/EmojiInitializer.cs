using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using AdvancedChatFeatures.Helpers;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace AdvancedChatFeatures.Emojis
{
    internal class EmojiInitializer : ModSystem
    {
        public static List<Emoji> Emojis { get; private set; } = new();
        public static Dictionary<string, List<string>> EmojiMap { get; private set; } = new(StringComparer.OrdinalIgnoreCase);

        public override void Load()
        {
            ChatManager.Register<EmojiTagHandler>(["e", "emoji"]);
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

                Log.Info($"Loaded {EmojiMap.Count} emoji entries from emojibase.raw.json");
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

            Log.Info("[start] Initializing emojis...");

            foreach (string file in Mod.GetFileNames())
            {
                if (!file.StartsWith("Assets/Emojis/", StringComparison.OrdinalIgnoreCase))
                    continue;

                int dot = file.LastIndexOf('.');
                if (dot < 0)
                    continue;

                // Accept rawimg/png/xnb
                string ext = file[(dot + 1)..].ToLowerInvariant();
                if (ext != "rawimg")
                    continue;

                string noExt = file[..dot];
                string codepoint = Path.GetFileNameWithoutExtension(file);

                // Resolve display name from map, fallback to codepoint
                string displayName = codepoint;
                if (EmojiMap.TryGetValue(codepoint, out var tags) && tags.Count > 0)
                    displayName = tags[0];

                // ModContent asset path must include mod name prefix and omit extension
                string texturePath = $"{Mod.Name}/{noExt}";

                bool registered = EmojiTagHandler.RegisterEmoji(displayName, texturePath);

                // Also register all synonyms to the same texture
                if (EmojiMap.TryGetValue(codepoint, out var synonyms))
                {
                    foreach (var syn in synonyms)
                        EmojiTagHandler.RegisterEmoji(syn, texturePath);
                }

                Emojis.Add(new Emoji
                {
                    FilePath = texturePath,
                    DisplayName = displayName,
                    Tag = EmojiTagHandler.GenerateTag(displayName) // [e:displayName]
                });
            }

            // Sort alphabetically by display name
            Emojis.Sort((a, b) => string.Compare(a.DisplayName, b.DisplayName, StringComparison.OrdinalIgnoreCase));

            Log.Info($"[end] Indexed {Emojis.Count} emojis.");
        }
    }
}
