using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using System;
using System.IO;
using System.Text;
using System.Text.Json;

namespace Steamer
{
    internal class LoadManager
    {
        public static DiscordConfiguration GetClientConfiguration()
        {
            return new DiscordConfiguration
            {
                AutoReconnect = true,
                Intents = DiscordIntents.All,
                Token = GetBotData().Token
            };
        }

        public static CommandsNextConfiguration GetCommandsConfiguration()
        {
            return new CommandsNextConfiguration
            {
                CaseSensitive = false,
                DmHelp = false,
                StringPrefixes = new string[] { GetBotData().Prefix }
            };
        }

        public static InteractivityConfiguration GetInteractivitysConfiguration()
        {
            return new InteractivityConfiguration
            {
                Timeout = TimeSpan.FromSeconds(60)
            };
        }

        private static Data GetBotData() =>
            JsonSerializer.Deserialize<Data>(File.ReadAllText("bot_configuration.json",
                Encoding.UTF8));
    }
}
