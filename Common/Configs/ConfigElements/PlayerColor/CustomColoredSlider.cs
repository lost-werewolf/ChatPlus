using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.Localization;
using Terraria.UI;

namespace ChatPlus.Common.Configs.ConfigElements.PlayerColor;
public class CustomColoredSlider : UISliderBase
{
    public Color _color;

    public LocalizedText _textKey;

    public Func<float> _getStatusTextAct;

    public Action<float> _slideKeyboardAction;

    public Func<float, Color> _blipFunc;

    public Action _slideGamepadAction;

    public const bool BOTHER_WITH_TEXT = false;

    public bool _isReallyMouseOvered;

    public bool _alreadyHovered;

    public bool _soundedUsage;

    public CustomColoredSlider(LocalizedText textKey, Func<float> getStatus, Action<float> setStatusKeyboard, Action setStatusGamepad, Func<float, Color> blipColorFunction, Color color)
    {
        _color = color;
        _textKey = textKey;
        _getStatusTextAct = ((getStatus != null) ? getStatus : ((Func<float>)(() => 0f)));
        _slideKeyboardAction = ((setStatusKeyboard != null) ? setStatusKeyboard : ((Action<float>)delegate
        {
        }));
        _blipFunc = ((blipColorFunction != null) ? blipColorFunction : ((Func<float, Color>)((float s) => Color.Lerp(Color.Black, Color.White, s))));
        _slideGamepadAction = setStatusGamepad;
        _isReallyMouseOvered = false;
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        UISliderBase.CurrentAimedSlider = null;
        if (!Main.mouseLeft) UISliderBase.CurrentLockedSlider = null;

        int usageLevel = GetUsageLevel();
        base.DrawSelf(spriteBatch);

        CalculatedStyle dims = GetDimensions();

        // Fill the entire element width (minus a tiny margin) by scaling the color bar texture
        float leftPadding = 340f;
        float rightPadding = 15f;
        float topPadding = 10f;

        Texture2D barTex = TextureAssets.ColorBar.Value;
        float availableWidth = Math.Max(1f, dims.Width - leftPadding - rightPadding);
        float drawScale = availableWidth / barTex.Width * 1f;

        // Right-anchored as expected by DrawValueBar (it subtracts the scaled width internally)
        Vector2 barRightAnchor = new Vector2(dims.X + dims.Width - rightPadding, dims.Y + topPadding + 8f);

        bool wasInBar;
        float sliderValue = DrawValueBar(spriteBatch, barRightAnchor, drawScale, _getStatusTextAct(), usageLevel, out wasInBar, _blipFunc);

        if (UISliderBase.CurrentLockedSlider == this || wasInBar)
        {
            UISliderBase.CurrentAimedSlider = this;
            if (PlayerInput.Triggers.Current.MouseLeft && !PlayerInput.UsingGamepad && UISliderBase.CurrentLockedSlider == this)
            {
                _slideKeyboardAction(sliderValue);
                if (!_soundedUsage) SoundEngine.PlaySound(12);
                _soundedUsage = true;
            }
            else
            {
                _soundedUsage = false;
            }
        }

        if (UISliderBase.CurrentAimedSlider != null && UISliderBase.CurrentLockedSlider == null)
            UISliderBase.CurrentLockedSlider = UISliderBase.CurrentAimedSlider;

        if (_isReallyMouseOvered && _slideGamepadAction != null)
            _slideGamepadAction();
    }

    public float DrawValueBar(SpriteBatch sb, Vector2 drawPosition, float drawScale, float sliderPosition, int lockMode, out bool wasInBar, Func<float, Color> blipColorFunc)
    {
        Texture2D value = TextureAssets.ColorBar.Value;
        Vector2 vector = new Vector2(value.Width, value.Height) * drawScale;
        drawPosition.X -= (int)vector.X;
        Rectangle rectangle = new Rectangle((int)drawPosition.X, (int)drawPosition.Y - (int)vector.Y / 2, (int)vector.X, (int)vector.Y);
        Rectangle destinationRectangle = rectangle;
        sb.Draw(value, rectangle, Color.White);
        float num = (float)rectangle.X + 5f * drawScale;
        float num2 = (float)rectangle.Y + 4f * drawScale;
        for (float num3 = 0f; num3 < 167f; num3 += 1f)
        {
            float arg = num3 / 167f;
            Color color = blipColorFunc(arg);
            sb.Draw(TextureAssets.ColorBlip.Value, new Vector2(num + num3 * drawScale, num2), null, color, 0f, Vector2.Zero, drawScale, SpriteEffects.None, 0f);
        }

        rectangle.X = (int)num - 2;
        rectangle.Y = (int)num2;
        rectangle.Width -= 4;
        rectangle.Height -= 8;
        bool flag = (_isReallyMouseOvered = rectangle.Contains(new Point(Main.mouseX, Main.mouseY)));
        if (IgnoresMouseInteraction)
        {
            flag = false;
        }

        if (lockMode == 2)
        {
            flag = false;
        }

        if (flag || lockMode == 1)
        {
            sb.Draw(TextureAssets.ColorHighlight.Value, destinationRectangle, Main.OurFavoriteColor);
            if (!_alreadyHovered)
            {
                SoundEngine.PlaySound(12);
            }

            _alreadyHovered = true;
        }
        else
        {
            _alreadyHovered = false;
        }

        wasInBar = false;
        if (!IgnoresMouseInteraction)
        {
            sb.Draw(TextureAssets.ColorSlider.Value, new Vector2(num + 167f * drawScale * sliderPosition, num2 + 4f * drawScale), null, Color.White, 0f, new Vector2(0.5f * (float)TextureAssets.ColorSlider.Value.Width, 0.5f * (float)TextureAssets.ColorSlider.Value.Height), drawScale, SpriteEffects.None, 0f);
            if (Main.mouseX >= rectangle.X && Main.mouseX <= rectangle.X + rectangle.Width)
            {
                wasInBar = flag;
                return (float)(Main.mouseX - rectangle.X) / (float)rectangle.Width;
            }
        }

        if (rectangle.X >= Main.mouseX)
        {
            return 0f;
        }

        return 1f;
    }
}

