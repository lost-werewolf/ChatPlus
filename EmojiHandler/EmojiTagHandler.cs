using System;
using System.Collections.Generic;
using AdvancedChatFeatures.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace AdvancedChatFeatures.EmojiHandler
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
            string key = text ?? string.Empty;
            if (Registry.TryGetValue(key, out var texture))
            {
                return new EmojiSnippet(texture)
                {
                    Text = GenerateEmojiTag(key),
                    Color = Color.White
                };
            }
            else
            {
                // If no uploadedTexture is found, return the original text as a fallback
                return new TextSnippet(key);
            }
        }
        public static string GetPrefixTag() => "[e";
        public static string GenerateEmojiTag(string key) => $"[e:{key}]";
    }
}