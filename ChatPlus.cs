using System;
using System.Collections.Generic;
using System.IO;
using ChatPlus.Common.Compat.CustomTags;
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
using ChatPlus.Core.UI;
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
            ModContent.GetInstance<UploadSystem>(),
            ModContent.GetInstance<CustomTagSystem>()
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

    /// <summary>
    /// A custom call method for inter-mod communication.
    /// </summary>
    public override object Call(params object[] args)
    {
        // First, check that we have at least one argument and that it's a string command.
        if (args.Length == 0 || args[0] is not string command)
            throw new ArgumentException("Error with ChatPlus Mod Call: First argument must be a command string.");

        if (args.Length != 3)
            throw new ArgumentException("Error with ChatPlus Mod Call: Expected exactly 3 arguments: RegisterTag, .");

        // Handle the "RegisterTag" command.
        if (string.Equals(command, "RegisterTag"))
        {
            // Get the second argument as a string tag, e.g [example:smile] means that tag = "example"
            if (args[1] is not string tag)
                throw new ArgumentException("Error with ChatPlus Mod Call: Second argument must be a string representing the tag.");

            // Get the third argument as a List<string> to populate with tags, e.g ["[e:example]"]
            if (args[2] is not IEnumerable<string> tagsToAdd)
                throw new ArgumentException("Error with ChatPlus Mod Call: Third argument must be a List<string> to populate with tags.");

            foreach (var tagToAdd in tagsToAdd)
            {
                Log.Info($"Registering custom tag: {tagToAdd} with prefix: {tag}");
                CustomTagSystem.CustomTags.Add(new CustomTag(tag, tagToAdd));
            }

            // Ensure a state + panel exists for this prefix
            if (!CustomTagSystem.States.ContainsKey(tag))
            {
                CustomTagSystem.States[tag] = new CustomTagState(tag);
                Log.Info($"Created CustomTagState for prefix [{tag}:...]");
            }
        }

        return base.Call(args);
    }
}