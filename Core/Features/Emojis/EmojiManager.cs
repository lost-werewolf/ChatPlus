using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using ChatPlus.Common.Configs;
using ChatPlus.Core.Helpers;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace ChatPlus.Core.Features.Emojis
{
    internal class EmojiManager : ModSystem
    {
        public static List<Emoji> Emojis { get; private set; } = [];
        public static Dictionary<string, List<string>> EmojiMap { get; private set; } = new(StringComparer.OrdinalIgnoreCase);

        public override void Load()
        {
            if (!Conf.C.Emojis) return;

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

            int indexedCount = 0;
            int skippedCount = 0;

            Log.Info("[start] Initializing emojis...");

            foreach (string file in Mod.GetFileNames())
            {
                if (!file.StartsWith("Assets/EmojiBase/", StringComparison.OrdinalIgnoreCase))
                    continue;

                int dot = file.LastIndexOf('.');
                if (dot < 0)
                    continue;

                string ext = file[(dot + 1)..].ToLowerInvariant();
                if (ext != "rawimg")
                    continue;

                string noExt = file[..dot];
                string key = Path.GetFileNameWithoutExtension(file);

                if (!EmojiMap.TryGetValue(key, out var tags) || tags.Count == 0)
                {
                    skippedCount++;
                    continue;
                }

                string displayName = tags[0];
                string texturePath = $"{Mod.Name}/{noExt}";

                EmojiTagHandler.RegisterEmoji(displayName, texturePath);
                for (int i = 1; i < tags.Count; i++)
                {
                    EmojiTagHandler.RegisterEmoji(tags[i], texturePath);
                }


                Emojis.Add(new Emoji
                {
                    FilePath = texturePath,
                    Description = displayName,
                    Tag = EmojiTagHandler.GenerateEmojiTag(displayName),
                    Synonyms = new List<string>(tags)
                });


                indexedCount++;
            }

            Emojis.Sort((a, b) => string.Compare(a.Description, b.Description, StringComparison.OrdinalIgnoreCase));
            Log.Info($"[end] Indexed {indexedCount} emojis, skipped {skippedCount} files without a JSON mapping");
        }
    }
}
