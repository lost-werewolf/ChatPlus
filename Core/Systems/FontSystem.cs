using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Graphics;
using Terraria;
using Terraria.ModLoader;
using static ReLogic.Graphics.DynamicSpriteFont;

namespace ChatPlus.Core.Systems;

/// <summary>
/// Automatically initializes fonts in Assets/Fonts folder.
/// </summary>
public static class FontSystem
{
    // Fonts
    public static DynamicSpriteFont Bold;
    public static DynamicSpriteFont Italics;
    public static DynamicSpriteFont BoldShader;

    // Effects
    private static readonly Lazy<Asset<Effect>> _boldEffect = new(() =>
        ModContent.Request<Effect>("ChatPlus/Assets/Effects/BoldFont", AssetRequestMode.ImmediateLoad)
    );
    public static Effect BoldEffect => _boldEffect.Value.Value;

    public static bool Initialized { get; private set; }

    static FontSystem()
    {
        foreach (FieldInfo field in typeof(FontSystem).GetFields(BindingFlags.Static | BindingFlags.Public))
        {
            if (field.FieldType == typeof(DynamicSpriteFont))
            {
                string assetPath = $"ChatPlus/Assets/Fonts/{field.Name}";
                var asset = ModContent.Request<DynamicSpriteFont>(assetPath, AssetRequestMode.ImmediateLoad).Value;
                field.SetValue(null, asset);
            }
        }

        Initialized = true;
    }

    public static DynamicSpriteFont MakeBoldFont(DynamicSpriteFont font, float characterSpace = 1f, float lineSpace = 1f, float boldRadius = 1f, float kernalSize = 1f)
    {
        Effect effect = BoldEffect;
        var spriteCharactersField = typeof(DynamicSpriteFont).GetField("_spriteCharacters", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        var defaultCharacterDataField = typeof(DynamicSpriteFont).GetField("_defaultCharacterData", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        float spacing = font.CharacterSpacing;
        int lineSpacing = font.LineSpacing;
        var letterMap = (Dictionary<char, SpriteCharacterData>)spriteCharactersField.GetValue(font);
        var defaultLetter = (SpriteCharacterData)defaultCharacterDataField.GetValue(font);
        float newSpacing = spacing * characterSpace;
        int newLineSpace = (int)(lineSpacing * lineSpace);
        var newLetterMap = new Dictionary<char, SpriteCharacterData>(letterMap.Count);
        SpriteCharacterData newDefaultLetter = default;
        char newDefaultChar = (char)0;
        Dictionary<Texture2D, Texture2D> oldToNew = [];
        foreach (var letter in letterMap)
        {
            Texture2D oldImage = letter.Value.Texture;
            if (!oldToNew.TryGetValue(oldImage, out Texture2D newImage))
            {
                var theNewImage = new RenderTarget2D(Main.instance.GraphicsDevice, oldImage.Width, oldImage.Height);
                var oldTargets = Main.instance.GraphicsDevice.GetRenderTargets();
                Main.instance.GraphicsDevice.SetRenderTarget(theNewImage);
                Main.instance.GraphicsDevice.Clear(Color.Transparent);
                using (var drawer = new SpriteBatch(Main.instance.GraphicsDevice))
                {
                    drawer.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null);
                    effect.Parameters["imageWidth"].SetValue((float)oldImage.Width);
                    effect.Parameters["imageHeight"].SetValue((float)oldImage.Height);
                    effect.Parameters["boldRadius"].SetValue((float)boldRadius);
                    effect.CurrentTechnique.Passes[0].Apply();
                    drawer.Draw(oldImage, Microsoft.Xna.Framework.Vector2.Zero, Microsoft.Xna.Framework.Color.White);
                    drawer.End();
                }
                Main.instance.GraphicsDevice.SetRenderTargets(oldTargets);
                newImage = theNewImage;
                oldToNew.Add(oldImage, newImage);
            }
            var newGlyph = letter.Value.Glyph;
            newGlyph.X -= 1;
            newGlyph.Y -= 1;
            newGlyph.Width += 2;
            newGlyph.Height += 2;
            var newLetter = new DynamicSpriteFont.SpriteCharacterData(newImage, newGlyph, letter.Value.Padding, letter.Value.Kerning * kernalSize);
            newLetterMap.Add(letter.Key, newLetter);
            if (newDefaultChar == 0 && oldImage == defaultLetter.Texture) { newDefaultChar = letter.Key; newDefaultLetter = newLetter; }
        }
        var newFont = new DynamicSpriteFont(newSpacing, newLineSpace, newDefaultChar);
        defaultCharacterDataField.SetValue(newFont, newDefaultLetter);
        spriteCharactersField.SetValue(newFont, newLetterMap);
        return newFont;
    }
}
