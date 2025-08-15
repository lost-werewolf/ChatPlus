namespace AdvancedChatFeatures.UI.Emojis
{
    /// <summary>
    /// An emoji that can be sent and rendered in chat.
    /// </summary>
    /// <param name="FilePath">e.g "/AdvancedChatFeatures/Assets/Emojis/12345.png"</param> 
    /// <param name="DisplayName"> e.g "happy_face"</param>
    /// <param name="Tag">e.g "[e:happy_face]</param>
    public readonly record struct Emoji(string FilePath, string DisplayName, string Tag);
}
