using System;
using System.Diagnostics;
using AdvancedChatFeatures.Common.Configs;
using AdvancedChatFeatures.Helpers;
using Microsoft.Xna.Framework;              // Vector2, Color
using Microsoft.Xna.Framework.Graphics;     // Texture2D, SpriteBatch
using ReLogic.Content;                      // Asset, AssetRequestMode
using Terraria;
using Terraria.GameContent;                 // TextureAssets (fallback)
using Terraria.ModLoader;                   // Mod, ModContent
using Terraria.UI.Chat;                     // TextSnippet

public sealed class ModIconSnippet : TextSnippet
{
    private Texture2D _icon;
    private readonly int _size;
    private readonly Mod mod;

    /// <param name="mod">The mod whose icon to draw. Pass null to show a generic fallback.</param>
    /// <param name="sizePx">Target size (pixels) at scale==1.</param>
    public ModIconSnippet(Mod mod, int sizePx = 18) : base("\u200B") // zero-width placeholder text
    {
        Log.Info("new iconsnippet for " + mod.Name);

        this.mod = mod;
        _size = Math.Max(12, sizePx);
        Color = Color.White;

        // Try to load "<ModName>/icon" from the mod; fall back to an inventory slot texture.
        try
        {
            if (mod != null)
            {
                var asset = ModContent.Request<Texture2D>($"{mod.Name}/icon", AssetRequestMode.ImmediateLoad);
                _icon = asset?.Value;
            }
        }
        catch
        {
            // ignore – we'll fall back below
        }

        _icon ??= TextureAssets.InventoryBack.Value;
    }

    public override bool UniqueDraw(bool justCheckingString, out Vector2 size, SpriteBatch spriteBatch, Vector2 position = default, Color color = default, float scale = 1f)
    {
        var asset = ModContent.Request<Texture2D>($"{mod.Name}/icon_small", AssetRequestMode.ImmediateLoad);
        if (asset == null)
        {
            Log.Info(mod.Name + "icon small null");
        }
        else
        {

        }
        _icon = asset?.Value;

        size = new Vector2(_size) * scale; // how much horizontal space this snippet occupies

        if (justCheckingString)
            return true; // tell the layout system our measured size and that we handled drawing

        //if (mod == ModLoader.GetMod("AdvancedChatFeatures"))
            //return true;

        // draw the icon
        if (_icon != null)
        {
            // Uniformly scale so the icon fits _size x _size
            float drawScale = scale * (_size / (float)Math.Max(_icon.Width, _icon.Height));

            position += new Vector2(-10, 0);

            if (Conf.C.styleConfig.ShowPlayerIcons)
                position += new Vector2(-26, 0);
            
            spriteBatch.Draw(_icon, position, null, Color.White, 0f, Vector2.Zero, drawScale, SpriteEffects.None, 0f);
        }

        return true; // we handled drawing
    }


        public static Mod GetCallingMod()
    {
        Mod mod = null;
        string name = string.Empty;

        StackFrame[] frames;
        try
        {
            frames = new StackTrace(true).GetFrames();
            Logging.PrettifyStackTraceSources(frames);
            int index = 2;
            while (index < frames.Length && frames[index].GetMethod().Name.Contains("NewText"))
                index++;
            if (index == frames.Length)
                name = string.Empty;
            else
            {
                var frame = frames[index];
                var method = frame.GetMethod();

                Type declaringType = method.DeclaringType;
                if (declaringType != null)
                {
                    name = declaringType.Namespace;
                    name = name.Split('.')[0];
                }
                else
                {
                    name = "Terraria";
                }

                if (name != "Terraria")
                {
                    {
                        mod = ModLoader.GetMod(name);
                        mod ??= ModLoader.GetMod(name + "Mod");
                    }
                }
            }
        }
        catch
        {
            Log.Info("guh");
        }
        return mod;
    }
}
