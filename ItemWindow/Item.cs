namespace AdvancedChatFeatures.ItemWindow
{
    /// <summary>
    /// Represents an item color for the item window and [i:ID] tag insertion.
    /// </summary>
    /// <param name="ID"> The id like </param>
    /// <param name="Tag">Chat tag like [i:ID] that renders the item icon</param>
    /// <param name="NoSpacesName"> Chat tag like [i:Zenith] that also renders the item</param>
    /// <param name="DisplayName">Item display name like "Zenith" </param>
    /// <param name="Tooltip">Item tooltip text (flat)</param>
    public readonly record struct Item(int ID, string Tag, string NoSpacesName, string DisplayName, string Tooltip);
}
