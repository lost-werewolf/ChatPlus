using System.Collections.Generic;
using AdvancedChatFeatures.Helpers;
using Terraria.GameContent.UI.Chat;
using Terraria.ModLoader;

namespace AdvancedChatFeatures.UI.Emojis
{
    public static class EmojiInitializer
    {
        public static List<Emoji> Emojis { get; private set; } = [];
    }

    internal class EmojiInitializerSystem : ModSystem
    {
        public override void PostSetupContent()
        {
            EmojiInitializer.Emojis.Clear();

            for (int i = 0; i < 26; i++)
            {
                string tag = GlyphTagHandler.GenerateTag(i);
                string name = $":g{i}";
                EmojiInitializer.Emojis.Add(new Emoji(name, tag));
                Log.Info($"(Glyph #{i} initialized: ){name} - {tag}");
            }
        }
    }
}
