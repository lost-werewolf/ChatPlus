using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;

namespace ChatPlus.Core.Helpers;
public static class ModHelper
{
    public static LocalMod GetLocalMod(Mod mod)
    {
        string modName = mod.Name;

        LocalMod[] mods = ModOrganizer.FindAllMods();
        LocalMod localMod = Array.Find(mods, m => m.Name.Equals(modName, StringComparison.OrdinalIgnoreCase));

        if (localMod == null) return null;

        return localMod;
    }

    public static string GetDisplayName(string name)
    {
        return ModLoader.TryGetMod(name, out var mod) ? (mod.DisplayName ?? name) : name;
    }
}
public static class ModMetaCache
{
    // name -> (size bytes, lastWriteUtc, tmodPath)
    private static readonly Dictionary<string, (long size, DateTime lastWriteUtc, string path)> _cache = new();

    public static (long size, DateTime lastWriteUtc, string path) Get(Mod mod)
    {
        if (mod == null) return default;
        if (_cache.TryGetValue(mod.Name, out var v)) return v;

        // Resolve once. DO NOT call any ModOrganizer/FindMods here.
        string path = TryGuessTmodPath(mod.Name);
        long size = 0;
        DateTime last = DateTime.MinValue;

        if (!string.IsNullOrEmpty(path) && File.Exists(path))
        {
            var fi = new FileInfo(path);
            size = fi.Length;
            last = fi.LastWriteTimeUtc;
        }

        v = (size, last, path);
        _cache[mod.Name] = v;
        return v;
    }

    private static string TryGuessTmodPath(string modName)
    {
        // Best-effort without poking ModOrganizer:
        // ModLoader.ModPath typically points to .../tModLoader/Mods
        // Sometimes it already includes "Mods"; handle both.
        var root = Terraria.ModLoader.ModLoader.ModPath; // public API
        // common locations to try
        var c1 = Path.Combine(root, modName + ".tmod");
        if (File.Exists(c1)) return c1;

        var c2 = Path.Combine(root, "Mods", modName + ".tmod");
        if (File.Exists(c2)) return c2;

        // Last resort: a one-time, narrow search (no global “find mods” call)
        var found = Directory.EnumerateFiles(root, modName + ".tmod", SearchOption.AllDirectories).FirstOrDefault();
        return found;
    }
}
