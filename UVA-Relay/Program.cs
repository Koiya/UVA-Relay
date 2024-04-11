using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using UVACanvasAccess.ApiParts;
//TODO 
// INSTALL TOML LIBRARY Tomlyn

namespace UVA_Relay {
    internal static class Program {
        public static async Task Main() {
            Canvas.CanvasApi = new Api(Environment.GetEnvironmentVariable("UVA_RELAY_CANVAS_TOKEN"),
                                       "https://uview.instructure.com/api/v1/");
            
            var discord = new DiscordClient(new DiscordConfiguration {
                //Token = Environment.GetEnvironmentVariable("UVA_RELAY_TOKEN") ?? throw new NullReferenceException("UVA_RELAY_TOKEN is unset"),
                Token = Environment.GetEnvironmentVariable("BOT_TOKEN"),
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.All
            });

            await discord.ConnectAsync();

            discord.UseInteractivity(new InteractivityConfiguration {
                PaginationBehaviour = PaginationBehaviour.Ignore
            });
            /*
            var testGuildId = ulong.Parse(Environment.GetEnvironmentVariable("UVA_RELAY_TEST_GUILD_ID")
                                          ?? throw new NullReferenceException("UVA_RELAY_TEST_GUILD_ID is unset"));
            */
            var testGuildId = ulong.Parse(Environment.GetEnvironmentVariable("GUILD_ID")
                                          ?? throw new NullReferenceException("UVA_RELAY_TEST_GUILD_ID is unset"));
            var slash = discord.UseSlashCommands();
            
            // Get all the guilds the bot is in
            //used for setting up database later
            discord.GuildDownloadCompleted += async(s, e) =>
            {
                var guilds = discord.Guilds;
 
                // Loop through the guilds and print their IDs
                foreach (var guild in guilds)
                {
                    ulong guildId = guild.Key;

                    Console.WriteLine($"Guild Name: {guild.Value.Name}, Guild ID: {guildId}");
                }
            };
            //DELETES GUILD COMMANDS
            //slash.RegisterCommands<ApplicationCommandModule>();
            
            //creates guild commands
            slash.RegisterCommands<AppCommands>(testGuildId);
            slash.RegisterCommands<PingCommandGroup>(testGuildId);
            slash.RegisterCommands<FetchCommandGroup>(testGuildId);
            slash.RegisterCommands<GetCommandGroup>(testGuildId);
            
            
            //turn into global commands for multiple servers
            /*slash.RegisterCommands<AppCommands>();
            slash.RegisterCommands<PingCommandGroup>();
            slash.RegisterCommands<FetchCommandGroup>();
            slash.RegisterCommands<GetCommandGroup>();*/
            await Task.Delay(-1);
        }
    }
}