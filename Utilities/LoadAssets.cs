using System.Reflection;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;

namespace LinksInChat.Utilities
{
    /// <summary>
    /// To add a new asset, simply add a new field like:
    /// public static Asset<Texture2D> MyAsset;
    /// </summary>
    public class LoadAssets : ModSystem
    {
        public override void Load()
        {
            _ = Ass.Initialized;
        }
    }
    public static class Ass
    {
        // My textures
        public static Asset<Texture2D> ButtonModConfig;

        // Bool for checking if assets are loaded
        public static bool Initialized { get; set; }

        // Constructor
        static Ass()
        {
            foreach (FieldInfo field in typeof(Ass).GetFields())
            {
                if (field.FieldType == typeof(Asset<Texture2D>))
                {
                    field.SetValue(null, RequestAsset(field.Name));
                }
            }
        }

        private static Asset<Texture2D> RequestAsset(string path)
        {
            return ModContent.Request<Texture2D>($"LinksInChat/Assets/" + path, AssetRequestMode.AsyncLoad);
        }
    }
}
