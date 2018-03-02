using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    public class ServerModule
    {
        [Command("channel")]
        [Aliases("cid")]
        [Description("Get channel information")]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task GetChannelID(CommandContext CTX, [RemainingText] DiscordChannel channel)
        {
            if (channel == null)
                channel = CTX.Channel;
            DiscordInvite invite = await CTX.Channel.CreateInviteAsync();
            var output = new DiscordEmbedBuilder()
                .WithTitle($"#{channel.Name} (ID: {channel.Id})")
                .WithDescription($"Created on: {channel.CreationTimestamp.DateTime.ToString(CultureInfo.InvariantCulture)}")
                .AddField("Type", channel.Type.ToString(), true)
                .AddField("Private?", channel.IsPrivate ? "YES" : "NO", true)
                .AddField("NSFW?", channel.IsNSFW ? "YES" : "NO", true)
                .WithThumbnailUrl(CTX.Guild.IconUrl)
                .WithFooter($"{CTX.Guild.Name} / #{CTX.Channel.Name} / {DateTime.Now}")
                .WithUrl($"https://discord.gg/{invite.Code}");
            if (!string.IsNullOrWhiteSpace(channel.Topic)) output.AddField("Topic", channel.Topic, true);
            await CTX.RespondAsync(embed: output.Build());
        }

        [Command("clean")]
        [Description("Remove server messages")]
        [RequirePermissions(Permissions.ManageMessages)]
        [Cooldown(3, 30, CooldownBucketType.User)]
        public async Task BotClean(CommandContext CTX, int limit, [RemainingText] DiscordChannel channel)
        {
            if (limit <= 0 || limit > 100)
                await CTX.RespondAsync(":warning: Invalid number of messages to delete (must be in range 1-100");
            else
            {
                await CTX.TriggerTypingAsync();
                if (channel == null)
                {
                    var MSGS = await CTX.Channel.GetMessagesAsync(limit).ConfigureAwait(false);
                    await CTX.Channel.DeleteMessagesAsync(MSGS).ConfigureAwait(false);
                    await CTX.RespondAsync($"**{MSGS.Count}** message(s) have been removed from #{CTX.Channel.Name}");
                }
                else
                {
                    var MSGS = await channel.GetMessagesAsync(limit).ConfigureAwait(false);
                    await channel.DeleteMessagesAsync(MSGS).ConfigureAwait(false);
                    await CTX.RespondAsync($"**{MSGS.Count}** message(s) have been removed from #{channel.Name}");
                }
            }
        }

        [Command("colorrole")]
        [Aliases("clrr")]
        [Description("Set a role's color to HEX color values")]
        [RequirePermissions(Permissions.ManageRoles)]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task ColorRole(CommandContext CTX, params string[] colors)
        {
            if (colors.Length != 2 && colors.Length != 4)
                await CTX.RespondAsync(":warning: Invalid parameters, try **.colorrole admin [0-255] [0-255] [0-255]**");
            else
            {
                try
                {
                    await CTX.TriggerTypingAsync();
                    var roleName = colors[0].ToLowerInvariant();
                    // REUSE THIS FOR OTHER DISCORD-RELATED EXCEPTIONS
                    var role = CTX.Guild.Roles.FirstOrDefault(r => r.Name.ToLowerInvariant() == roleName);
                    if (role == null)
                        await CTX.RespondAsync(":warning: This role does not exist in the server! :warning:");
                    else
                    {
                        var RGB = colors.Length == 4;
                        var ARG = colors[1].Replace("#", "");
                        DiscordColor color = new DiscordColor(Convert.ToByte(RGB ? int.Parse(ARG) : Convert.ToInt32(ARG.Substring(0, 2), 16)), Convert.ToByte(RGB ? int.Parse(colors[2]) : Convert.ToInt32(ARG.Substring(2, 2), 16)), Convert.ToByte(RGB ? int.Parse(colors[3]) : Convert.ToInt32(ARG.Substring(4, 2), 16)));
                        await CTX.Guild.UpdateRoleAsync(role: role, color: color).ConfigureAwait(false);
                        await CTX.RespondAsync($"Color of the role **{roleName}** has been changed to **{color.ToString()}**");
                    }
                }
                catch
                {
                    await CTX.RespondAsync(":warning: Unable to change role color values, try **.colorrole admin [0-255] [0-255] [0-255]** :warning:");
                }
            }
        }

        [Command("createrole")]
        [Aliases("csr")]
        [Description("Create a server role")]
        [RequirePermissions(Permissions.ManageRoles)]
        [Cooldown(1, 5, CooldownBucketType.Guild)]
        public async Task CreateRole(CommandContext CTX, [RemainingText] string role)
        {
            if (string.IsNullOrWhiteSpace(role))
                await CTX.RespondAsync(":warning: Role name cannot be blank! :warning:");
            else
            {
                await CTX.TriggerTypingAsync();
                await CTX.Guild.CreateRoleAsync(role);
                await CTX.RespondAsync($"Role **{role}** has been **created**");
            }
        }

        [Command("createtext")]
        [Aliases("ctc")]
        [Description("Create a text channel")]
        [RequirePermissions(Permissions.ManageChannels)]
        [Cooldown(1, 5, CooldownBucketType.Guild)]
        public async Task CreateTextChannel(CommandContext CTX, [RemainingText] string channel)
        {
            if (string.IsNullOrWhiteSpace(channel))
                await CTX.RespondAsync(":warning: Text channel name cannot be blank! :warning:");
            else
            {
                await CTX.TriggerTypingAsync();
                await CTX.Guild.CreateChannelAsync(channel.Trim().Replace(" ", "-"), ChannelType.Text, parent: CTX.Channel.Parent);
                await CTX.RespondAsync($"Text Channel **#{channel.Trim().Replace(" ", "-")}** has been **created**");
            }
        }

        [Command("createvoice")]
        [Aliases("cvc")]
        [Description("Create a voice channel")]
        [RequirePermissions(Permissions.ManageChannels)]
        [Cooldown(1, 5, CooldownBucketType.Guild)]
        public async Task CreateVoiceChannel(CommandContext CTX, [RemainingText] string channel)
        {
            if (string.IsNullOrWhiteSpace(channel))
                await CTX.RespondAsync(":warning: Text channel name cannot be blank! :warning:");
            else
            {
                await CTX.TriggerTypingAsync();
                await CTX.Guild.CreateChannelAsync(channel.Trim().Replace(" ", "-"), ChannelType.Voice, parent: CTX.Channel.Parent);
                await CTX.RespondAsync($"Voice Channel **#{channel.Trim().Replace(" ", "-")}** has been **created**");
            }
        }

        [Command("deleterole")]
        [Aliases("dsr")]
        [Description("Delete a server role")]
        [RequirePermissions(Permissions.ManageRoles)]
        [Cooldown(1, 5, CooldownBucketType.Guild)]
        public async Task DeleteRole(CommandContext CTX, [RemainingText] DiscordRole role)
        {
            if (role == null)
                await CTX.RespondAsync(":warning: That role does not exist! :warning:");
            else
            {
                await CTX.TriggerTypingAsync();
                await CTX.Guild.DeleteRoleAsync(role);
                await CTX.RespondAsync($"Role **{role.Name}** has been **removed**");
            }
        }

        [Command("deletetext")]
        [Aliases("dtc")]
        [Description("Remove a text channel")]
        [RequirePermissions(Permissions.ManageChannels)]
        [Cooldown(1, 5, CooldownBucketType.Guild)]
        public async Task RemoveTextChannel(CommandContext CTX, [RemainingText] DiscordChannel channel)
        {
            if (channel == null)
                await CTX.RespondAsync(":warning: That text channel does not exist! :warning:");
            else if (channel.Type != ChannelType.Text)
                await CTX.RespondAsync("This is not a text channel, use **.deletevoice** instead.");
            else
            {
                await CTX.TriggerTypingAsync();
                await channel.DeleteAsync();
                await CTX.RespondAsync($"Text Channel **{channel.Name}** has been **removed**");
            }
        }

        [Command("deletevoice")]
        [Aliases("dvc")]
        [Description("Remove a voice channel")]
        [RequirePermissions(Permissions.ManageChannels)]
        [Cooldown(1, 5, CooldownBucketType.Guild)]
        public async Task RemoveVoiceChannel(CommandContext CTX, [RemainingText] DiscordChannel channel)
        {
            if (channel == null)
                await CTX.RespondAsync(":warning: That voice channel does not exist! :warning:");
            else if (channel.Type != ChannelType.Voice)
                await CTX.RespondAsync("This is not a voice channel, use **.deletetext** instead.");
            else
            {
                await CTX.TriggerTypingAsync();
                await channel.DeleteAsync();
                await CTX.RespondAsync($"Voice Channel **{channel.Name}** has been **removed**");
            }
        }

        [Command("inrole")]
        [Description("Lists all users in specified role")]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task UsersInRole(CommandContext CTX, [RemainingText] DiscordRole role)
        {
            if (role != null)
            {
                await CTX.TriggerTypingAsync();
                var users = (await CTX.Guild.GetAllMembersAsync()).ToArray();
                int userCount = 0;
                string usersList = null;
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
                    await CTX.RespondAsync($"Role **{role.Name}** has no members");
                else
                    await CTX.RespondAsync($"Role **{role.Name}** has **{userCount}** member(s): {usersList}");
            }
        }

        [Command("invite")]
        [Aliases("inv")]
        [Description("Create an instant invite link for this server")]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task InviteAsync(CommandContext CTX)
        {
            DiscordInvite invite = await CTX.Channel.CreateInviteAsync();
            await CTX.RespondAsync($"Here is your instant invite link to **{CTX.Guild.Name}**: https://discord.gg/{invite.Code}");
        }

        [Command("leave")]
        [Description("Makes this bot leave the current server.")]
        [RequireUserPermissions(Permissions.Administrator)]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task LeaveAsync(CommandContext CTX)
        {
            await CTX.TriggerTypingAsync();
            var interactivity = CTX.Client.GetInteractivityModule();
            await CTX.RespondAsync("Are you sure you want to remove FlawBOT from this server?\nRespond with **yes** to proceed or wait 15 seconds to cancel this operation.");
            var response = await interactivity.WaitForMessageAsync(x => x.ChannelId == CTX.Channel.Id && x.Author.Id == CTX.Member.Id, TimeSpan.FromSeconds(15));
            if (response.Message.Content.ToUpper() == "YES")
            {
                await CTX.RespondAsync("Thank you for using FlawBOT...");
                await CTX.Guild.LeaveAsync();
            }
            else
                await CTX.RespondAsync("Request timed out...");
        }

        [Command("mentionrole")]
        [Aliases("mr")]
        [Description("Toggle whether this role can be mentioned by others")]
        [RequirePermissions(Permissions.ManageRoles)]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task MentionRole(CommandContext CTX, [RemainingText] DiscordRole role)
        {
            if (role != null)
            {
                if (role.IsMentionable)
                {
                    await CTX.Guild.UpdateRoleAsync(role, mentionable: false);
                    await CTX.RespondAsync($"Role **{role.Name}** is now **not-mentionable**");
                }
                else
                {
                    await CTX.Guild.UpdateRoleAsync(role, mentionable: true);
                    await CTX.RespondAsync($"Role **{role.Name}** is now **mentionable**");
                }
            }
        }

        [Command("perms")]
        [Aliases("prm")]
        [Description("Retrieve your server permissions")]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task ListServerPermissions(CommandContext CTX, DiscordMember member = null, [RemainingText] DiscordChannel channel = null)
        {
            if (member == null)
                member = CTX.Member;
            if (channel == null)
                channel = CTX.Channel;
            await CTX.TriggerTypingAsync();
            string perms = $"{Formatter.Bold(member.DisplayName)} cannot access channel {Formatter.Bold(channel.Name)}.";
            if (member.PermissionsIn(channel).HasPermission(Permissions.AccessChannels))
                perms = member.PermissionsIn(channel).ToPermissionString();
            var output = new DiscordEmbedBuilder()
                .WithTitle($"Permissions for member {member.Username} in channel #{channel.Name}:")
                .WithDescription(perms)
                .WithColor(DiscordColor.Turquoise);
            await CTX.RespondAsync(embed: output.Build());
        }

        [Command("poll")]
        [Description("Run a poll with reactions.")]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task Poll(CommandContext CTX, string time, params DiscordEmoji[] options)
        {
            if (int.TryParse(time, out int minutes))
            {
                var interactivity = CTX.Client.GetInteractivityModule();
                var poll_options = options.Select(xe => xe.ToString());
                TimeSpan duration = new TimeSpan(0, 0, minutes, 0, 0);
                var output = new DiscordEmbedBuilder()
                    .WithTitle("Poll Time!")
                    .WithDescription(string.Join(" **VS.** ", poll_options));
                var message = await CTX.RespondAsync(embed: output.Build());
                for (var i = 0; i < options.Length; i++)
                    await message.CreateReactionAsync(options[i]);
                var poll_result = await interactivity.CollectReactionsAsync(message, duration);
                var results = poll_result.Reactions.Where(xkvp => options.Contains(xkvp.Key)).Select(xkvp => $"{xkvp.Key} wins the poll with **{xkvp.Value}** votes");
                await CTX.RespondAsync(string.Join("\n", results));
            }
            else
                await CTX.RespondAsync(":warning: Please input a valid number of days like **.poll 5** :open_mouth: :smile: :warning:");
        }

        [Command("prune")]
        [Description("Prune inactive server members")]
        [RequirePermissions(Permissions.DeafenMembers)]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task PruneUsers(CommandContext CTX, string day)
        {
            if (int.TryParse(day, out int days))
            {
                await CTX.TriggerTypingAsync();
                await CTX.RespondAsync($"**{CTX.Guild.GetPruneCountAsync(days).Result}** server members have been pruned.");
                await CTX.Guild.PruneAsync(days);
            }
            else
                await CTX.RespondAsync(":warning: Please input a valid number of days, try **.prune 30** :warning:");
        }

        [Command("removerole")]
        [Aliases("rr")]
        [Description("Remove a role from mentioned user")]
        [RequirePermissions(Permissions.ManageRoles)]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task RemoveUserRole(CommandContext CTX, DiscordMember member, [RemainingText] DiscordRole role)
        {
            if (member != null && role != null)
            {
                await CTX.TriggerTypingAsync();
                await member.RevokeRoleAsync(role);
                await CTX.RespondAsync($"{member.DisplayName} has been revoked the role **{role.Name}**");
            }
        }

        [Command("removeroles")]
        [Aliases("rrs")]
        [Description("Remove all roles from mentioned user")]
        [RequirePermissions(Permissions.ManageRoles)]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task RemoveUserRoles(CommandContext CTX, DiscordMember member)
        {
            if (member.Roles.Max(r => r.Position) >= CTX.Member.Roles.Max(r => r.Position))
                await CTX.RespondAsync(":warning: You are not authorised to remove roles from this user! :warning:");
            else
            {
                await CTX.TriggerTypingAsync();
                await member.ReplaceRolesAsync(Enumerable.Empty<DiscordRole>()).ConfigureAwait(false);
                await CTX.RespondAsync($"Removed all roles from {member.DisplayName}");
            }
        }

        [Command("role")]
        [Aliases("rid")]
        [Description("Retrieve role information")]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task GetRole(CommandContext CTX, [RemainingText] DiscordRole role)
        {
            if (role != null)
            {
                await CTX.TriggerTypingAsync();
                var output = new DiscordEmbedBuilder()
                    .WithTitle($"{role.Name} (ID: {role.Id}")
                    .WithDescription($"Created on {role.CreationTimestamp.DateTime.ToString(CultureInfo.InvariantCulture)}")
                    .AddField("Permissions", role.Permissions.ToPermissionString())
                    .AddField("Managed", (role.IsManaged ? "YES" : "NO"), true)
                    .AddField("Hoisted", (role.IsHoisted ? "YES" : "NO"), true)
                    .AddField("Mentionable", (role.IsMentionable ? "YES" : "NO"), true)
                    .WithThumbnailUrl(CTX.Guild.IconUrl)
                    .WithFooter($"{CTX.Guild.Name} / #{CTX.Channel.Name} / {DateTime.Now}")
                    .WithColor(role.Color);
                await CTX.RespondAsync(embed: output.Build());
            }
        }

        [Command("server")]
        [Aliases("sid")]
        [Description("Retrieve server information")]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task GetServer(CommandContext CTX)
        {
            await CTX.TriggerTypingAsync();
            var channels = new StringBuilder();
            var roles = new StringBuilder();
            var output = new DiscordEmbedBuilder()
                .WithAuthor($"Owner: {CTX.Guild.Owner.Username}#{CTX.Guild.Owner.Discriminator}", icon_url: string.IsNullOrEmpty(CTX.Guild.Owner.AvatarHash) ? null : CTX.Guild.Owner.AvatarUrl)
                .WithTitle($"{CTX.Guild.Name} (ID: {CTX.Guild.Id})")
                .WithDescription($"Created on: {CTX.Guild.CreationTimestamp.DateTime.ToString(CultureInfo.InvariantCulture)}")
                .WithFooter($"{CTX.Guild.Name} / #{CTX.Channel.Name} / {DateTime.Now}")
                .WithColor(DiscordColor.Sienna);
            if (!string.IsNullOrEmpty(CTX.Guild.IconHash))
                output.WithThumbnailUrl(CTX.Guild.IconUrl);

            foreach (var channel in CTX.Guild.Channels)
            {
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

                    default:
                        channels.Append($"`\n[{channel.Name}]`\n");
                        break;
                }
            }
            if (channels.Length == 0) channels.Append("None");
            output.AddField("Channels", channels.ToString());
            foreach (var role in CTX.Guild.Roles)
                roles.Append($"[`{role.Name}`]");
            if (roles.Length == 0) roles.Append("None");
            output.AddField("Roles", roles.ToString());
            output.AddField("Member Count", CTX.Guild.MemberCount.ToString(), true);
            output.AddField("Region", CTX.Guild.RegionId.ToUpperInvariant(), true);
            output.AddField("Authentication", CTX.Guild.MfaLevel.ToString(), true);
            output.AddField("Content Filter", CTX.Guild.ExplicitContentFilter.ToString(), true);
            output.AddField("Verification", CTX.Guild.VerificationLevel.ToString(), true);
            await CTX.RespondAsync(embed: output.Build());
        }

        [Command("setname")]
        [Aliases("sn")]
        [Description("Set a name for the current channel")]
        [RequirePermissions(Permissions.ManageChannels)]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task SetChannelName(CommandContext CTX, [RemainingText] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                await CTX.RespondAsync(":warning: Channel name cannot be blank! :warning:");
            else
            {
                await CTX.TriggerTypingAsync();
                await CTX.Channel.ModifyAsync(name: name.Trim().Replace(" ", "-"));
                await CTX.RespondAsync($"Channel name has been changed to **{name.Trim().Replace(" ", "-")}**");
            }
        }

        [Command("setnickname")]
        [Aliases("setnick")]
        [Description("Set server member's nickname")]
        [RequireUserPermissions(Permissions.ChangeNickname)]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task SetNickname(CommandContext CTX, DiscordMember member, [RemainingText] string name)
        {
            if (member.Roles.Max(r => r.Position) >= CTX.Member.Roles.Max(r => r.Position))
                await CTX.RespondAsync(":warning: You are not authorised to nickname this user! :warning:");
            else if (string.IsNullOrWhiteSpace(name))
                await member.ModifyAsync(nickname: null);
            else
            {
                await CTX.TriggerTypingAsync();
                string nickname = member.DisplayName;
                await member.ModifyAsync(nickname: name);
                await CTX.RespondAsync($"{nickname}'s nickname has been changed to **{name}**");
            }
        }

        [Command("setrole")]
        [Aliases("sr")]
        [Description("Set a role for mentioned user")]
        [RequirePermissions(Permissions.ManageRoles)]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task SetUserRole(CommandContext CTX, DiscordMember member, [RemainingText] DiscordRole role)
        {
            await CTX.TriggerTypingAsync();
            await member.GrantRoleAsync(role);
            await CTX.RespondAsync($"{member.DisplayName} been granted the role **{role.Name}**");
        }

        [Command("setserveravatar")]
        [Description("Set server avatar")]
        [RequireUserPermissions(Permissions.ManageGuild)]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task SetServerAvatar(CommandContext CTX, string query)
        {
            if ((!Uri.TryCreate(query, UriKind.Absolute, out Uri uriResult) && (!query.EndsWith(".img") || !query.EndsWith(".png") || !query.EndsWith(".jpg"))))
                await CTX.RespondAsync(":warning: An image URL ending with .img, .png or .jpg is required! :warning:");
            else
            {
                await CTX.TriggerTypingAsync();
                using (WebClient client = new WebClient())
                {
                    using (MemoryStream stream = new MemoryStream())
                    {
                        var data = client.DownloadData(query);
                        stream.Write(data, 0, data.Length);
                        stream.Position = 0;
                        await CTX.Guild.ModifyAsync(icon: stream);
                        await CTX.RespondAsync($"{CTX.Guild.Name} server avatar has been updated!");
                    }
                }
            }
        }

        [Command("setservername")]
        [Description("Set server name")]
        [RequireUserPermissions(Permissions.ManageGuild)]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task SetServerName(CommandContext CTX, [RemainingText] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                await CTX.RespondAsync(":warning: Server name cannot be blank :warning:");
            else
            {
                await CTX.TriggerTypingAsync();
                await CTX.Guild.ModifyAsync(name: name.Trim());
                await CTX.RespondAsync($"Server name has been changed to **{name}**");
            }
        }

        [Command("settopic")]
        [Aliases("st")]
        [Description("Set a topic for the current channel")]
        [RequirePermissions(Permissions.ManageChannels)]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task SetChannelTopic(CommandContext CTX, [RemainingText] string topic)
        {
            await CTX.TriggerTypingAsync();
            if (string.IsNullOrWhiteSpace(topic))
                await CTX.Channel.ModifyAsync(topic: "");
            else
                await CTX.Channel.ModifyAsync(topic: topic);
            await CTX.RespondAsync("Channel topic has been updated");
        }

        [Command("showrole")]
        [Aliases("dr")]
        [Description("Toggles whether this role is displayed in the sidebar or not")]
        [RequirePermissions(Permissions.ManageRoles)]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task SidebarRole(CommandContext CTX, [RemainingText] DiscordRole role)
        {
            await CTX.TriggerTypingAsync();
            if (role.IsHoisted)
            {
                await CTX.Guild.UpdateRoleAsync(role, hoist: false);
                await CTX.RespondAsync($"Role {role.Name} is now **hidden**");
            }
            else
            {
                await CTX.Guild.UpdateRoleAsync(role, hoist: true);
                await CTX.RespondAsync($"Role {role.Name} is now **displayed**");
            }
        }

        [Command("user")]
        [Aliases("uid")]
        [Description("Retrieve User Information")]
        [Cooldown(3, 5, CooldownBucketType.Channel)]
        public async Task GetUser(CommandContext CTX, [RemainingText] DiscordMember member)
        {
            if (member == null)
                member = CTX.Member;
            await CTX.TriggerTypingAsync();
            var roles = new StringBuilder();
            var permsobj = member.PermissionsIn(CTX.Channel);
            var perms = permsobj.ToPermissionString();
            var output = new DiscordEmbedBuilder()
                .WithTitle($"@{member.Username}#{member.Discriminator} (ID: {member.Id.ToString()})")
                .WithDescription("Nickname: ")
                .AddField("Registered on", member.CreationTimestamp.DateTime.ToString(CultureInfo.InvariantCulture))
                .AddField("Joined on", member.JoinedAt.DateTime.ToString(CultureInfo.InvariantCulture))
                .AddField("Muted?", member.IsMuted ? "YES" : "NO", true)
                .AddField("Deafened?", member.IsDeafened ? "YES" : "NO", true)
                .WithThumbnailUrl(member.AvatarUrl)
                .WithFooter($"{CTX.Guild.Name} / #{CTX.Channel.Name} / {DateTime.Now}")
                .WithColor(member.Color);
            if (member.IsBot)
                output.Title += " __[BOT]__ ";
            if (member.IsOwner)
                output.Title += " __[OWNER]__ ";
            if (member.Verified == true)
                output.AddField("Verified?", "YES", true);
            else
                output.AddField("Verified?", "NO", true);
            if (member.MfaEnabled == true)
                output.AddField("Secured?", "YES", true);
            else output.AddField("Secured?", "NO", true);
            if (!string.IsNullOrWhiteSpace(member.Nickname))
                output.Description += (member.Nickname);
            foreach (var role in member.Roles)
                roles.Append($"[`{role.Name}`] ");
            if (roles.Length == 0)
                roles.Append("*None*");
            output.AddField("Roles", roles.ToString());
            if (((permsobj & Permissions.Administrator) | (permsobj & Permissions.AccessChannels)) == 0)
                perms = $"**This user cannot see this channel!**\n{perms}";
            if (perms == String.Empty)
                perms = "*None*";
            output.AddField("Permissions", perms);
            await CTX.RespondAsync(embed: output.Build());
        }
    }
}