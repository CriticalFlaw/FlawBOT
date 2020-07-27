using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using FlawBOT.Core.Properties;
using FlawBOT.Framework.Models;
using FlawBOT.Framework.Services;

namespace FlawBOT.Modules
{
    [Cooldown(3, 5, CooldownBucketType.Channel)]
    public class MathModule : BaseCommandModule
    {
        #region COMMAND_MATH

        [Command("math")]
        [Aliases("calculate")]
        [Description("Perform a basic math operation")]
        public async Task Math(CommandContext ctx,
            [Description("First operand")] double num1,
            [Description("Operator")] string operation,
            [Description("Second operand")] double num2)
        {
            try
            {
                double result;
                switch (operation)
                {
                    default: //case "+":
                        result = num1 + num2;
                        break;

                    case "-":
                        result = num1 - num2;
                        break;

                    case "*":
                    case "x":
                        result = num1 * num2;
                        break;

                    case "/":
                        result = num1 / num2;
                        break;

                    case "%":
                        result = num1 % num2;
                        break;
                }

                var output = new DiscordEmbedBuilder()
                    .WithDescription($":1234: The result is {result:#,##0.00}")
                    .WithColor(DiscordColor.CornflowerBlue);
                await ctx.RespondAsync(embed: output.Build()).ConfigureAwait(false);
            }
            catch
            {
                await BotServices.SendEmbedAsync(ctx, Resources.ERR_MATH_EQUATION, EmbedType.Warning)
                    .ConfigureAwait(false);
            }
        }

        #endregion COMMAND_MATH

        #region COMMAND_SUM

        [Command("sum")]
        [Aliases("total")]
        [Description("Calculate the sum of all inputted values")]
        public async Task Sum(CommandContext ctx,
            [Description("Numbers to sum up")] params int[] args)
        {
            var output = new DiscordEmbedBuilder()
                .WithDescription($":1234: The sum is {args.Sum():#,##0}")
                .WithColor(DiscordColor.CornflowerBlue);
            await ctx.RespondAsync(embed: output.Build()).ConfigureAwait(false);
        }

        #endregion COMMAND_SUM
    }
}