using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordBotTutorials.Handler;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Configuration;

namespace DiscordBotTutorials // Note: actual namespace depends on the project name.
{
    public class Program
    {
        public static Task Main(string[] args) => new Program().MainAsync();

        public async Task MainAsync()
        {
            using IHost host = Host.CreateDefaultBuilder()
                .ConfigureServices((_, services) => services
                .AddSingleton(x => new DiscordSocketConfig()
                {
                    GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.GuildMessages,
                    AlwaysDownloadUsers = true,
                })
                .AddSingleton(x => new DiscordSocketClient(x.GetRequiredService<DiscordSocketConfig>()))
                .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>().Rest))
                .AddSingleton<SlashCommandHandler>())
                .Build();

            await RunAsync(host);
        }
        private async Task RunAsync(IHost host)
        {
            using IServiceScope serviceScope = host.Services.CreateScope();
            IServiceProvider serviceProvider = serviceScope.ServiceProvider;

            DiscordSocketClient client = serviceProvider.GetRequiredService<DiscordSocketClient>();
            InteractionService slashCommand = serviceProvider.GetRequiredService<InteractionService>();

            await serviceProvider.GetRequiredService<SlashCommandHandler>().InitializeAsync();

            client.Log += async (LogMessage msg) => { Console.WriteLine(msg.Message); };
            slashCommand.Log += async (LogMessage msg) => { Console.WriteLine(msg.Message); };

            client.Ready += async () =>
            {
                Console.WriteLine("Bot online and Ready!");
                await slashCommand.RegisterCommandsToGuildAsync(UInt64.Parse(ConfigurationManager.AppSettings["GuildId"]));
            };

            await client.LoginAsync(TokenType.Bot, ConfigurationManager.AppSettings["BotToken"]);
            await client.StartAsync();

            await Task.Delay(-1);
        }
    }
}