using Discord.Interactions;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBotTutorials.Handler
{
    [Group("Ping-commands", "All the bot ping commands")]
    public  class SlashCommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly InteractionService _command;
        private readonly IServiceProvider _service;

        public SlashCommandHandler(DiscordSocketClient client, InteractionService command, IServiceProvider service)
        {
            _client = client;
            _command = command;
            _service = service;
        }

        public async Task InitializeAsync()
        {
            await _command.AddModuleAsync(Type.GetType("DiscordBotTutorials.Module.SlashCommandModule"), _service);
            _client.SlashCommandExecuted += HandleSlashInteraction;
        }

        private async Task HandleSlashInteraction(SocketSlashCommand arg)
        {
            try
            {
                SocketInteractionContext context = new SocketInteractionContext(_client, arg);
                await _command.ExecuteCommandAsync(context, _service);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
