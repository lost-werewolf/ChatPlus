using System;
using System.Globalization;
using ChatPlus.Common.Configs.ConfigElements.PlayerColor;
using ChatPlus.Core.Features.PlayerColors;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.OS;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.ModLoader.Config.UI;
using Terraria.ModLoader.UI;
using Terraria.UI;
using Terraria.UI.Chat;
using static Terraria.GameContent.UI.States.UICharacterCreation;

namespace ChatPlus.Common.Configs.ConfigElements;

public class PlayerColorConfigElement : ConfigElement<string>
{
    private Vector3 hsl;
    private string lastHex = "FFFFFF";

    private UIColoredImageButton copyButton;
    private UIColoredImageButton pasteButton;
    private UIColoredImageButton randomizeButton;

    private CustomColoredSlider hueSlider;
    private CustomColoredSlider saturationSlider;
    private CustomColoredSlider luminanceSlider;

    public override void OnBind()
    {
        base.OnBind();
        var hex = NormalizeHex(Value);
        if (!TryParseHex(hex, out var c)) c = Color.White;
        hsl = Main.rgbToHsl(c);
        lastHex = PlayerColorHandler.ColorToHex(c);
        Value = lastHex;

        MinHeight.Set(100, 0);
        Height.Set(100, 0);
    }

    public override void OnInitialize()
    {
        MinHeight.Set(100, 0);
        Height.Set(100, 0);

        // Make buttons
        copyButton = MakeButton("Copy", 0, Copy);
        pasteButton = MakeButton("Paste", 36, Paste);
        randomizeButton = MakeButton("Randomize", 36*2, Randomize);

        // Make sliders
        hueSlider = MakeHslSlider(HSLSliderId.Hue, 0);
        saturationSlider = MakeHslSlider(HSLSliderId.Saturation, 30);
        luminanceSlider = MakeHslSlider(HSLSliderId.Luminance, 60);
    }

    private void Copy()
    {
        var clip = Platform.Get<IClipboard>(); 
        if (clip != null) 
            clip.Value = "#" + CurrentHex(); 
    }

    private void Paste()
    {
        var s = Platform.Get<IClipboard>()?.Value ?? string.Empty; 
        ApplyHex(NormalizeHex(s)); 
    }

    private void Randomize()
    {
        const float SAT_MIN = 0.65f;   // vivid
        const float SAT_MAX = 0.95f;
        const float L_MIN = 0.42f;   // avoid too dark
        const float L_MAX = 0.62f;   // avoid too bright
        const byte RGB_MIN = 20;      // clamp after HSL->RGB
        const byte RGB_MAX = 235;

        // try a few times to find something that passes the RGB sanity check
        for (int attempt = 0; attempt < 6; attempt++)
        {
            float h = Main.rand.NextFloat(); // any hue

            // bias saturation toward the higher end (vivid): ease-out curve
            float sT = Main.rand.NextFloat();
            float s = MathHelper.Lerp(SAT_MIN, SAT_MAX, 1f - (float)System.Math.Pow(1f - sT, 2f));

            // lightness centered in the middle: triangular distribution
            float lT = 0.5f * (Main.rand.NextFloat() + Main.rand.NextFloat());
            float l = MathHelper.Lerp(L_MIN, L_MAX, lT);

            // check the final RGB isn't too dark/bright
            Color c = Main.hslToRgb(h, s, l);
            byte lo = (byte)System.Math.Min(c.R, System.Math.Min(c.G, c.B));
            byte hi = (byte)System.Math.Max(c.R, System.Math.Max(c.G, c.B));

            hsl = new Vector3(h, s, l);
            if (lo >= RGB_MIN && hi <= RGB_MAX || attempt == 5)
                break; // accept; last attempt falls back to whatever we got
        }

        PushFromHsl();
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        var hex = NormalizeHex(Value);
        if (hex != lastHex && TryParseHex(hex, out var c)) {
            hsl = Main.rgbToHsl(c);
            lastHex = hex; 
        }
    }

    private CustomColoredImageButton MakeButton(string assetName, float left, Action onClick)
    {
        var btn = new CustomColoredImageButton(
            texture: Main.Assets.Request<Texture2D>($"Images/UI/CharCreation/{assetName}"),
            tooltip: assetName);
        btn.OnLeftMouseDown += (_, _) => onClick();
        btn.Top.Set(34, 0);
        btn.Left.Set(155 + left, 0);
        Append(btn);
        return btn;
    }

    public override void Draw(SpriteBatch sb)
    {
        base.Draw(sb);
        var dims = GetDimensions();
        Rectangle area = new((int)dims.X, (int)dims.Y, (int)dims.Width, (int)dims.Height);
        Color color = Main.hslToRgb(hsl);

        // Draw player name
        string name = "PlayerName";
        if (Main.LocalPlayer != null) 
            name = Main.LocalPlayer.name;
        Vector2 playerNamePos = new(area.X + 158, area.Y+72);
        ChatManager.DrawColorCodedStringWithShadow(sb, FontAssets.MouseText.Value, name, playerNamePos, color, 0f, Vector2.Zero, baseScale: new Vector2(0.8f));
        
        // Draw preview box
        //var boxSize = 30;
        //var box = new Rectangle((int)area.X+430, area.Y, boxSize, boxSize);
        //Color c = Color.Black; //box outline color
        //sb.Draw(TextureAssets.MagicPixel.Value, box, color);
        //sb.Draw(TextureAssets.MagicPixel.Value, new Rectangle(box.X, box.Y, box.Width, 1), c);
        //sb.Draw(TextureAssets.MagicPixel.Value, new Rectangle(box.X, box.Bottom - 1, box.Width, 1), c);
        //sb.Draw(TextureAssets.MagicPixel.Value, new Rectangle(box.X, box.Y, 1, box.Height), c);
        //sb.Draw(TextureAssets.MagicPixel.Value, new Rectangle(box.Right - 1, box.Y, 1, box.Height), c);

        // Draw hex text
        string hexText = "#" + CurrentHex();
        Vector2 hexTextPos = new(area.X+155, area.Y+6);
        ChatManager.DrawColorCodedStringWithShadow(sb, FontAssets.MouseText.Value, hexText, hexTextPos, Color.White, 0f, Vector2.Zero, new Vector2(1.0f));
    }

    private CustomColoredSlider MakeHslSlider(HSLSliderId id, int top)
    {
        Func<float> get = () => id == HSLSliderId.Hue ? hsl.X : id == HSLSliderId.Saturation ? hsl.Y : hsl.Z;
        Action<float> set = v => { if (id == HSLSliderId.Hue) hsl.X = v; else if (id == HSLSliderId.Saturation) hsl.Y = v; else hsl.Z = v; PushFromHsl(); };
        Func<float, Color> grad = x => id == HSLSliderId.Hue ? Main.hslToRgb(x, 1, 0.5f) : id == HSLSliderId.Saturation ? Main.hslToRgb(hsl.X, x, hsl.Z) : Main.hslToRgb(hsl.X, hsl.Y, x);
        CustomColoredSlider slider = new(LocalizedText.Empty, get, set, () => { }, grad, Color.Transparent) 
        { 
            Left = StyleDimension.FromPixels(5), 
            Top = StyleDimension.FromPixels(top),
            Width = StyleDimension.FromPercent(1)
        };
        Append(slider);
        return slider;
    }

    private void PushFromHsl()
    {
        var c = Main.hslToRgb(hsl);
        var hex = PlayerColorHandler.ColorToHex(c);
        if (hex != lastHex) { lastHex = hex; Value = hex; }
    }

    private void ApplyHex(string hex)
    {
        if (!TryParseHex(hex, out var c)) return;
        hsl = Main.rgbToHsl(c);
        var fixedHex = PlayerColorHandler.ColorToHex(c);
        lastHex = fixedHex;
        Value = fixedHex;
    }

    private static string NormalizeHex(string s)
    {
        if (string.IsNullOrWhiteSpace(s)) return "FFFFFF";
        s = s.Trim();
        if (s.StartsWith("#", StringComparison.Ordinal)) s = s.Substring(1);
        if (s.Length != 6) return "FFFFFF";
        return s.ToUpperInvariant();
    }

    private static bool TryParseHex(string s, out Color c)
    {
        c = Color.White;
        if (string.IsNullOrWhiteSpace(s) || s.Length != 6) return false;
        if (!int.TryParse(s.Substring(0, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var r)) return false;
        if (!int.TryParse(s.Substring(2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var g)) return false;
        if (!int.TryParse(s.Substring(4, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var b)) return false;
        c = new Color(r, g, b);
        return true;
    }

    private string CurrentHex() => PlayerColorHandler.ColorToHex(Main.hslToRgb(hsl));
}
