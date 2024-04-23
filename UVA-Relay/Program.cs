using System;
using Tomlet;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using Tomlet.Models;
using UVACanvasAccess.ApiParts;
using UVACanvasAccess.Util;
using UVA_Relay.sql;

namespace UVA_Relay {
    internal static class Program {

        public static async Task Main()
        {
            //SQL initialize
            SQL db = new SQL();
            // Get tokens from toml file
            TomlDocument document = TomlParser.ParseFile(Environment.GetEnvironmentVariable("config"));
            // document.GetSubTable("keys").GetString("BOT_TOKEN"));
            Canvas.CanvasApi = new Api(document.GetSubTable("keys").GetString("UVA_RELAY_CANVAS_TOKEN"),
                "https://uview.instructure.com/api/v1/");
            
            var discord = new DiscordClient(new DiscordConfiguration {
                //Token = Environment.GetEnvironmentVariable("UVA_RELAY_TOKEN") ?? throw new NullReferenceException("UVA_RELAY_TOKEN is unset"),
                Token = document.GetSubTable("keys").GetString("BOT_TOKEN"),
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
            var testGuildId = ulong.Parse(document.GetSubTable("keys").GetString("GUILD_ID")
                                          ?? throw new NullReferenceException("UVA_RELAY_TEST_GUILD_ID is unset"));
            var slash = discord.UseSlashCommands();
            slash.SlashCommandErrored += Utils.CmdErroredHandler;

            // Get all the guilds the bot is in
            //used for setting up database later
            discord.GuildDownloadCompleted += (s, e) =>
            {
                var guilds = discord.Guilds;
                // Loop through the guilds and print their IDs
                foreach (var guild in guilds)
                {
                    try
                    {
                        //gets database query result and add guild ids to database
                        ulong guildId = guild.Key;
                        string guildName = guild.Value.Name;
                        var memCount = guild.Value.Members;
                        foreach (var member in memCount)
                        {
                            ulong memId = member.Key;
                            db.AddUsersToDatabase(memId,guildId);
                            //Console.WriteLine(member.Value.Username);
                        }
                        db.AddGuildToDatabase(guildName, guildId);
                        //Console.WriteLine($"Guild Name:{guildName}, Guild ID: {guildId}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }

                }

                return Task.CompletedTask;
            };
            //DELETES GUILD COMMANDS
            //slash.RegisterCommands<ApplicationCommandModule>();
            
            //creates guild commands
            slash.RegisterCommands<AppCommands>(testGuildId);
            slash.RegisterCommands<PingCommandGroup>(testGuildId);
            slash.RegisterCommands<FetchCommandGroup>(testGuildId);
            slash.RegisterCommands<GetCommandGroup>(testGuildId);
            slash.RegisterCommands<AdminCommandGroup>(testGuildId);
            
            //turn into global commands for multiple servers
            /*slash.RegisterCommands<AppCommands>();
            slash.RegisterCommands<PingCommandGroup>();
            slash.RegisterCommands<FetchCommandGroup>();
            slash.RegisterCommands<GetCommandGroup>();*/
            await Task.Delay(-1);
        }
    }
}