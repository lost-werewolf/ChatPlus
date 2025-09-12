using System.IO;
using System.Linq;
using ChatPlus.Core.Features.Colors;
using ChatPlus.Core.Features.Commands;
using ChatPlus.Core.Features.Emojis;
using ChatPlus.Core.Features.Glyphs;
using ChatPlus.Core.Features.Items;
using ChatPlus.Core.Features.Mentions;
using ChatPlus.Core.Features.ModIcons;
using ChatPlus.Core.Features.PlayerIcons;
using ChatPlus.Core.Features.Uploads;
using ChatPlus.Core.Helpers;
using ChatPlus.Core.Netcode;
using Terraria.ModLoader;

namespace ChatPlus;

public sealed class ChatPlus : Mod
{
    public static StateManager StateManager { get; private set; }

    public override void Load()
    {
        StateManager = new StateManager(
            ModContent.GetInstance<CommandSystem>(),
            ModContent.GetInstance<ColorSystem>(),
            ModContent.GetInstance<EmojiSystem>(),
            ModContent.GetInstance<GlyphSystem>(),
            ModContent.GetInstance<ItemSystem>(),
            ModContent.GetInstance<ModIconSystem>(),
            ModContent.GetInstance<MentionSystem>(),
            ModContent.GetInstance<PlayerIconSystem>(),
            ModContent.GetInstance<UploadSystem>()
        );
    }

    public override void Unload()
    {
        StateManager = null;
    }

    public override void HandlePacket(BinaryReader reader, int whoAmI)
    {
        ModNetHandler.HandlePacket(reader, whoAmI);
    }

    public override void PostSetupContent()
    {
        if (File == null) return;

        var names = File.GetFileNames();
        var entries = names
            .Select(n => new { Name = n, Bytes = File.GetBytes(n) })
            .OrderByDescending(e => e.Bytes?.Length ?? 0)
            .ToList();

        // Added aggregate stats
        if (entries.Count == 0)
        {
            Log.Info("[ModSize] No files found in mod.");
            return;
        }

        long totalBytes = entries.Sum(e => (long)(e.Bytes?.Length ?? 0));
        int totalFiles = entries.Count;
        double totalMB = totalBytes / 1024f / 1024f;
        double avgBytes = totalFiles > 0 ? (double)totalBytes / totalFiles : 0;
        double avgKB = avgBytes / 1024.0;
        var sizeArray = entries.Select(e => (long)(e.Bytes?.Length ?? 0)).OrderBy(x => x).ToArray();
        double medianBytes = sizeArray.Length % 2 == 1
            ? sizeArray[sizeArray.Length / 2]
            : (sizeArray[sizeArray.Length / 2 - 1] + sizeArray[sizeArray.Length / 2]) / 2.0;
        double medianKB = medianBytes / 1024.0;
        int uniqueExtCount = entries.Select(e => System.IO.Path.GetExtension(e.Name).ToLowerInvariant()).Distinct().Count();

        Log.Info($"[ModSize] Total files: {totalFiles}");
        Log.Info($"[ModSize] Total size : {totalMB:0.00} MB");
        Log.Info($"[ModSize] Avg size   : {avgKB:0.0} KB   Median: {medianKB:0.0} KB");
        Log.Info($"[ModSize] Unique extensions: {uniqueExtCount}");
        var largest = entries[0];
        Log.Info($"[ModSize] Largest file: {largest.Name} ({largest.Bytes.Length / 1024f / 1024f:0.00} MB)");

        Log.Info("[ModSize] Top 10 biggest files inside mod:");
        foreach (var e in entries.Take(10))
            Log.Info($"[ModSize] {e.Bytes.Length / 1024f / 1024f:0.00} MB  {e.Name}");

        var byExt = entries.GroupBy(e => System.IO.Path.GetExtension(e.Name).ToLowerInvariant())
            .Select(g => new { Ext = g.Key, MB = g.Sum(x => x.Bytes?.Length ?? 0) / 1024f / 1024f })
            .OrderByDescending(x => x.MB);

        Log.Info("[ModSize] Totals by extension:");
        foreach (var g in byExt)
            Log.Info($"[ModSize] {g.Ext,-6} {g.MB:0.00} MB");
    }

}