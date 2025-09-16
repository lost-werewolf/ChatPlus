using ChatPlus.Core.Features.Stats.Base;
using ChatPlus.Core.Features.Uploads;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Terraria.UI; 

namespace ChatPlus.Core.Features.Stats.UploadStats;
public class UploadInfoState : BaseInfoState, ILoadable
{
    public static UploadInfoState Instance;

    private Upload? _current;
    public void Load(Mod mod) => Instance = this;
    public void Unload() => Instance = null;

    protected override void DrawContent(SpriteBatch sb, Rectangle inner)
    {
        if (_current == null || _current.Value.Texture == null)
            return;

        var tex = _current.Value.Texture;

        float scale = System.MathF.Min(
            inner.Width / (float)tex.Width,
            inner.Height / (float)tex.Height
        );

        int drawW = (int)(tex.Width * scale);
        int drawH = (int)(tex.Height * scale);

        var pos = new Vector2(
            inner.X + (inner.Width - drawW) / 2f,
            inner.Y + (inner.Height - drawH) / 2f
        );

        sb.Draw(tex, new Rectangle((int)pos.X, (int)pos.Y, drawW, drawH), Color.White);
    }

    public void Show(Upload upload)
    {
        _current = upload;
        SetTitle(upload.FileName);
        OpenForCurrentContext();
    }

    public void Close()
    {
        _current = null;
        CloseForCurrentContext();
    }
}
