using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Management.Instrumentation;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using UVA_Relay.sql;
using UVACanvasAccess.Util;
using static DSharpPlus.InteractionResponseType;
using static UVA_Relay.Canvas;
using static UVA_Relay.Utils;
using static UVACanvasAccess.ApiParts.Api;
/*
 * TODO
 * Make update command to update users information into the database
 * Maybe make one for individual and one for all
   /update user {guildid} {userid}
   /update allUsers   
 * Make command to insert new users into database
 * gathers all users in that guild and insert into database
   /get users insert {guildid} 
 * after making update command, can make update settings command
 */


// ReSharper disable ClassNeverInstantiated.Global
namespace UVA_Relay {
    
    public class AppCommands : ApplicationCommandModule {
        
    }

    [SlashCommandGroup("fetch", "Fetch commands..")]
    public class FetchCommandGroup : ApplicationCommandModule {
        
        [SlashCommand("summary", "Fetches a concise summary of a course."), SlashCooldown(1,5, SlashCooldownBucketType.User)]
        public async Task FetchCourseSummary(InteractionContext ctx, 
                                             [Option("courseId", "The course ID.")] long courseId) {
            await ctx.CreateResponseAsync(DeferredChannelMessageWithSource,
                                          new DiscordInteractionResponseBuilder().WithContent("Gimme a sec."));

            try {
                var course = await TimeoutThrow(
                    CanvasApi.GetCourse((ulong) courseId, includes: IndividualLevelCourseIncludes.Everything),
                    5000
                );

                var baseEmbed = MakeOkEmbed(course.Name, course.Id.ToString()).AddField("Term", course.Term?.Name ?? "n/a");
                var assignmentSummaryEmbed = MakeOkEmbed("Upcoming Due Dates");
                
                var assignments = await CanvasApi.StreamCourseAssignments(course.Id, AssignmentIncludes.AllDates, orderBy: "due_at")
                                                 .Where(a => a.Published)
                                                 .Where(a => a.DueAt != null)
                                                 .ToListAsync();

                if (assignments.Count == 0) {
                    assignmentSummaryEmbed.WithDescription("Nothing here!");
                }
                
                foreach (var assignment in assignments.Take(3)) {
                    Debug.Assert(assignment.DueAt != null);
                    assignmentSummaryEmbed.AddField(assignment.Name, $"Due: {assignment.DueAt.Value.FriendlyFormat()}");
                }

                if (assignments.Count > 3) {
                    assignmentSummaryEmbed.WithFooter($"And {assignments.Count - 3} more. Use '/fetch assignments' to see the rest!");
                }

                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(baseEmbed).AddEmbed(assignmentSummaryEmbed));
            } catch (Exception e) {
                Console.WriteLine(e);
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(MakeGenericErrorEmbed(timeout: e is TimeoutException)));
            }
        }

        [SlashCommand("assignments", "Fetches all upcoming assignments."), SlashCooldown(1,5, SlashCooldownBucketType.User)]
        public async Task FetchUpcomingAssignments(InteractionContext ctx,
                                                   [Option("courseId", "The course ID.")] long courseId) {
            await ctx.CreateResponseAsync(DeferredChannelMessageWithSource,
                                          new DiscordInteractionResponseBuilder().WithContent("Gimme a sec."));

            try {
                var assignments = await CanvasApi.StreamCourseAssignments((ulong) courseId, AssignmentIncludes.AllDates, orderBy: "due_at")
                                                 .Where(a => a.Published)
                                                 .Where(a => a.DueAt != null)
                                                 .Select(a => (a.Name, a.DueAt?.FriendlyFormat()))
                                                 .ToListAsync();

                
                var pages = assignments.Chunk(3).ZipCount().Select(t => {
                    var (page, i) = t;
                    var embed = MakeOkEmbed("Due Assignments", $"Page {i + 1}");
                    foreach (var (name, due) in page) {
                        embed.AddField(name, due);
                    }
                    return new Page(embed: embed);
                });

                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(MakeOkEmbed("Ok, check it out!")));
                await ctx.Interaction.Channel.SendPaginatedMessageAsync(ctx.User, pages);
            } catch (Exception e) {
                Console.WriteLine(e);
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(MakeGenericErrorEmbed(timeout: e is TimeoutException)));
            }
        }
    }
    
    [SlashCommandGroup("ping", "Ping commands.")]
    public class PingCommandGroup : ApplicationCommandModule {
        
        [SlashCommand("bot", "Pings the bot.."),SlashCooldown(1,5, SlashCooldownBucketType.User)]
        public async Task Ping(InteractionContext ctx) {
            await ctx.CreateResponseAsync(ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Pong."));
        }

        [SlashCommand("canvas", "Pings Canvas.."),SlashCooldown(1,5, SlashCooldownBucketType.User)]
        public async Task CanvasPing(InteractionContext ctx) {
            await ctx.CreateResponseAsync(DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder());

            try {
                var (good, me) = await Timeout(CanvasApi.GetUser(), 3000);
                if (good) {
                    await ctx.EditResponseAsync(
                        new DiscordWebhookBuilder().AddEmbed(MakeOkEmbed("Canvas is up!")
                                                            .AddField("I am", me.Name)
                                                            .AddField("Id", me.Id.ToString()).Build())
                    );
                } else {
                    await ctx.EditResponseAsync(
                        new DiscordWebhookBuilder().AddEmbed(MakeErrorEmbed("I'm having trouble connecting to Canvas", "Timed out after 3 seconds."))
                    );
                }
            } catch (Exception) {
                await ctx.EditResponseAsync(
                    new DiscordWebhookBuilder().AddEmbed(MakeErrorEmbed("I'm having trouble connecting to Canvas", "Exception from API."))
                );
            } 
            
        }
    }

    [SlashCommandGroup("get", "Get information")]
    public class GetCommandGroup : ApplicationCommandModule
    {
        
        [SlashCommand("user", "get user information"), SlashCooldown(1,5,SlashCooldownBucketType.User)]
        public async Task UserInfo(InteractionContext ctx)
        {  
            await ctx.CreateResponseAsync(DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder());
            try
            {
                //gets database query result
                SQL db = new SQL();
                string guildId = ctx.Guild.Id.ToString();
                string result = db.GetQuery(guildId);
                await ctx.EditResponseAsync(
                    new DiscordWebhookBuilder().WithContent($"GUILD ID: {guildId} QUERIED: {result}")
                    );
            }
            catch (Exception ex)
            {
                await ctx.EditResponseAsync(
                    new DiscordWebhookBuilder().WithContent($"Error getting information. {ex.ToString()}"));
            }
            

        }

        [SlashCommand("test", "test"), SlashCooldown(1,1, SlashCooldownBucketType.User)]
        public async Task TestInfo(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

            await Task.Delay(1000);

            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Thanks for waiting!"));
        }
    }

    [SlashCommandGroup("admin", "Commands for admin stuff")]
    public class AdminCommandGroup : ApplicationCommandModule
    {
        [SlashCommandGroup("settings", "Update settings to a guild")]
        public class AdminSubGroup : ApplicationCommandModule
        {
            [SlashCommand("optionOne", "Change option one")]
            public async Task ChangeOptionOne(InteractionContext ctx, [Option("value", "change the value")] long value)
            {
                await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
                try
                {
                    //gets database query result
                    SQL db = new SQL();
                    ulong gid = ctx.Guild.Id;
                    db.UpdateGuildSettings(gid, value,"optionOne");
                    await ctx.EditResponseAsync(
                        new DiscordWebhookBuilder()
                            .WithContent($"Updated guild settings in Guild ID: {gid} with optionOne: {value}"));

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            [SlashCommand("optionTwo", "Change option one")]
            public async Task ChangeOptionTwo(InteractionContext ctx, [Option("value", "change the value")] long value)
            {
                await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
                try
                {
                    //gets database query result
                    SQL db = new SQL();
                    ulong gid = ctx.Guild.Id;
                    db.UpdateGuildSettings(gid, value,"optionTwo");
                    await ctx.EditResponseAsync(
                        new DiscordWebhookBuilder()
                            .WithContent($"Updated guild settings in Guild ID: {gid} with optionTwo: {value}"));

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }
    }
}   