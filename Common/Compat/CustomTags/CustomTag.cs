using Terraria.UI;

namespace ChatPlus.Common.Compat.CustomTags;

/// <summary>
/// 
/// </summary>
/// <param name="tag"></param> The main registered tag, e.g "t"
/// <param name="ActualTag"></param> The tag that actually affects chat, e.g "[t:1]" or "[t:2]"
/// <param name="DisplayElement"></param>
public readonly record struct CustomTag(string tag, string ActualTag, UIElement DisplayElement);
