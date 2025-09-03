using ChatPlus.Core.Features.Uploads;

namespace ChatPlus.Core.Features.Uploads.UploadInfo;

/// <summary>
/// Global static storage for hovered upload so we can draw its preview at the very top UI layer.
/// </summary>
public static class HoveredUploadOverlay
{
    private static Upload? _hovered;

    public static void Set(Upload upload)
    {
        _hovered = upload;
    }

    internal static Upload? Consume()
    {
        var u = _hovered;
        _hovered = null;
        return u;
    }
}
