using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.VoiceNext;

namespace FlawBOT.Modules
{
    [Group("voice")]
    [Cooldown(3, 5, CooldownBucketType.Channel)]
    public class VoiceModule : BaseCommandModule
    {
        #region COMMAND_JOIN

        [Command("join"), Description("Joins a voice channel.")]
        public async Task JoinChannel(CommandContext ctx, DiscordChannel channel = null)
        {
            var voiceExt = ctx.Client.GetVoiceNext();
            // Check that voice extension is enabled.
            if (voiceExt == null)
            {
                await ctx.RespondAsync("Voice extension is not enabled or configured.").ConfigureAwait(false);
                return;
            }

            // Check that the client isn't already connected.
            if (voiceExt.GetConnection(ctx.Guild) != null)
            {
                await ctx.RespondAsync("Already connected to a voice channel.").ConfigureAwait(false);
                return;
            }

            var voiceState = ctx.Member?.VoiceState;
            // Check that the sender is in a voice channel
            if (voiceState?.Channel == null && channel == null)
            {
                await ctx.RespondAsync("You are not in a voice channel.").ConfigureAwait(false);
                return;
            }

            // Use sender's voice channel if one was not provided.
            channel ??= voiceState.Channel;

            // Connect to the voice channel.
            await voiceExt.ConnectAsync(channel).ConfigureAwait(false);
            await ctx.RespondAsync($"Connected to `{channel.Name}`").ConfigureAwait(false);
        }

        #endregion COMMAND_JOIN

        #region COMMAND_LEAVE

        [Command("leave"), Description("Leaves a voice channel.")]
        public async Task LeaveChannel(CommandContext ctx)
        {
            var voiceExt = ctx.Client.GetVoiceNext();
            // Check that voice extension is enabled.
            if (voiceExt == null)
            {
                await ctx.RespondAsync("Voice extension is not enabled or configured.").ConfigureAwait(false);
                return;
            }

            // Check that the client isn't already connected.
            if (voiceExt.GetConnection(ctx.Guild) != null)
            {
                await ctx.RespondAsync("Already connected to a voice channel.").ConfigureAwait(false);
                return;
            }

            // Disconnect from the voice channel.
            voiceExt.GetConnection(ctx.Guild).Disconnect();
            await ctx.RespondAsync("Disconnected from the voice channel.").ConfigureAwait(false);
        }

        #endregion COMMAND_LEAVE
    }
}