using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ChatPlus.Core.Features.PlayerIcons;
using Terraria;
using Terraria.Chat;
using Terraria.Chat.Commands;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace ChatPlus.Common.ModCommands
{
    public class PlayingOverrideSystem : ModSystem
    {
        public override void PostSetupContent()
        {
            // grab the private _commands field
            var field = typeof(ChatCommandProcessor).GetField("_commands", BindingFlags.NonPublic | BindingFlags.Instance);
            if (field == null)
                return;

            var dict = (Dictionary<ChatCommandId, IChatCommand>)field.GetValue(ChatManager.Commands);

            // swap vanilla ListPlayersCommand with our own
            var id = ChatCommandId.FromType<ListPlayersCommand>();
            dict[id] = new PlayingOverrideCommand();
        }
    }

    public class PlayingOverrideCommand : IChatCommand
    {
        public void ProcessIncomingMessage(string text, byte clientId)
        {
            Main.NewText(" [c/fff014:Players:]");

            var players = Main.player
    .Where(p => p != null && p.active)
    .Select(p =>
    {
        string tag = PlayerIconTagHandler.GenerateTag(p.name);
        return $"{tag} {p.name}"; // space before tag, space after
    });

            string message = string.Join(",", players);

            if (string.IsNullOrEmpty(message))
            {
                message = "No players online.";
            }

            Main.NewText(message, Color.LightGreen);
        }

        public void ProcessOutgoingMessage(ChatMessage message)
        {
            // no-op
        }
    }
}
