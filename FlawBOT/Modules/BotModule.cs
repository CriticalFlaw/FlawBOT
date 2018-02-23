using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
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
                .WithThumbnailUrl(CTX.Client.CurrentUser.AvatarUrl)
                .WithColor(DiscordColor.Aquamarine);
            await CTX.RespondAsync(embed: output.Build());
        }

        [Command("github")]
        [Aliases("git")]
        [Description("Retrieve FlawBOT GitHub repository link")]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task GitHub(CommandContext CTX)
        {
            await CTX.TriggerTypingAsync();
            var output = new DiscordEmbedBuilder()
                .WithTitle("FlawBOT GitHub Repository")
                .WithDescription("[Click here to submit an issue report on GitHub](https://github.com/CriticalFlaw/FlawBOT/issues/new).")
                .WithFooter("*GitHub account is required. If you do not have one use .report instead")
                .WithThumbnailUrl(CTX.Client.CurrentUser.AvatarUrl)
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
                await CTX.RespondAsync($"{DiscordEmoji.FromName(CTX.Client, ":wave:")} Hello, {CTX.User.Mention}!");
            else
                await CTX.RespondAsync($"{DiscordEmoji.FromName(CTX.Client, ":wave:")} Hello, {member.Mention} from {CTX.User.Mention}!");
        }

        [Command("info")]
        [Aliases("i")]
        [Description("Retrieve FlawBOT client information")]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task BotInfo(CommandContext CTX)
        {
            await CTX.TriggerTypingAsync();
            var output = new DiscordEmbedBuilder()
                .WithTitle("FlawBOT")
                .WithDescription("A Discord bot written in C# using [DSharpPlus](https://github.com/NaamloosDT/DSharpPlus).")
                .AddField("Version", "0.5.0 (Build 20180222)")
                .AddField("Links", "[Commands](https://docs.google.com/spreadsheets/d/15c0Q7Cm07wBRNeSFwkagwDOe6zk9rVMvlM7H_Y7nGUs/edit?usp=sharing) - [Invite](https://discordapp.com/oauth2/authorize?client_id=339833029013012483&scope=bot) - [GitHub](https://github.com/criticalflaw/flawbot) - [Discord](https://discord.gg/vqz7KCh).")
                .WithThumbnailUrl(CTX.Client.CurrentUser.AvatarUrl)
                .WithFooter("Thank you for using FlawBOT!")
                .WithUrl("https://github.com/CriticalFlaw/flawbot")
                .WithColor(DiscordColor.Azure);
            await CTX.RespondAsync(embed: output.Build());
        }

        [Command("ping")]
        [Aliases("pong")]
        [Description("Ping the FlawBOT client")]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task Ping(CommandContext CTX)
        {
            await CTX.TriggerTypingAsync();
            await CTX.RespondAsync($"{DiscordEmoji.FromName(CTX.Client, ":ping_pong:")} Pong! Ping: **{CTX.Client.Ping}**ms");
        }

        [Command("report")]
        [Aliases("issue")]
        [Description("Send a report directly to developer")]
        [Cooldown(1, 60, CooldownBucketType.User)]
        public async Task ReportIssue(CommandContext CTX, [RemainingText] string query)
        {
            if ((string.IsNullOrWhiteSpace(query)) || (query.Length < 50))
                await CTX.RespondAsync($"{DiscordEmoji.FromName(CTX.Client, ":warning:")} Please provide more information on the issue (50 characters minimum).");
            else
            {
                var prompt = await CTX.RespondAsync("The following information will be sent to developer for investigation: **User ID**, **Server ID**, **Server Name** and **Server Owner Name**. Respond with **yes** to proceed or wait 10 seconds to cancel this operation.");
                var interactivity = CTX.Client.GetInteractivityModule();
                var input = await interactivity.WaitForMessageAsync(x => x.Author.Id == CTX.User.Id && x.Channel.Id == CTX.Channel.Id && x.Content == "yes", TimeSpan.FromSeconds(10));
                if (input == null)
                    await CTX.RespondAsync("Timed Out! Your report has **not** been submitted.");
                else
                {
                    await prompt.DeleteAsync();
                    await input.Message.DeleteAsync();
                    var DM = await CTX.Client.CreateDmAsync(CTX.Client.CurrentApplication.Owner);
                    var MSG = new DiscordEmbedBuilder()
                        .WithAuthor($"{CTX.User.Username}#{CTX.User.Discriminator}", icon_url: CTX.User.AvatarUrl ?? CTX.User.DefaultAvatarUrl)
                        .AddField("Issue", query)
                        .AddField("Server", $"{CTX.Guild.Name} (ID: {CTX.Guild.Id})")
                        .AddField("Owner", $"{CTX.Guild.Owner.Username}#{CTX.Guild.Owner.Discriminator}")
                        .AddField("Confirm", "[Click here to add this issue to GitHub](https://github.com/CriticalFlaw/FlawBOT/issues/new)")
                        .WithColor(DiscordColor.Black);
                    await DM.SendMessageAsync(embed: MSG.Build());
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
            if (message == null)
                await CTX.RespondAsync(":thinking:");
            else
                await CTX.RespondAsync(message, is_tts: false);
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
                await CTX.RespondAsync($"FlawBOT activity has been changed to **Normal**");
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
        public async Task SetBotAvatar(CommandContext CTX, [RemainingText] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                query = "http://givemesport.azureedge.net/images/17/07/14/d57674728648fe4608784eea3b66cbbe/960.jpg";
            using (var HTTP = new HttpClient())
            {
                using (var SR = await HTTP.GetStreamAsync(query))
                {
                    await CTX.TriggerTypingAsync();
                    var avatar = new MemoryStream();
                    await SR.CopyToAsync(avatar);
                    avatar.Position = 0;
                    await CTX.Client.EditCurrentUserAsync(avatar: avatar);
                    await CTX.RespondAsync("FlawBOT avatar updated!");
                }
            }
        }

        [Hidden]
        [RequireOwner]
        [Command("setstatus")]
        [Description("Set FlawBOT's status")]
        public async Task SetBotStatus(CommandContext CTX, string status)
        {
            await CTX.TriggerTypingAsync();
            switch (status.Trim().ToUpper())
            {
                case "OFF":
                case "OFFLINE":
                    await CTX.Client.UpdateStatusAsync(user_status: UserStatus.Offline);
                    await CTX.RespondAsync($"FlawBOT status has been changed to **Offline**");
                    break;

                case "INVISIBLE":
                    await CTX.Client.UpdateStatusAsync(user_status: UserStatus.Invisible);
                    await CTX.RespondAsync($"FlawBOT status has been changed to **Invisible**");
                    break;

                case "IDLE":
                    await CTX.Client.UpdateStatusAsync(user_status: UserStatus.Idle);
                    await CTX.RespondAsync($"FlawBOT status has been changed to **Idle**");
                    break;

                case "DND":
                case "DO NOT DISTURB":
                    await CTX.Client.UpdateStatusAsync(user_status: UserStatus.DoNotDisturb);
                    await CTX.RespondAsync($"FlawBOT status has been changed to **Do Not Disturb**");
                    break;

                default:
                    await CTX.Client.UpdateStatusAsync(user_status: UserStatus.Online);
                    await CTX.RespondAsync($"FlawBOT status has been changed to **Online**");
                    break;
            }
        }
    }
}