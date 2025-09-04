namespace ChatPlus.Core.Features.Mentions;

/// <summary>
/// Represents a mention entry that can be inserted into chat as a tag.
/// </summary>
/// <param name="Tag"></param>
public readonly record struct Mention(string Tag);
