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
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task BotCommands(CommandContext CTX)
        {
            await CTX.TriggerTypingAsync();
            var output = new DiscordEmbedBuilder()
                .WithTitle("FlawBOT Commands List")
                .WithFooter("Accessible via Google Docs")
                .WithUrl("https://docs.google.com/spreadsheets/d/15c0Q7Cm07wBRNeSFwkagwDOe6zk9rVMvlM7H_Y7nGUs/edit?usp=sharing")
                .WithColor(DiscordColor.Aquamarine);
            await CTX.RespondAsync(embed: output.Build());
        }

        [Command("github")]
        [Aliases("git")]
        [Description("Get a link to the GitHub repository")]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task BotRepo(CommandContext CTX)
        {
            await CTX.TriggerTypingAsync();
            var output = new DiscordEmbedBuilder()
                .WithTitle("FlawBOT GitHub Repository")
                .WithDescription("[Click here to report an issue](https://github.com/CriticalFlaw/FlawBOT/issues/new)")
                .WithFooter("A GitHub account is required. If you do not have one use .report instead")
                .WithUrl("https://www.github.com/CriticalFlaw/flawbot")
                .WithColor(DiscordColor.Aquamarine);
            await CTX.RespondAsync(embed: output.Build());
        }

        [Command("hello")]
        [Aliases("hi")]
        [Description("Say hello to a user")]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task Greet(CommandContext CTX, [RemainingText] DiscordMember member)
        {
            await CTX.TriggerTypingAsync();
            if (member == null)
                await CTX.RespondAsync($":wave: Hello, {CTX.User.Mention}!");
            else
                await CTX.RespondAsync($":wave: Hello, {member.Mention} from {CTX.User.Mention}!");
        }

        [Command("info")]
        [Aliases("i")]
        [Description("Get the FlawBOT client information")]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task BotInfo(CommandContext CTX)
        {
            await CTX.TriggerTypingAsync();
            TimeSpan uptime = DateTime.Now - GlobalVariables.ProcessStarted;
            var output = new DiscordEmbedBuilder()
                .WithTitle("FlawBOT")
                .WithDescription("A multipurpose Discord bot created using [DSharpPlus](https://github.com/NaamloosDT/DSharpPlus).")
                .AddField("Version", GlobalVariables.Version)
                .AddField("Uptime", $"{(int)uptime.TotalDays:00}:{uptime.Hours:00}:{uptime.Minutes:00}:{uptime.Seconds:00}", true)
                .AddField("Links", "[Commands](https://docs.google.com/spreadsheets/d/15c0Q7Cm07wBRNeSFwkagwDOe6zk9rVMvlM7H_Y7nGUs/edit?usp=sharing) - [Invite](https://discordapp.com/oauth2/authorize?client_id=339833029013012483&scope=bot) - [GitHub](https://github.com/criticalflaw/flawbot) - [Discord](https://discord.gg/vqz7KCh).")
                .WithThumbnailUrl(CTX.Client.CurrentUser.AvatarUrl)
                .WithFooter("Thank you for using FlawBOT!")
                .WithUrl("https://github.com/CriticalFlaw/flawbot")
                .WithColor(DiscordColor.Aquamarine);
            await CTX.RespondAsync(embed: output.Build());
        }

        [Command("ping")]
        [Aliases("pong")]
        [Description("Ping the FlawBOT client")]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task Ping(CommandContext CTX)
        {
            await CTX.TriggerTypingAsync();
            await CTX.RespondAsync($":ping_pong: Pong! Ping: **{CTX.Client.Ping}**ms");
        }

        [Command("report")]
        [Aliases("issue")]
        [Description("Send a problem report to the developer")]
        [Cooldown(1, 10, CooldownBucketType.User)]
        public async Task ReportIssue(CommandContext CTX, [RemainingText] string report)
        {
            if ((string.IsNullOrWhiteSpace(report)) || (report.Length < 50))
                await CTX.RespondAsync("Please provide more information on the issue (50 characters minimum).");
            else
            {
                var prompt = await CTX.RespondAsync("The following information will be sent to the developer for investigation: **User ID**, **Server ID**, **Server Name** and **Server Owner Name**.\nRespond with **yes** to proceed or wait 10 seconds to cancel this operation.");
                var interactivity = CTX.Client.GetInteractivityModule();
                var input = await interactivity.WaitForMessageAsync(x => x.Author.Id == CTX.User.Id && x.Channel.Id == CTX.Channel.Id && x.Content == "yes", TimeSpan.FromSeconds(10));
                if (input == null)
                    await CTX.RespondAsync("Timed Out! Your report has **not** been submitted.");
                else
                {
                    await prompt.DeleteAsync();
                    await input.Message.DeleteAsync();
                    var DM = await CTX.Client.CreateDmAsync(CTX.Client.CurrentApplication.Owner);
                    var output = new DiscordEmbedBuilder()
                        .WithAuthor($"{CTX.User.Username}#{CTX.User.Discriminator}", icon_url: CTX.User.AvatarUrl ?? CTX.User.DefaultAvatarUrl)
                        .AddField("Issue", report)
                        .AddField("Server", $"{CTX.Guild.Name} (ID: {CTX.Guild.Id})")
                        .AddField("Owner", $"{CTX.Guild.Owner.Username}#{CTX.Guild.Owner.Discriminator}")
                        .AddField("Confirm", "[Click here to add this issue to GitHub](https://github.com/CriticalFlaw/FlawBOT/issues/new)")
                        .WithColor(DiscordColor.Turquoise);
                    await DM.SendMessageAsync(embed: output.Build());
                    await CTX.RespondAsync("Thank You! Your report has been submitted.");
                }
            }
        }

        [Command("say")]
        [Description("Repeat the inputted message")]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task Say(CommandContext CTX, [RemainingText] string message)
        {
            await CTX.TriggerTypingAsync();
            if (string.IsNullOrWhiteSpace(message))
                await CTX.RespondAsync(":thinking:");
            else
                await CTX.RespondAsync(message);
        }

        [Hidden]
        [RequireOwner]
        [Command("setactivity")]
        [Description("Set FlawBOT's activity")]
        public async Task SetBotActivity(CommandContext CTX, [RemainingText] string activity)
        {
            await CTX.TriggerTypingAsync();
            if (string.IsNullOrWhiteSpace(activity))
            {
                await CTX.Client.UpdateStatusAsync(game: null);
                await CTX.RespondAsync("FlawBOT activity has been changed to **Normal**");
            }
            else
            {
                DiscordGame game = new DiscordGame();
                game.Name = activity;
                await CTX.Client.UpdateStatusAsync(game: game);
                await CTX.RespondAsync($"FlawBOT activity has been changed to **Playing {game.Name}**");
            }
        }

        [Hidden]
        [RequireOwner]
        [Command("setavatar")]
        [Description("Set FlawBOT's avatar")]
        public async Task SetBotAvatar(CommandContext CTX, [RemainingText] string imageURL)
        {
            if (string.IsNullOrWhiteSpace(imageURL))
                imageURL = "http://givemesport.azureedge.net/images/17/07/14/d57674728648fe4608784eea3b66cbbe/960.jpg";
            await CTX.TriggerTypingAsync();
            HttpClient http = new HttpClient();
            using (var stream = await http.GetStreamAsync(imageURL))
            {
                var avatar = new MemoryStream();
                await stream.CopyToAsync(avatar);
                avatar.Position = 0;
                await CTX.Client.EditCurrentUserAsync(avatar: avatar);
                await CTX.RespondAsync("FlawBOT avatar updated!");
            }
        }

        [Hidden]
        [RequireOwner]
        [Command("setstatus")]
        [Description("Set FlawBOT's status")]
        public async Task SetBotStatus(CommandContext CTX, [RemainingText] string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                await CTX.Client.UpdateStatusAsync(user_status: UserStatus.Online);
            else
            {
                await CTX.TriggerTypingAsync();
                switch (status.Trim().ToUpper())
                {
                    case "OFF":
                    case "OFFLINE":
                        await CTX.Client.UpdateStatusAsync(user_status: UserStatus.Offline);
                        await CTX.RespondAsync("FlawBOT status has been changed to **Offline**");
                        break;

                    case "INVISIBLE":
                        await CTX.Client.UpdateStatusAsync(user_status: UserStatus.Invisible);
                        await CTX.RespondAsync("FlawBOT status has been changed to **Invisible**");
                        break;

                    case "IDLE":
                        await CTX.Client.UpdateStatusAsync(user_status: UserStatus.Idle);
                        await CTX.RespondAsync("FlawBOT status has been changed to **Idle**");
                        break;

                    case "DND":
                    case "DO NOT DISTURB":
                        await CTX.Client.UpdateStatusAsync(user_status: UserStatus.DoNotDisturb);
                        await CTX.RespondAsync("FlawBOT status has been changed to **Do Not Disturb**");
                        break;

                    default:
                        await CTX.Client.UpdateStatusAsync(user_status: UserStatus.Online);
                        await CTX.RespondAsync("FlawBOT status has been changed to **Online**");
                        break;
                }
            }
        }

        [Command("uptime")]
        [Aliases("up")]
        [Description("Get the FlawBOT client uptime")]
        public async Task Uptime(CommandContext CTX)
        {
            TimeSpan uptime = DateTime.Now - GlobalVariables.ProcessStarted;
            await CTX.RespondAsync($"This bot has been running for {(int)uptime.TotalDays:00}:{uptime.Hours:00}:{uptime.Minutes:00}:{uptime.Seconds:00}");
        }
    }
}