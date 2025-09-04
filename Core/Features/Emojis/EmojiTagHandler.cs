using System;
using System.Collections.Generic;
using ChatPlus.Core.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI.Chat;
using static ChatPlus.Core.Features.Emojis.EmojiSnippet;

namespace ChatPlus.Core.Features.Emojis
{
    public class EmojiTagHandler : ITagHandler
    {
        /// <summary>
        /// A dictionary to hold emoji keys and their corresponding uploadedTexture assets.
        /// </summary>
        private static readonly Dictionary<string, Asset<Texture2D>> Registry = new(StringComparer.OrdinalIgnoreCase);

        public static bool RegisterEmoji(string key, string texturePath)
        {
            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(texturePath))
                return false;
            try
            {
                var asset = ModContent.Request<Texture2D>(texturePath, AssetRequestMode.AsyncLoad);
                Registry[key] = asset;
                return true;
            }
            catch
            {
                Log.Error($"Failed to find texturePath: '{texturePath}' for key: '{key}'");
                return false;
            }
        }

        TextSnippet ITagHandler.Parse(string text, Color baseColor, string options)
        {
            // 1) Normalize: extract base key and merge options from "text" if it contains "/..."
            string key = text;
            string mergedOptions = options;

            int slash = string.IsNullOrEmpty(text) ? -1 : text.IndexOf('/');
            if (slash >= 0)
            {
                string suffix = text.Substring(slash + 1);
                key = text.Substring(0, slash);

                if (string.IsNullOrEmpty(mergedOptions))
                {
                    mergedOptions = suffix;
                }
                else
                {
                    mergedOptions = mergedOptions + "," + suffix;
                }
            }

            // 2) Resolve the texture by base key
            if (!Registry.TryGetValue(key, out var texture))
            {
                return new TextSnippet(text);
            }

            // 3) Build a clean display tag without options for Text (stable re-emit)
            string cleanTag = GenerateEmojiTag(key);

            var snippet = new EmojiSnippet(texture, cleanTag)
            {
                Text = cleanTag,
                Color = Color.White,
                CheckForHover = true
            };

            // 4) Decide role based on merged options (accept several separators)
            snippet.Role = HasOption(mergedOptions, "button")
                ? EmojiSnippet.EmojiRenderRole.Button
                : EmojiSnippet.EmojiRenderRole.Normal;

            return snippet;


            static bool HasOption(string opts, string target)
            {
                if (string.IsNullOrEmpty(opts)) return false;

                var parts = opts.Split(new[] { '/', ',', ';', '|' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < parts.Length; i++)
                {
                    if (parts[i].Trim().Equals(target, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        // Optional convenience: emit with or without options
        public static string GenerateEmojiTag(string key)
        {
            return "[e:" + key + "]";
        }

        public static string GenerateEmojiTag(string key, string option)
        {
            if (string.IsNullOrWhiteSpace(option)) return "[e:" + key + "]";
            return "[e:" + key + "/" + option + "]";
        }
    }
}