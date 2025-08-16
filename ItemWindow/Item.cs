namespace AdvancedChatFeatures.ItemWindow
{
    /// <summary>
    /// Represents an item Entry for the item window and [i:ID] tag insertion.
    /// </summary>
    /// <param name="Tag">Chat tag like [i:ID] that renders the item icon</param>
    /// <param name="Name">Item display name</param>
    /// <param name="Tooltip">Item tooltip text (flat)</param>
    public readonly record struct Item(string Tag, string Name, string Tooltip);
}
