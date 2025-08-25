namespace ChatPlus.ModIconHandler;

/// <summary>
/// Represents a mod icon reference that can be inserted into chat.
/// </summary>
/// <param name="Tag">e.g. "[mi:ModInternalName]"</param>
/// <param name="InternalName">The mod's internal name (assembly / folder name)</param>
/// <param name="DisplayName">The user facing display name</param>
public readonly record struct ModIcon(string Tag, string InternalName, string DisplayName);
