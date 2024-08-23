using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using CSharpFunctionalExtensions;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Yuki.Services;

namespace Yuki {
    public class Yuki {
        public static readonly IServiceProvider Services = ConfigureServices();
        public static readonly DiscordSocketClient Client = Services.GetRequiredService<DiscordSocketClient>();
        public static readonly CommandService Commands = Services.GetRequiredService<CommandService>();
        public static readonly InteractionService Interactions = Services.GetRequiredService<InteractionService>();
        public static readonly LoggingService Log = Services.GetRequiredService<LoggingService>();
        public static readonly Config Config = Config.FromFile("data/config.toml");

        private static IServiceProvider ConfigureServices() {
            var discordConfig = new DiscordConfig() {

            };

            var interactionConfig = new InteractionServiceConfig() {
                AutoServiceScopes = true
            };  
            

            var collection = new ServiceCollection()
                    .AddSingleton(discordConfig)
                    .AddSingleton(interactionConfig)
                    .AddSingleton<DiscordSocketClient>()
                    .AddSingleton<CommandService>()
                    .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>(), interactionConfig    ))
                    .AddSingleton<LoggingService>();

            return collection.BuildServiceProvider();
        }

        public async Task RunYukiAsync() {
            Client.Ready += ClientReady;

            if (Config.Token != null) {
                await Client.LoginAsync(TokenType.Bot, Config.Token);
                await Client.StartAsync();

                await Task.Delay(Timeout.Infinite);
            }

            await Task.CompletedTask;
        }

        private async Task ClientReady() {
            Console.WriteLine("OK");

            await Interactions.AddModulesAsync(Assembly.GetEntryAssembly(), Services);
            
            //await Interactions.RegisterCommandsGloballyAsync();
            
            #if DEBUG
                if (Config.DevGuild != 0) {
                    await Interactions.RegisterCommandsToGuildAsync(Config.DevGuild);
                } else {
                    Console.WriteLine("DevGuild not set; commands will not be registered");
                }
            #else
                await Interactions.RegisterCommandsGloballyAsync();
            #endif

            Client.InteractionCreated += async interaction => {
                var ctx = new SocketInteractionContext(Client, interaction);
                
                await Interactions.ExecuteCommandAsync(ctx, Services);
            };

            Interactions.InteractionExecuted += async (commandInfo, interactionContext, interactionResult) => {
                if (!interactionResult.IsSuccess) {
                    await interactionContext.Interaction.RespondAsync(interactionResult.ErrorReason, ephemeral: true);
                }
            };

            await Task.CompletedTask;
        }
    }
}