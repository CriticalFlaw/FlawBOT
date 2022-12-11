using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using FlawBOT.Properties;
using FlawBOT.Services;
using System;
using System.Threading.Tasks;

namespace FlawBOT.Modules.Bot
{
    public class SlashCommands : ApplicationCommandModule
    {
        [SlashCommand("test", "A slash command made to test the DSharpPlus Slash Commands extension!")]
        public async Task TestCommand(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Success!"));
        }
    }
}