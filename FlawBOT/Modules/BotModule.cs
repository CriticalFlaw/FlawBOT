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
    public class BotModule : BaseCommandModule
    {
        [Command("help")]
        [Aliases("cmd", "command")]
        [Description("Get a short list of commands")]
        [Cooldown(2, 5, CooldownBucketType.User)]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task Helper(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            var output = new DiscordEmbedBuilder()
                .WithTitle("FlawBOT Command List")
                .WithDescription("The **complete** command list can be found [here](https://docs.google.com/spreadsheets/d/15c0Q7Cm07wBRNeSFwkagwDOe6zk9rVMvlM7H_Y7nGUs/edit?usp=sharing)!")
                .AddField(":robot: Bot Commands", "info, leave, report")
                .AddField(":mag: Lookup Commands", "8ball, catfact, define, imdb, imgur, overwatch, pokemon, math, randomdog, shorten, simpsons, steamgame, steamlink, steamuser, tf2, time, twitch, youtube, weather")
                .AddField(":hammer: Moderation Commands", "ban, clean, deafen, kick, mute, purge, prune, removerole, removeroles, setrole, warn")
                .AddField(":tools: Server Commands", "channel, inrole, invite, mentionrole, perms, role, poll, server, setname, setnickname, setserveravatar, setservername, settopic, user, createrole, createtext, createvoice")
                .WithColor(DiscordColor.Turquoise);
            await ctx.RespondAsync(embed: output.Build());
        }

        [Command("github")]
        [Aliases("git")]
        [Description("Get a link to the GitHub repository")]
        [Cooldown(2, 5, CooldownBucketType.User)]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task GitHub(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            var output = new DiscordEmbedBuilder()
                .WithTitle("FlawBOT GitHub Repository")
                .WithDescription("[Click here to report an issue](https://github.com/criticalflaw/flawbot/issues/new)")
                .WithFooter("A GitHub account is required. If you do not have one use .report instead")
                .WithThumbnailUrl("https://image.flaticon.com/icons/svg/25/25231.svg")
                .WithUrl("https://www.github.com/criticalflaw/flawbot")
                .WithColor(DiscordColor.Aquamarine);
            await ctx.RespondAsync(embed: output.Build());
        }

        [Command("hello")]
        [Aliases("hi", "howdy")]
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
                .WithDescription("A multipurpose Discord bot created using [DSharpPlus](https://github.com/NaamloosDT/DSharpPlus).")
                .AddField("FlawBOT Version", GlobalVariables.Version, true)
                .AddField("DSharpPlus Version", ctx.Client.VersionString, true)
                .AddField("Uptime", $"{(int)uptime.TotalDays:00}:{uptime.Hours:00}:{uptime.Minutes:00}:{uptime.Seconds:00}", true)
                .AddField("Ping", $"{ctx.Client.Ping}ms", true)
                .AddField("Links", "[Commands](https://docs.google.com/spreadsheets/d/15c0Q7Cm07wBRNeSFwkagwDOe6zk9rVMvlM7H_Y7nGUs/edit?usp=sharing) **|** [Invite](https://discordapp.com/oauth2/authorize?client_id=339833029013012483&scope=bot) **|** [GitHub](https://github.com/criticalflaw/flawbot) **|** [Discord](https://discord.gg/vqz7KCh).")
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
                var interactivity = ctx.Client.GetInteractivity();
                var input = await interactivity.WaitForMessageAsync(x => x.Author.Id == ctx.User.Id && x.Channel.Id == ctx.Channel.Id && x.Content == "yes", TimeSpan.FromSeconds(10));
                if (input == null)
                    await ctx.RespondAsync("Timed Out! Your report has **NOT** been submitted.");
                else
                {
                    await prompt.DeleteAsync();
                    await input.Message.DeleteAsync();
                    var dm = await ctx.Member.CreateDmChannelAsync();
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

        #region OWNER-ONLY

        [Hidden]
        [RequireOwner]
        [Command("setactivity")]
        [Description("Set FlawBOT's activity")]
        public async Task SetActivity(CommandContext ctx, [RemainingText] string activity)
        {
            await ctx.TriggerTypingAsync();
            if (string.IsNullOrWhiteSpace(activity))
            {
                await ctx.Client.UpdateStatusAsync(null);
                await ctx.RespondAsync("FlawBOT activity has been changed to **Normal**");
            }
            else
            {
                var game = new DiscordActivity { Name = activity };
                await ctx.Client.UpdateStatusAsync(game);
                await ctx.RespondAsync($"FlawBOT activity has been changed to **Playing {game.Name}**");
            }
        }

        [Hidden]
        [RequireOwner]
        [Command("setavatar")]
        [Description("Set FlawBOT's avatar")]
        public async Task SetAvatar(CommandContext ctx, [RemainingText] string query)
        {
            try
            {
                await ctx.TriggerTypingAsync();
                var http = new HttpClient();
                if (string.IsNullOrWhiteSpace(query))
                    query = "http://givemesport.azureedge.net/images/17/07/14/d57674728648fe4608784eea3b66cbbe/960.jpg";
                using (var stream = await http.GetStreamAsync(query))
                {
                    var avatar = new MemoryStream();
                    await stream.CopyToAsync(avatar);
                    avatar.Position = 0;
                    await ctx.Client.UpdateCurrentUserAsync(avatar: avatar);
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
        public async Task SetStatus(CommandContext ctx, [RemainingText] string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                await ctx.Client.UpdateStatusAsync(userStatus: UserStatus.Online);
            else
            {
                await ctx.TriggerTypingAsync();
                switch (status.Trim().ToUpperInvariant())
                {
                    case "OFF":
                    case "OFFLINE":
                        await ctx.Client.UpdateStatusAsync(userStatus: UserStatus.Offline);
                        await ctx.RespondAsync("FlawBOT status has been changed to **Offline**");
                        break;

                    case "INVISIBLE":
                        await ctx.Client.UpdateStatusAsync(userStatus: UserStatus.Invisible);
                        await ctx.RespondAsync("FlawBOT status has been changed to **Invisible**");
                        break;

                    case "IDLE":
                        await ctx.Client.UpdateStatusAsync(userStatus: UserStatus.Idle);
                        await ctx.RespondAsync("FlawBOT status has been changed to **Idle**");
                        break;

                    case "DND":
                    case "DO NOT DISTURB":
                        await ctx.Client.UpdateStatusAsync(userStatus: UserStatus.DoNotDisturb);
                        await ctx.RespondAsync("FlawBOT status has been changed to **Do Not Disturb**");
                        break;

                    default:
                        await ctx.Client.UpdateStatusAsync(userStatus: UserStatus.Online);
                        await ctx.RespondAsync("FlawBOT status has been changed to **Online**");
                        break;
                }
            }
        }

        [Hidden]
        [RequireOwner]
        [Command("shutdown")]
        [Description("Shutdown the FlawBOT client")]
        public async Task Shutdown(CommandContext ctx)
        {
            await Task.Delay(0).ConfigureAwait(false);
        }

        #endregion OWNER-ONLY
    }
}