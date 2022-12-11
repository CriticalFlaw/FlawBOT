using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using FlawBOT.Common;
using FlawBOT.Properties;
using FlawBOT.Services;
using System.Linq;
using System.Threading.Tasks;

namespace FlawBOT.Modules.Misc
{
    public class MathModule : ApplicationCommandModule
    {
        #region COMMAND_MATH

        [SlashCommand("math", "Perform a basic math operation.")]
        public async Task Math(InteractionContext ctx,
            [Option("query", "First operand.")] double num1,
            [Option("query", "Operator")] string operation,
            [Option("query", "Second operand.")] double num2)
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
                await ctx.CreateResponseAsync(output.Build()).ConfigureAwait(false);
            }
            catch
            {
                await BotServices.SendResponseAsync(ctx, Resources.ERR_MATH_EQUATION, ResponseType.Warning).ConfigureAwait(false);
            }
        }

        #endregion COMMAND_MATH

        #region COMMAND_SUM

        [SlashCommand("sum", "Calculate the sum of all inputted values.")]
        public async Task Sum(InteractionContext ctx, [Option("args", "Numbers to sum up.")] params int[] args)
        {
            var output = new DiscordEmbedBuilder()
                .WithDescription($":1234: The sum is {args.Sum():#,##0}")
                .WithColor(DiscordColor.CornflowerBlue);
            await ctx.CreateResponseAsync(output.Build()).ConfigureAwait(false);
        }

        #endregion COMMAND_SUM
    }
}