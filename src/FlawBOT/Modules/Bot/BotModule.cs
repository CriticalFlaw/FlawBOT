using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using FlawBOT.Models;
using FlawBOT.Services;
using System;
using System.Threading.Tasks;

namespace FlawBOT.Modules.Bot
{
    [Group("bot")]
    [Description("Basic commands for interacting with FlawBOT")]
    [Cooldown(3, 5, CooldownBucketType.Channel)]
    public class BotModule : BaseCommandModule
    {
        #region COMMAND_HELLO

        [Command("hello")]
        [Aliases("hi", "howdy")]
        [Description("Say hello to another server user")]
        public async Task Greet(CommandContext ctx,
            [Description("User to say hello to")] [RemainingText] DiscordMember member)
        {
            if (member == null)
                await ctx.RespondAsync($":wave: Hello, {ctx.User.Mention}!");
            else
                await ctx.RespondAsync($":wave: Hello, {member.Mention} from {ctx.User.Mention}!");
        }

        #endregion COMMAND_HELLO

        #region COMMAND_INFO

        [Command("info")]
        [Aliases("i")]
        [Description("Retrieve FlawBOT information")]
        public async Task BotInfo(CommandContext ctx)
        {
            var uptime = DateTime.Now - GlobalVariables.ProcessStarted;
            var output = new DiscordEmbedBuilder()
                .WithTitle("FlawBOT")
                .WithDescription("A multipurpose Discord bot created using [DSharpPlus](https://github.com/NaamloosDT/DSharpPlus).")
                .AddField("FlawBOT Version", GlobalVariables.Version, true)
                .AddField("DSharpPlus Version", ctx.Client.VersionString, true)
                .AddField("Uptime", $"{(int)uptime.TotalDays:00}:{uptime.Hours:00}:{uptime.Minutes:00}:{uptime.Seconds:00}", true)
                .AddField("Ping", $"{ctx.Client.Ping}ms", true)
                .AddField("Links", "[Commands](https://docs.google.com/spreadsheets/d/15c0Q7Cm07wBRNeSFwkagwDOe6zk9rVMvlM7H_Y7nGUs/edit?usp=sharing) **|** [Invite](https://discordapp.com/oauth2/authorize?client_id=339833029013012483&scope=bot) **|** [GitHub](https://github.com/criticalflaw/flawbot)")
                .WithThumbnailUrl(ctx.Client.CurrentUser.AvatarUrl)
                .WithFooter("Thank you for using FlawBOT!")
                .WithUrl("https://github.com/criticalflaw/flawbot")
                .WithColor(DiscordColor.Aquamarine);
            await ctx.RespondAsync(embed: output.Build());
        }

        #endregion COMMAND_INFO

        #region COMMAND_LEAVE

        [Command("leave")]
        [Description("Make FlawBOT leave the current server")]
        [RequireUserPermissions(Permissions.Administrator)]
        public async Task LeaveAsync(CommandContext ctx)
        {
            await BotServices.SendEmbedAsync(ctx, "Are you sure you want FlawBOT to leave this server?\nRespond with **yes** to proceed or wait 10 seconds to cancel this operation.");
            var interactivity = await ctx.Client.GetInteractivity().WaitForMessageAsync(m => m.Channel.Id == ctx.Channel.Id && m.Content.ToLowerInvariant() == "yes", TimeSpan.FromSeconds(10));
            if (interactivity == null)
                await BotServices.SendEmbedAsync(ctx, "Request cancelled...");
            else
            {
                await BotServices.SendEmbedAsync(ctx, "Thank you for using FlawBOT...");
                await ctx.Guild.LeaveAsync();
            }
        }

        #endregion COMMAND_LEAVE

        #region COMMAND_PING

        [Command("ping")]
        [Aliases("pong")]
        [Description("Ping the FlawBOT client")]
        public async Task Ping(CommandContext ctx)
        {
            await ctx.RespondAsync($":ping_pong: Pong! Ping: **{ctx.Client.Ping}**ms");
        }

        #endregion COMMAND_PING

        #region COMMAND_REPORT

        [Command("report")]
        [Aliases("issue")]
        [Description("Send a problem report to the developer. Please do not abuse.")]
        public async Task ReportIssue(CommandContext ctx,
            [Description("Detailed description of the issue")] [RemainingText] string report)
        {
            if (string.IsNullOrWhiteSpace(report) || report.Length < 50)
                await ctx.RespondAsync("Please provide more information on the issue (50 characters minimum).");
            else
            {
                var prompt = await ctx.RespondAsync("The following information will be sent to the developer for investigation: **User ID**, **Server ID**, **Server Name** and **Server Owner Name**.\nRespond with **yes** to proceed or wait 10 seconds to cancel this operation.");
                var input = await ctx.Client.GetInteractivity().WaitForMessageAsync(x => x.Author.Id == ctx.User.Id && x.Channel.Id == ctx.Channel.Id && x.Content == "yes", TimeSpan.FromSeconds(10));
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
                    await BotServices.SendEmbedAsync(ctx, "Thank You! Your report has been submitted.", EmbedType.Good);
                }
            }
        }

        #endregion COMMAND_REPORT

        #region COMMAND_SAY

        [Command("say")]
        [Aliases("echo")]
        [Description("Repeat a message")]
        public Task Say(CommandContext ctx,
            [Description("Message for the bot to repeat")] [RemainingText] string message)
        {
            message = (string.IsNullOrWhiteSpace(message)) ? ":thinking:" : message;
            return ctx.RespondAsync(message);
        }

        #endregion COMMAND_SAY

        #region COMMAND_UPTIME

        [Command("uptime")]
        [Aliases("time")]
        [Description("Retrieve the FlawBOT uptime")]
        public async Task Uptime(CommandContext ctx)
        {
            var uptime = DateTime.Now - GlobalVariables.ProcessStarted;
            await BotServices.SendEmbedAsync(ctx, $":clock1: FlawBOT has been online for {(int)uptime.TotalDays:00} days ({uptime.Hours:00}:{uptime.Minutes:00}:{uptime.Seconds:00})");
        }

        #endregion COMMAND_UPTIME
    }
}