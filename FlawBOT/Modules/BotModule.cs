using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using FlawBOT.Services;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    public class BotModule
    {
        [Command("commands")]
        [Aliases("cmd", "cmds")]
        [Description("Get a link to the complete commands list")]
        [Cooldown(2, 5, CooldownBucketType.User)]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task BotCommands(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            var output = new DiscordEmbedBuilder()
                .WithTitle("FlawBOT Commands List")
                .WithFooter("Accessible via Google Docs")
                .WithUrl("https://docs.google.com/spreadsheets/d/15c0Q7Cm07wBRNeSFwkagwDOe6zk9rVMvlM7H_Y7nGUs/edit?usp=sharing")
                .WithColor(DiscordColor.Aquamarine);
            await ctx.RespondAsync(embed: output.Build());
        }

        [Command("help")]
        [Description("Get a sample of the commands list")]
        [Cooldown(2, 5, CooldownBucketType.User)]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task HelpCommands(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            var output = new DiscordEmbedBuilder()
                .WithTitle("FlawBOT Command List")
                .WithDescription("For the **complete** command list, click [here](https://docs.google.com/spreadsheets/d/15c0Q7Cm07wBRNeSFwkagwDOe6zk9rVMvlM7H_Y7nGUs/edit?usp=sharing)!\n> **.info** Get the FlawBOT client information\n> **.report** Send a problem report to the developer\n> **.8ball** Roll an 8-ball\n> **.tf2** Get a random Team Fortress 2 item\n> **.pokemon** Get a random Pokemon card\n> **.overwatch** Get Overwatch player information\n> **.dictionary** Get an Urban Dictionary entry for a word or phrase\n> **.imdb** Get a movie or TV show from OMDB\n> **.imgur** Get an imager from Imgur\n> **.math** Perform a basic math operation\n> **.simpsons** Get a random Simpsons screenshot and episode\n> **.time** Get time for specified location\n> **.weather** Get weather information for specified location\n> **.twitch** Get Twitch stream information\n> **.youtube** Get the first YouTube video search result\n> **.steamgame** Get a random Steam game")
                .WithColor(DiscordColor.Turquoise);
            await ctx.RespondAsync(embed: output.Build());
        }

        [Command("github")]
        [Aliases("git")]
        [Description("Get a link to the GitHub repository")]
        [Cooldown(2, 5, CooldownBucketType.User)]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task BotRepo(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            var output = new DiscordEmbedBuilder()
                .WithTitle("FlawBOT GitHub Repository")
                .WithDescription("[Click here to report an issue](https://github.com/criticalflaw/flawbot/issues/new)")
                .WithFooter("A GitHub account is required. If you do not have one use .report instead")
                .WithUrl("https://www.github.com/criticalflaw/flawbot")
                .WithColor(DiscordColor.Aquamarine);
            await ctx.RespondAsync(embed: output.Build());
        }

        [Command("hello")]
        [Aliases("hi")]
        [Description("Say hello to a user")]
        [Cooldown(2, 5, CooldownBucketType.User)]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task Greet(CommandContext ctx, [RemainingText] DiscordMember member)
        {
            await ctx.TriggerTypingAsync();
            if (member == null)
                await ctx.RespondAsync($":wave: Hello, {ctx.User.Mention}!");
            else
                await ctx.RespondAsync($":wave: Hello, {member.Mention} from {ctx.User.Mention}!");
        }

        [Command("info")]
        [Aliases("i")]
        [Description("Get the FlawBOT client information")]
        [Cooldown(2, 5, CooldownBucketType.User)]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task BotInfo(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            var uptime = DateTime.Now - GlobalVariables.ProcessStarted;
            var output = new DiscordEmbedBuilder()
                .WithTitle("FlawBOT")
                .WithDescription("A multipurpose Discord bot created using [DSharpPlus](https://github.com/NaamloosDT/DSharpPlus).\nPlease work inline")
                .AddField("Version", GlobalVariables.Version, true)
                .AddField("Uptime", $"{(int)uptime.TotalDays:00}:{uptime.Hours:00}:{uptime.Minutes:00}:{uptime.Seconds:00}", true)
                .AddField("Ping", $"{ctx.Client.Ping}ms", true)
                .AddField("Links", "[Commands](https://docs.google.com/spreadsheets/d/15c0Q7Cm07wBRNeSFwkagwDOe6zk9rVMvlM7H_Y7nGUs/edit?usp=sharing) - [Invite](https://discordapp.com/oauth2/authorize?client_id=339833029013012483&scope=bot) - [GitHub](https://github.com/criticalflaw/flawbot) - [Discord](https://discord.gg/vqz7KCh).")
                .WithThumbnailUrl(ctx.Client.CurrentUser.AvatarUrl)
                .WithFooter("Thank you for using FlawBOT!")
                .WithUrl("https://github.com/criticalflaw/flawbot")
                .WithColor(DiscordColor.Aquamarine);
            await ctx.RespondAsync(embed: output.Build());
        }

        [Command("ping")]
        [Aliases("pong")]
        [Description("Ping the FlawBOT client")]
        [Cooldown(2, 5, CooldownBucketType.User)]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task Ping(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            await ctx.RespondAsync($":ping_pong: Pong! Ping: **{ctx.Client.Ping}**ms");
        }

        [Command("report")]
        [Aliases("issue")]
        [Description("Send a problem report to the developer")]
        [Cooldown(1, 10, CooldownBucketType.User)]
        [Cooldown(3, 10, CooldownBucketType.Channel)]
        public async Task ReportIssue(CommandContext ctx, [RemainingText] string report)
        {
            if (string.IsNullOrWhiteSpace(report) || report.Length < 50)
                await ctx.RespondAsync("Please provide more information on the issue (50 characters minimum).");
            else
            {
                var prompt = await ctx.RespondAsync("The following information will be sent to the developer for investigation: **User ID**, **Server ID**, **Server Name** and **Server Owner Name**.\nRespond with **yes** to proceed or wait 10 seconds to cancel this operation.");
                var interactivity = ctx.Client.GetInteractivityModule();
                var input = await interactivity.WaitForMessageAsync(x => x.Author.Id == ctx.User.Id && x.Channel.Id == ctx.Channel.Id && x.Content == "yes", TimeSpan.FromSeconds(10));
                if (input == null)
                    await ctx.RespondAsync("Timed Out! Your report has **NOT** been submitted.");
                else
                {
                    await prompt.DeleteAsync();
                    await input.Message.DeleteAsync();
                    var dm = await ctx.Client.CreateDmAsync(ctx.Client.CurrentApplication.Owner);
                    var output = new DiscordEmbedBuilder()
                        .WithAuthor($"{ctx.User.Username}#{ctx.User.Discriminator}", icon_url: ctx.User.AvatarUrl ?? ctx.User.DefaultAvatarUrl)
                        .AddField("Issue", report)
                        .AddField("Server", $"{ctx.Guild.Name} (ID: {ctx.Guild.Id})")
                        .AddField("Owner", $"{ctx.Guild.Owner.Username}#{ctx.Guild.Owner.Discriminator}")
                        .AddField("Confirm", "[Click here to add this issue to GitHub](https://github.com/criticalflaw/flawbot/issues/new)")
                        .WithColor(DiscordColor.Turquoise);
                    await dm.SendMessageAsync(embed: output.Build());
                    await ctx.RespondAsync("Thank You! Your report has been submitted.");
                }
            }
        }

        [Command("say")]
        [Description("Repeat the inputted message")]
        [Cooldown(2, 5, CooldownBucketType.User)]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task Say(CommandContext ctx, [RemainingText] string message)
        {
            await ctx.TriggerTypingAsync();
            if (string.IsNullOrWhiteSpace(message))
                await ctx.RespondAsync(":thinking:");
            else
                await ctx.RespondAsync(message);
        }

        [Hidden]
        [RequireOwner]
        [Command("setactivity")]
        [Description("Set FlawBOT's activity")]
        public async Task SetBotActivity(CommandContext ctx, [RemainingText] string activity)
        {
            await ctx.TriggerTypingAsync();
            if (string.IsNullOrWhiteSpace(activity))
            {
                await ctx.Client.UpdateStatusAsync(null);
                await ctx.RespondAsync("FlawBOT activity has been changed to **Normal**");
            }
            else
            {
                var game = new DiscordGame { Name = activity };
                await ctx.Client.UpdateStatusAsync(game);
                await ctx.RespondAsync($"FlawBOT activity has been changed to **Playing {game.Name}**");
            }
        }

        [Hidden]
        [RequireOwner]
        [Command("setavatar")]
        [Description("Set FlawBOT's avatar")]
        public async Task SetBotAvatar(CommandContext ctx, [RemainingText] string query)
        {
            try
            {
                await ctx.TriggerTypingAsync();
                var http = new HttpClient();
                using (var stream = await http.GetStreamAsync(query))
                {
                    var avatar = new MemoryStream();
                    await stream.CopyToAsync(avatar);
                    avatar.Position = 0;
                    await ctx.Client.EditCurrentUserAsync(avatar: avatar);
                    await ctx.RespondAsync("FlawBOT avatar updated!");
                }
            }
            catch
            {
                await ctx.RespondAsync(":warning: Provided URL is not a valid image file!");
            }
        }

        [Hidden]
        [RequireOwner]
        [Command("setstatus")]
        [Description("Set FlawBOT's status")]
        public async Task SetBotStatus(CommandContext ctx, [RemainingText] string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                await ctx.Client.UpdateStatusAsync(user_status: UserStatus.Online);
            else
            {
                await ctx.TriggerTypingAsync();
                switch (status.Trim().ToUpper())
                {
                    case "OFF":
                    case "OFFLINE":
                        await ctx.Client.UpdateStatusAsync(user_status: UserStatus.Offline);
                        await ctx.RespondAsync("FlawBOT status has been changed to **Offline**");
                        break;

                    case "INVISIBLE":
                        await ctx.Client.UpdateStatusAsync(user_status: UserStatus.Invisible);
                        await ctx.RespondAsync("FlawBOT status has been changed to **Invisible**");
                        break;

                    case "IDLE":
                        await ctx.Client.UpdateStatusAsync(user_status: UserStatus.Idle);
                        await ctx.RespondAsync("FlawBOT status has been changed to **Idle**");
                        break;

                    case "DND":
                    case "DO NOT DISTURB":
                        await ctx.Client.UpdateStatusAsync(user_status: UserStatus.DoNotDisturb);
                        await ctx.RespondAsync("FlawBOT status has been changed to **Do Not Disturb**");
                        break;

                    default:
                        await ctx.Client.UpdateStatusAsync(user_status: UserStatus.Online);
                        await ctx.RespondAsync("FlawBOT status has been changed to **Online**");
                        break;
                }
            }
        }

        [Command("uptime")]
        [Aliases("up")]
        [Description("Get the FlawBOT client uptime")]
        [Cooldown(2, 5, CooldownBucketType.User)]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task Uptime(CommandContext ctx)
        {
            var uptime = DateTime.Now - GlobalVariables.ProcessStarted;
            await ctx.RespondAsync($"This bot has been running for {(int)uptime.TotalDays:00}:{uptime.Hours:00}:{uptime.Minutes:00}:{uptime.Seconds:00}");
        }

        [Hidden]
        [RequireOwner]
        [Command("shutdown")]
        [Description("Shutdown the FlawBOT client")]
        public async Task Shutdown(CommandContext ctx)
        {
            await Task.Delay(0).ConfigureAwait(false);
        }
    }
}