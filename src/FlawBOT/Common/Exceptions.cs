using DSharpPlus;
using DSharpPlus.Exceptions;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using DSharpPlus.SlashCommands.EventArgs;
using FlawBOT.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlawBOT.Common
{
    public static class Exceptions
    {
        public static async Task Process(SlashCommandErrorEventArgs e, EventId eventId)
        {
            var ctx = e.Context;
            var (message, type) = Describe(e.Exception, ctx.QualifiedName);

            if (type == ResponseType.Error)
                ctx.Client.Logger.LogError(eventId, e.Exception, "[{Guild} : {Channel}] {User} executed the command '{Command}' but it threw an error.",
                    ctx.Guild?.Name ?? "DM", ctx.Channel?.Name, ctx.User.Username, ctx.QualifiedName);

            try
            {
                await BotServices.SendResponseAsync(ctx, message, type).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                // The interaction may have expired or the channel may be gone. Nothing else can be done for the user.
                ctx.Client.Logger.LogWarning(eventId, ex, "Unable to report a command error to the user.");
            }
        }

        /// <summary>
        ///     Converts a command exception into a user-facing message.
        /// </summary>
        public static (string Message, ResponseType Type) Describe(Exception exception, string commandName)
        {
            switch (exception)
            {
                case SlashExecutionChecksFailedException cfe:
                    var reasons = new List<string> { $"Command {Formatter.Bold(commandName)} could not be executed." };
                    reasons.AddRange(cfe.FailedChecks.Select(DescribeCheck));
                    return (string.Join("\n", reasons), ResponseType.Error);
                case UnauthorizedException:
                    return ("One of us does not have the required permissions.", ResponseType.Warning);
                case NotFoundException:
                    return ("The requested Discord resource could not be found.", ResponseType.Missing);
                case ArgumentException:
                    return ($"Invalid or missing parameters for {Formatter.InlineCode("/" + commandName)}.", ResponseType.Warning);
                case InvalidOperationException:
                    return (exception.Message, ResponseType.Warning);
                default:
                    return ("Something went wrong while running this command. Please try again later.", ResponseType.Error);
            }
        }

        private static string DescribeCheck(SlashCheckBaseAttribute check)
        {
            return check switch
            {
                SlashRequireUserPermissionsAttribute perms => $"- You do not have sufficient permissions ({perms.Permissions.ToPermissionString()})!",
                SlashRequireBotPermissionsAttribute perms => $"- I do not have sufficient permissions ({perms.Permissions.ToPermissionString()})!",
                SlashRequirePermissionsAttribute perms => $"- Both you and I need the following permissions: {perms.Permissions.ToPermissionString()}.",
                SlashRequireOwnerAttribute => "- This command is reserved only for the bot owner.",
                SlashRequireGuildAttribute => "- This command can only be used in a server.",
                _ => "- A command check failed."
            };
        }
    }
}
