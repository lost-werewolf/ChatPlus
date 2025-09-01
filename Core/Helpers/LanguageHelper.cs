namespace ChatPlus.Core.Helpers;

public static class Loc
{
    public static string Get(string key, params object[] args)
    {
        if (Terraria.Localization.Language.Exists($"Mods.ChatPlus.{key}"))
        {
            return Terraria.Localization.Language.GetTextValue($"Mods.ChatPlus.{key}", args);
        }
        else
        {
            key = key.Replace("Mods.ChatPlus", string.Empty);
            return key;
        }
    }
}
