namespace ChatPlus.Common.Compat.CustomTags;

/// <summary>
/// Represents a custom tag that can be registered with the CustomTagSystem.
/// </summary>
/// <param name="tag"></param> The tag code prefix used in chat, e.g. "t"
/// <param name="ActualTag"></param> The actual tag string, e.g. "[t:smile]"
public readonly record struct CustomTag(string tag, string ActualTag);

