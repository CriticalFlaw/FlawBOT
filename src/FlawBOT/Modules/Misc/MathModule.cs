using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using FlawBOT.Common;
using FlawBOT.Properties;
using FlawBOT.Services;
using System.Linq;
using System.Threading.Tasks;

namespace FlawBOT.Modules.Misc
{
    [Cooldown(3, 5, CooldownBucketType.Channel)]
    public class MathModule : BaseCommandModule
    {
        #region COMMAND_MATH

        [Command("math")]
        [Aliases("calculate")]
        [Description("Perform a basic math operation.")]
        public async Task Math(CommandContext ctx,
            [Description("First operand.")] double num1,
            [Description("Operator.")] string operation,
            [Description("Second operand.")] double num2)
        {
            try
            {
                var result = operation switch
                {
                    "-" => num1 - num2,
                    "*" or "x" => num1 * num2,
                    "/" => num1 / num2,
                    "%" => num1 % num2,
                    _ => num1 + num2
                };
                var output = new DiscordEmbedBuilder()
                    .WithDescription($":1234: The answer is {result:#,##0.00}")
                    .WithColor(DiscordColor.CornflowerBlue);
                await ctx.RespondAsync(output.Build()).ConfigureAwait(false);
            }
            catch
            {
                await BotServices.SendResponseAsync(ctx, Resources.ERR_MATH_EQUATION, ResponseType.Warning)
                    .ConfigureAwait(false);
            }
        }

        #endregion COMMAND_MATH

        #region COMMAND_SUM

        [Command("sum")]
        [Aliases("total")]
        [Description("Calculate the sum of all inputted values")]
        public async Task Sum(CommandContext ctx,
            [Description("Numbers to sum up.")] params int[] args)
        {
            var output = new DiscordEmbedBuilder()
                .WithDescription($":1234: The sum is {args.Sum():#,##0}")
                .WithColor(DiscordColor.CornflowerBlue);
            await ctx.RespondAsync(output.Build()).ConfigureAwait(false);
        }

        #endregion COMMAND_SUM
    }
}