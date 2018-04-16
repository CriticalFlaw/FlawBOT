using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using FlawBOT.Services;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    public class ServerModule : BaseCommandModule
    {
        [Command("channel")]
        [Aliases("cid")]
        [Description("Get channel information")]
        [Cooldown(2, 5, CooldownBucketType.User)]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task GetChannel(CommandContext ctx, [RemainingText] DiscordChannel channel)
        {
            if (channel == null)
                channel = ctx.Channel;
            var output = new DiscordEmbedBuilder()
                .WithTitle($"#{channel.Name}")
                .AddField("ID", channel.Id.ToString(), true)
                .AddField("Created on", channel.CreationTimestamp.DateTime.ToString(CultureInfo.InvariantCulture), true)
                .AddField("Type", channel.Type.ToString(), true)
                .AddField("Private?", channel.IsPrivate ? "YES" : "NO", true)
                .AddField("NSFW?", channel.IsNSFW ? "YES" : "NO", true)
                .AddField("User Limit", channel.UserLimit.ToString(), true)
                .WithThumbnailUrl(ctx.Guild.IconUrl)
                .WithFooter($"{ctx.Guild.Name} / #{ctx.Channel.Name} / {DateTime.Now}")
                .WithUrl($"https://discord.gg/{ctx.Channel.CreateInviteAsync().Result.Code}");
            if (!string.IsNullOrWhiteSpace(channel.Topic)) output.AddField("Topic", channel.Topic, true);
            await ctx.RespondAsync(embed: output.Build());
        }

        [Command("clean")]
        [Aliases("clear")]
        [Description("Remove server messages")]
        [RequirePermissions(Permissions.ManageMessages)]
        [Cooldown(1, 5, CooldownBucketType.User)]
        [Cooldown(2, 10, CooldownBucketType.Guild)]
        public async Task Clean(CommandContext ctx, int limit, [RemainingText] DiscordChannel channel)
        {
            if (limit <= 0 || limit > 100)
                await BotServices.SendErrorEmbedAsync(ctx, ":warning: Invalid number of messages to delete, must be in range of 1-100!");
            else
            {
                await ctx.TriggerTypingAsync();
                if (channel == null)
                    channel = ctx.Channel;
                var messages = await ctx.Channel.GetMessagesAsync(limit).ConfigureAwait(false);
                await ctx.Channel.DeleteMessagesAsync(messages).ConfigureAwait(false);
                await ctx.RespondAsync($"**{messages.Count}** message(s) have been removed from #{ctx.Channel.Name}");
            }
        }

        [Command("colorrole")]
        [Aliases("clrr")]
        [Description("Set a role's color to HEX color values")]
        [RequirePermissions(Permissions.ManageRoles)]
        [Cooldown(2, 5, CooldownBucketType.User)]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task ColorRole(CommandContext ctx, params string[] colors)
        {
            try
            {
                if (colors.Length != 2 && colors.Length != 4)
                    await BotServices.SendErrorEmbedAsync(ctx, ":warning: Invalid parameters, try **.colorrole admin [0-255] [0-255] [0-255]**");
                else
                {
                    await ctx.TriggerTypingAsync();
                    var roleName = colors[0].ToLowerInvariant();
                    var role = ctx.Guild.Roles.FirstOrDefault(r => r.Name.ToLowerInvariant() == roleName);
                    if (role == null)
                        await BotServices.SendErrorEmbedAsync(ctx, ":mag: Role not found in this server!");
                    else
                    {
                        var rgb = colors.Length == 4;
                        var arg = colors[1].Replace("#", "");
                        var color = new DiscordColor(
                            Convert.ToByte(rgb ? int.Parse(arg) : Convert.ToInt32(arg.Substring(0, 2), 16)),
                            Convert.ToByte(rgb ? int.Parse(colors[2]) : Convert.ToInt32(arg.Substring(2, 2), 16)),
                            Convert.ToByte(rgb ? int.Parse(colors[3]) : Convert.ToInt32(arg.Substring(4, 2), 16)));
                        await role.UpdateAsync(color: color).ConfigureAwait(false);
                        await ctx.RespondAsync($"Color of the role **{roleName}** has been changed to **{color}**");
                    }
                }
            }
            catch
            {
                await BotServices.SendErrorEmbedAsync(ctx, ":warning: Unable to change role color, try **.colorrole admin [0-255] [0-255] [0-255]**");
            }
        }

        [Command("createrole")]
        [Aliases("csr")]
        [Description("Create a server role")]
        [RequirePermissions(Permissions.ManageRoles)]
        [Cooldown(1, 5, CooldownBucketType.User)]
        [Cooldown(2, 5, CooldownBucketType.Guild)]
        public async Task CreateRole(CommandContext ctx, [RemainingText] string role)
        {
            if (string.IsNullOrWhiteSpace(role))
                await BotServices.SendErrorEmbedAsync(ctx, ":warning: Role name cannot be blank!");
            else
            {
                await ctx.TriggerTypingAsync();
                await ctx.Guild.CreateRoleAsync(role);
                await ctx.RespondAsync($"Role **{role}** has been **created**");
            }
        }

        [Command("createtext")]
        [Aliases("ctc")]
        [Description("Create a text channel")]
        [RequirePermissions(Permissions.ManageChannels)]
        [Cooldown(1, 5, CooldownBucketType.User)]
        [Cooldown(2, 5, CooldownBucketType.Guild)]
        public async Task CreateTextChannel(CommandContext ctx, [RemainingText] string channel)
        {
            if (string.IsNullOrWhiteSpace(channel))
                await BotServices.SendErrorEmbedAsync(ctx, ":warning: Channel name cannot be blank!");
            else
            {
                await ctx.TriggerTypingAsync();
                await ctx.Guild.CreateChannelAsync(channel.Trim().Replace(" ", "-"), ChannelType.Text, ctx.Channel.Parent);
                await ctx.RespondAsync($"Text Channel **#{channel.Trim().Replace(" ", "-")}** has been **created**");
            }
        }

        [Command("createvoice")]
        [Aliases("cvc")]
        [Description("Create a voice channel")]
        [RequirePermissions(Permissions.ManageChannels)]
        [Cooldown(1, 5, CooldownBucketType.User)]
        [Cooldown(2, 5, CooldownBucketType.Guild)]
        public async Task CreateVoiceChannel(CommandContext ctx, [RemainingText] string channel)
        {
            if (string.IsNullOrWhiteSpace(channel))
                await BotServices.SendErrorEmbedAsync(ctx, ":warning: Channel name cannot be blank!");
            else
            {
                await ctx.TriggerTypingAsync();
                await ctx.Guild.CreateChannelAsync(channel.Trim().Replace(" ", "-"), ChannelType.Voice, ctx.Channel.Parent);
                await ctx.RespondAsync($"Voice Channel **#{channel.Trim().Replace(" ", "-")}** has been **created**");
            }
        }

        [Command("deleterole")]
        [Aliases("dsr")]
        [Description("Delete a server role")]
        [RequirePermissions(Permissions.ManageRoles)]
        [Cooldown(1, 5, CooldownBucketType.User)]
        [Cooldown(2, 5, CooldownBucketType.Guild)]
        public async Task DeleteRole(CommandContext ctx, [RemainingText] DiscordRole role)
        {
            if (role == null)
                await BotServices.SendErrorEmbedAsync(ctx, ":mag: Role not found in this server!");
            else
            {
                await ctx.TriggerTypingAsync();
                await role.DeleteAsync();
                await ctx.RespondAsync($"Role **{role.Name}** has been **removed**");
            }
        }

        [Command("deletetext")]
        [Aliases("dtc")]
        [Description("Remove a text channel")]
        [RequirePermissions(Permissions.ManageChannels)]
        [Cooldown(1, 5, CooldownBucketType.User)]
        [Cooldown(2, 5, CooldownBucketType.Guild)]
        public async Task RemoveTextChannel(CommandContext ctx, [RemainingText] DiscordChannel channel)
        {
            if (channel == null)
                await BotServices.SendErrorEmbedAsync(ctx, ":mag: Channel not found in this server!");
            else if (channel.Type != ChannelType.Text)
                await BotServices.SendErrorEmbedAsync(ctx, ":warning: This is not a text channel, use **.deletevoice** instead!");
            else
            {
                await ctx.TriggerTypingAsync();
                await channel.DeleteAsync();
                await ctx.RespondAsync($"Text Channel **{channel.Name}** has been **removed**");
            }
        }

        [Command("deletevoice")]
        [Aliases("dvc")]
        [Description("Remove a voice channel")]
        [RequirePermissions(Permissions.ManageChannels)]
        [Cooldown(1, 5, CooldownBucketType.User)]
        [Cooldown(2, 5, CooldownBucketType.Guild)]
        public async Task RemoveVoiceChannel(CommandContext ctx, [RemainingText] DiscordChannel channel)
        {
            if (channel == null)
                await BotServices.SendErrorEmbedAsync(ctx, ":mag: Channel not found in this server!");
            else if (channel.Type != ChannelType.Voice)
                await BotServices.SendErrorEmbedAsync(ctx, ":warning: This is not a voice channel, use **.deletetext** instead!");
            else
            {
                await ctx.TriggerTypingAsync();
                await channel.DeleteAsync();
                await ctx.RespondAsync($"Voice Channel **{channel.Name}** has been **removed**");
            }
        }

        [Command("inrole")]
        [Description("Lists all users in specified role")]
        [Cooldown(2, 5, CooldownBucketType.User)]
        [Cooldown(3, 5, CooldownBucketType.Guild)]
        public async Task UsersInRole(CommandContext ctx, [RemainingText] string roleName)
        {
            var role = ctx.Guild.Roles.FirstOrDefault(r => r.Name.ToLowerInvariant() == roleName);
            if (role != null)
            {
                await ctx.TriggerTypingAsync();
                var userCount = 0;
                string usersList = null;
                var users = (await ctx.Guild.GetAllMembersAsync()).ToArray();
                foreach (var user in users)
                    if (user.Roles.Contains(role))
                    {
                        userCount++;
                        if (user.Equals(users.Last()))
                            usersList += $"{user.DisplayName}";
                        else
                            usersList += $"{user.DisplayName}, ";
                    }

                if (string.IsNullOrWhiteSpace(usersList))
                    await ctx.RespondAsync($"Role **{role.Name}** has no members");
                else
                    await ctx.RespondAsync($"Role **{role.Name}** has **{userCount}** member(s): {usersList}");
            }
        }

        [Command("invite")]
        [Aliases("inv")]
        [Description("Create an instant invite link for this server")]
        [Cooldown(2, 5, CooldownBucketType.User)]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task InviteAsync(CommandContext ctx)
        {
            await BotServices.SendErrorEmbedAsync(ctx, $"Instant Invite to **{ctx.Guild.Name}**: https://discord.gg/{ctx.Channel.CreateInviteAsync().Result.Code}");
        }

        [Command("leave")]
        [Description("Makes this bot leave the current server.")]
        [RequireUserPermissions(Permissions.Administrator)]
        [Cooldown(2, 5, CooldownBucketType.User)]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task LeaveAsync(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            var interactivity = ctx.Client.GetInteractivity();
            await ctx.RespondAsync("Are you sure you want to remove FlawBOT from this server?\nRespond with **yes** to proceed or wait 15 seconds to cancel this operation.");
            var response = await interactivity.WaitForMessageAsync(x => x.ChannelId == ctx.Channel.Id && x.Author.Id == ctx.Member.Id, TimeSpan.FromSeconds(15));
            if (response.Message.Content.ToUpper() == "YES")
            {
                await ctx.RespondAsync("Thank you for using FlawBOT...");
                await ctx.Guild.LeaveAsync();
            }
            else
                await ctx.RespondAsync("Request timed out...");
        }

        [Command("mentionrole")]
        [Aliases("mr")]
        [Description("Toggle whether this role can be mentioned by others")]
        [RequirePermissions(Permissions.ManageRoles)]
        [Cooldown(2, 5, CooldownBucketType.User)]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task MentionRole(CommandContext ctx, [RemainingText] DiscordRole role)
        {
            if (role == null) return;
            await ctx.TriggerTypingAsync();
            if (role.IsMentionable)
            {
                await role.UpdateAsync(mentionable: false);
                await ctx.RespondAsync($"Role **{role.Name}** is now **not-mentionable**");
            }
            else
            {
                await role.UpdateAsync(mentionable: true);
                await ctx.RespondAsync($"Role **{role.Name}** is now **mentionable**");
            }
        }

        [Command("perms")]
        [Aliases("prm")]
        [Description("Retrieve your server permissions")]
        [Cooldown(2, 5, CooldownBucketType.User)]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task ListServerPermissions(CommandContext ctx, DiscordMember member = null, DiscordChannel channel = null)
        {
            if (member == null)
                member = ctx.Member;
            if (channel == null)
                channel = ctx.Channel;
            await ctx.TriggerTypingAsync();
            var perms = $"{Formatter.Bold(member.DisplayName)} cannot access channel {Formatter.Bold(channel.Name)}.";
            if (member.PermissionsIn(channel).HasPermission(Permissions.AccessChannels))
                perms = member.PermissionsIn(channel).ToPermissionString();
            var output = new DiscordEmbedBuilder()
                .WithTitle($"Permissions for member {member.Username} in channel #{channel.Name}:")
                .WithDescription(perms)
                .WithColor(DiscordColor.Turquoise);
            await ctx.RespondAsync(embed: output.Build());
        }

        [Command("poll")]
        [Description("Run a poll with reactions.")]
        [Cooldown(2, 5, CooldownBucketType.User)]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task Poll(CommandContext ctx, string time, params DiscordEmoji[] options)
        {
            try
            {
                if (int.TryParse(time, out var minutes))
                {
                    var interactivity = ctx.Client.GetInteractivity();
                    var pollOptions = options.Select(xe => xe.ToString());
                    var duration = new TimeSpan(0, 0, minutes, 0, 0);
                    var output = new DiscordEmbedBuilder()
                        .WithTitle("Poll Time!")
                        .WithDescription(string.Join(" **VS.** ", pollOptions));
                    var message = await ctx.RespondAsync(embed: output.Build());
                    foreach (var t in options)
                        await message.CreateReactionAsync(t);
                    var pollResult = await interactivity.CollectReactionsAsync(message, duration);
                    var results = pollResult.Reactions.Where(xkvp => options.Contains(xkvp.Key)).Select(xkvp => $"{xkvp.Key} wins the poll with **{xkvp.Value}** votes");
                    await ctx.RespondAsync(string.Join("\n", results));
                }
                else
                    await BotServices.SendErrorEmbedAsync(ctx, ":warning: Invalid number of days, try **.poll 5** :open_mouth: :smile:");
            }
            catch
            {
                await BotServices.SendErrorEmbedAsync(ctx, ":warning: Unable to create the poll, please use stock Discord emojis as options!");
            }
        }

        [Command("role")]
        [Aliases("rid")]
        [Description("Retrieve role information")]
        [Cooldown(2, 5, CooldownBucketType.User)]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task GetRole(CommandContext ctx, [RemainingText] string roleName)
        {
            var role = ctx.Guild.Roles.FirstOrDefault(r => r.Name.ToLowerInvariant() == roleName);
            if (role != null)
            {
                await ctx.TriggerTypingAsync();
                var output = new DiscordEmbedBuilder()
                    .WithTitle($"{role.Name} (ID: {role.Id}")
                    .WithDescription($"Created on {role.CreationTimestamp.DateTime.ToString(CultureInfo.InvariantCulture)}")
                    .AddField("Permissions", role.Permissions.ToPermissionString())
                    .AddField("Managed", role.IsManaged ? "YES" : "NO", true)
                    .AddField("Hoisted", role.IsHoisted ? "YES" : "NO", true)
                    .AddField("Mentionable", role.IsMentionable ? "YES" : "NO", true)
                    .WithThumbnailUrl(ctx.Guild.IconUrl)
                    .WithFooter($"{ctx.Guild.Name} / #{ctx.Channel.Name} / {DateTime.Now}")
                    .WithColor(role.Color);
                await ctx.RespondAsync(embed: output.Build());
            }
        }

        [Command("server")]
        [Aliases("sid")]
        [Description("Retrieve server information")]
        [Cooldown(2, 5, CooldownBucketType.User)]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task GetServer(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            var channels = new StringBuilder();
            var roles = new StringBuilder();
            var emojis = new StringBuilder();
            var output = new DiscordEmbedBuilder()
                .WithAuthor($"Owner: {ctx.Guild.Owner.Username}#{ctx.Guild.Owner.Discriminator}", icon_url: string.IsNullOrEmpty(ctx.Guild.Owner.AvatarHash) ? null : ctx.Guild.Owner.AvatarUrl)
                .WithTitle($"{ctx.Guild.Name}")
                .AddField("ID", ctx.Guild.Id.ToString(), true)
                .AddField("Created on", ctx.Guild.CreationTimestamp.DateTime.ToString(CultureInfo.InvariantCulture), true)
                .WithFooter($"{ctx.Guild.Name} / #{ctx.Channel.Name} / {DateTime.Now}")
                .WithColor(DiscordColor.Rose);
            if (!string.IsNullOrEmpty(ctx.Guild.IconHash))
                output.WithThumbnailUrl(ctx.Guild.IconUrl);

            foreach (var channel in ctx.Guild.Channels)
                switch (channel.Type)
                {
                    case ChannelType.Text:
                        channels.Append($"`[#{channel.Name}]`");
                        break;

                    case ChannelType.Voice:
                        channels.Append($"`[{channel.Name}]`");
                        break;

                    case ChannelType.Category:
                        channels.Append($"`\n[{channel.Name.ToUpper()}]`\n");
                        break;

                    case ChannelType.Private:
                        break;

                    case ChannelType.Group:
                        break;

                    case ChannelType.Unknown:
                        break;

                    default:
                        channels.Append($"`\n[{channel.Name}]`\n");
                        break;
                }
            if (channels.Length == 0) channels.Append("None");
            output.AddField("Channels", channels.ToString());
            foreach (var role in ctx.Guild.Roles)
                roles.Append($"[`{role.Name}`]");
            foreach (var emoji in ctx.Guild.Emojis)
                emojis.Append(emoji.Name);
            if (roles.Length == 0) roles.Append("None");
            output.AddField("Roles", roles.ToString());
            output.AddField("Member Count", ctx.Guild.MemberCount.ToString(), true);
            output.AddField("Region", ctx.Guild.VoiceRegion.Name.ToUpperInvariant(), true);
            output.AddField("Authentication", ctx.Guild.MfaLevel.ToString(), true);
            output.AddField("Content Filter", ctx.Guild.ExplicitContentFilter.ToString(), true);
            output.AddField("Verification", ctx.Guild.VerificationLevel.ToString(), true);
            if (emojis.Length != 0) output.AddField("Emojis", emojis.ToString(), true);
            await ctx.RespondAsync(embed: output.Build());
        }

        [Command("setname")]
        [Aliases("sn")]
        [Description("Set a name for the current channel")]
        [RequirePermissions(Permissions.ManageChannels)]
        [Cooldown(2, 5, CooldownBucketType.User)]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task SetChannelName(CommandContext ctx, [RemainingText] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                await BotServices.SendErrorEmbedAsync(ctx, ":warning: Channel name cannot be blank!");
            else
            {
                await ctx.TriggerTypingAsync();
                await ctx.Channel.ModifyAsync(chn => chn.Name = name.Trim().Replace(" ", "-"));
                await ctx.RespondAsync($"Channel name has been changed to **{name.Trim().Replace(" ", "-")}**");
            }
        }

        [Command("setnickname")]
        [Aliases("setnick")]
        [Description("Set server member's nickname")]
        [RequireUserPermissions(Permissions.ChangeNickname)]
        [Cooldown(2, 5, CooldownBucketType.User)]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task SetUserName(CommandContext ctx, DiscordMember member, [RemainingText] string name)
        {
            await ctx.TriggerTypingAsync();
            var nickname = member.DisplayName;
            await member.ModifyAsync(usr => usr.Nickname = $"{name}");
            await ctx.RespondAsync($"{nickname}'s nickname has been changed to **{name}**");
        }

        [Command("setrole")]
        [Aliases("sr", "addrole")]
        [Description("Set a role for mentioned user")]
        [RequirePermissions(Permissions.ManageRoles)]
        [Cooldown(2, 5, CooldownBucketType.User)]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task SetUserRole(CommandContext ctx, DiscordMember member, [RemainingText] DiscordRole role)
        {
            if (member == null)
                member = ctx.Member;
            await ctx.TriggerTypingAsync();
            await member.GrantRoleAsync(role);
            await ctx.RespondAsync($"{member.DisplayName} been granted the role **{role.Name}**");
        }

        [Command("setserveravatar")]
        [Description("Set server avatar")]
        [RequireUserPermissions(Permissions.ManageGuild)]
        [Cooldown(2, 5, CooldownBucketType.User)]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task SetServerAvatar(CommandContext ctx, string query)
        {
            if (!Uri.TryCreate(query, UriKind.Absolute, out var uriResult) && (!query.EndsWith(".img") || !query.EndsWith(".png") || !query.EndsWith(".jpg")))
                await BotServices.SendErrorEmbedAsync(ctx, ":warning: An image URL ending with .img, .png or .jpg is required!");
            else
            {
                await ctx.TriggerTypingAsync();
                using (var client = new WebClient())
                {
                    var stream = new MemoryStream();
                    var data = client.DownloadData(query);
                    stream.Write(data, 0, data.Length);
                    stream.Position = 0;
                    await ctx.Guild.ModifyAsync(chn => chn.Icon = stream);
                    await ctx.RespondAsync($"{ctx.Guild.Name} server avatar has been updated!");
                }
            }
        }

        [Command("setservername")]
        [Description("Set server name")]
        [RequireUserPermissions(Permissions.ManageGuild)]
        [Cooldown(2, 5, CooldownBucketType.User)]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task SetServerName(CommandContext ctx, [RemainingText] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                await BotServices.SendErrorEmbedAsync(ctx, ":warning: Server name cannot be blank!");
            else
            {
                await ctx.TriggerTypingAsync();
                await ctx.Guild.ModifyAsync(srv => srv.Name = $"{name}");
                await ctx.RespondAsync($"Server name has been changed to **{name}**");
            }
        }

        [Command("settopic")]
        [Aliases("st")]
        [Description("Set a topic for the current channel")]
        [RequirePermissions(Permissions.ManageChannels)]
        [Cooldown(2, 5, CooldownBucketType.User)]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task SetChannelTopic(CommandContext ctx, [RemainingText] string topic)
        {
            await ctx.TriggerTypingAsync();
            await ctx.Channel.ModifyAsync(chn => chn.Topic = $"{topic}");
            await ctx.RespondAsync("Channel topic has been updated!");
        }

        [Command("showrole")]
        [Aliases("dr")]
        [Description("Toggles whether this role is displayed in the sidebar or not")]
        [RequirePermissions(Permissions.ManageRoles)]
        [Cooldown(2, 5, CooldownBucketType.User)]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task SidebarRole(CommandContext ctx, [RemainingText] DiscordRole role)
        {
            if (role == null) return;
            await ctx.TriggerTypingAsync();
            if (role.IsHoisted)
            {
                await role.UpdateAsync(hoist: false);
                await ctx.RespondAsync($"Role {role.Name} is now **hidden**");
            }
            else
            {
                await role.UpdateAsync(hoist: true);
                await ctx.RespondAsync($"Role {role.Name} is now **displayed**");
            }
        }

        [Command("user")]
        [Aliases("uid")]
        [Description("Retrieve User Information")]
        [Cooldown(2, 5, CooldownBucketType.User)]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task GetUser(CommandContext ctx, [RemainingText] DiscordMember member)
        {
            if (member == null)
                member = ctx.Member;
            await ctx.TriggerTypingAsync();
            var roles = new StringBuilder();
            var permsobj = member.PermissionsIn(ctx.Channel);
            var perms = permsobj.ToPermissionString();
            var output = new DiscordEmbedBuilder()
                .WithTitle($"@{member.Username}#{member.Discriminator}")
                .WithDescription("Nickname: ")
                .AddField("ID", member.Id.ToString(), true)
                .AddField("Registered on", member.CreationTimestamp.DateTime.ToString(CultureInfo.InvariantCulture), true)
                .AddField("Joined on", member.JoinedAt.DateTime.ToString(CultureInfo.InvariantCulture), true)
                .AddField("Muted?", member.IsMuted ? "YES" : "NO", true)
                .AddField("Deafened?", member.IsDeafened ? "YES" : "NO", true)
                .WithThumbnailUrl(member.AvatarUrl)
                .WithFooter($"{ctx.Guild.Name} / #{ctx.Channel.Name} / {DateTime.Now}")
                .WithColor(member.Color);
            if (member.IsBot)
                output.Title += " __[BOT]__ ";
            if (member.IsOwner)
                output.Title += " __[OWNER]__ ";
            output.AddField("Verified?", member.Verified == true ? "YES" : "NO", true);
            output.AddField("Secured?", member.MfaEnabled == true ? "YES" : "NO", true);
            if (!string.IsNullOrWhiteSpace(member.Nickname))
                output.Description += member.Nickname;
            foreach (var role in member.Roles)
                roles.Append($"[`{role.Name}`] ");
            if (roles.Length == 0)
                roles.Append("*None*");
            output.AddField("Roles", roles.ToString(), true);
            if (((permsobj & Permissions.Administrator) | (permsobj & Permissions.AccessChannels)) == 0)
                perms = $"**This user cannot see this channel!**\n{perms}";
            if (string.IsNullOrWhiteSpace(perms))
                perms = "*None*";
            output.AddField("Permissions", perms);
            await ctx.RespondAsync(embed: output.Build());
        }
    }
}