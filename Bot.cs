using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using System;
using System.Threading.Tasks;

namespace Steamer
{
    internal class Bot
    {
        public DiscordClient Client { get; }
        public CommandsNextExtension Commands { get; }

        internal Bot()
        {
            Client = new DiscordClient(LoadManager.GetClientConfiguration());
            Commands = Client.UseCommandsNext(LoadManager.GetCommandsConfiguration());

            Client.UseInteractivity(new InteractivityConfiguration { Timeout = TimeSpan.FromSeconds(60) });

            Commands.RegisterCommands<Commands>();
            Commands.CommandErrored += ErrorHanlder.CmdErroredHandler;
        }

        public async Task RunAsync()
        {
            await Client.ConnectAsync(new DiscordActivity { Name = "Surfing the Net" });
            await Task.Delay(-1);
        }
    }
}
