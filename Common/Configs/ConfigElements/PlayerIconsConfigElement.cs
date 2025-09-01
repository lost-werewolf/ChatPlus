using ChatPlus.Core.Features.PlayerHeads;
using ChatPlus.Core.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader.Config.UI;
using Terraria.UI;
using Terraria.UI.Chat;

namespace ChatPlus.Common.Configs.ConfigElements;

public class PlayerIconsConfigElement : ConfigElement<bool>
{
    private Asset<Texture2D> _toggleTexture = Asset<Texture2D>.Empty;

    // Called once when the config UI binds this element to your Width property
    public override void OnBind()
    {
        base.OnBind();

        _toggleTexture = Main.Assets.Request<Texture2D>("Images/UI/Settings_Toggle");

        // Toggle the value when clicked
        OnLeftClick += delegate
        {
            Value = !Value;

            Conf.C.PlayerIcons = Value;
        };
    }

    public override void OnInitialize()
    {
        base.OnInitialize();
    }

    public override void Draw(SpriteBatch sb)
    {
        base.Draw(sb);
        DrawPlayerIcon(sb);
        DrawToggleTexture(sb);
        DrawOnOffText(sb);
    }

    private void DrawPlayerIcon(SpriteBatch sb)
    {
        // Get pos
        var dims = GetDimensions();
        Vector2 pos = new(dims.X+162, dims.Y+12);

        // Get this player
        var player = Main.LocalPlayer;
        if (player == null)
        {
            // custom draw guide if no player was found (e.g player is in main menu).
            var guide = Ass.AuthorIcon.Value;
            Rectangle guideRect = new((int)pos.X, (int)pos.Y, 40, 40);
            sb.Draw(guide, guideRect, Color.White);
            return;
        }

        // Draw player head
        PlayerHeadFlipHook.shouldFlipHeadDraw = player.direction == -1;
        Main.MapPlayerRenderer.DrawPlayerHead(Main.Camera, player, pos, 1f, 0.75f, Color.White);
        PlayerHeadFlipHook.shouldFlipHeadDraw = false;
    }

    private void DrawToggleTexture(SpriteBatch sb)
    {
        // Draw toggle texture
        Rectangle sourceRectangle = new Rectangle(
            Value ? (_toggleTexture.Width() - 2) / 2 + 2 : 0,
            0,
            (_toggleTexture.Width() - 2) / 2,
            _toggleTexture.Height()
        );

        // Get the dimensions of the parent element
        CalculatedStyle dimensions = GetDimensions();

        // Calculate the position to draw the toggle texture
        sb.Draw(
            _toggleTexture.Value,
            new Vector2(
                dimensions.X + dimensions.Width - sourceRectangle.Width - 10f,
                dimensions.Y + 8f
            ),
            sourceRectangle,
            Color.White
        );
    }

    private void DrawOnOffText(SpriteBatch sb)
    {
        CalculatedStyle dimensions = GetDimensions();

        string label = Value ? "On" : "Off";

        // Shift to a position you know is visible
        Vector2 pos = new Vector2(dimensions.X + dimensions.Width - 60f, dimensions.Y + 8f);
        ChatManager.DrawColorCodedStringWithShadow(
            sb,
            FontAssets.ItemStack.Value,
            label,
            pos,
            Color.White,
            0f,
            Vector2.Zero,
            new Vector2(0.8f)
        );
    }

    // Called every frame while the in-game config UI is open
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
    }
}
