using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Exceptions;
using FlawBOT.Services;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace FlawBOT.Common
{
    public class Exceptions
    {
        public static async Task Process(CommandErrorEventArgs e, EventId eventId)
        {
            switch (e.Exception)
            {
                case CommandNotFoundException:
                    await BotServices.SendResponseAsync(e.Context, e.Exception.Message, ResponseType.Missing).ConfigureAwait(false);
                    break;

                case InvalidOperationException:
                    await BotServices.SendResponseAsync(e.Context, e.Exception.Message, ResponseType.Warning).ConfigureAwait(false);
                    break;

                case ChecksFailedException cfe:
                    await BotServices.SendResponseAsync(e.Context, $"Command {Formatter.Bold(e.Command.QualifiedName)} could not be executed.", ResponseType.Error).ConfigureAwait(false);
                    foreach (var check in cfe.FailedChecks)
                        switch (check)
                        {
                            case RequirePermissionsAttribute perms:
                                await BotServices.SendResponseAsync(e.Context, $"- One of us does not have the required permissions ({perms.Permissions.ToPermissionString()})!", ResponseType.Error).ConfigureAwait(false);
                                break;

                            case RequireUserPermissionsAttribute perms:
                                await BotServices.SendResponseAsync(e.Context, $"- You do not have sufficient permissions ({perms.Permissions.ToPermissionString()})!", ResponseType.Error).ConfigureAwait(false);
                                break;

                            case RequireBotPermissionsAttribute perms:
                                await BotServices.SendResponseAsync(e.Context, $"- I do not have sufficient permissions ({perms.Permissions.ToPermissionString()})!", ResponseType.Error).ConfigureAwait(false);
                                break;

                            case RequireOwnerAttribute:
                                await BotServices.SendResponseAsync(e.Context, "- This command is reserved only for the bot owner.", ResponseType.Error) .ConfigureAwait(false);
                                break;

                            case RequirePrefixesAttribute pa:
                                await BotServices.SendResponseAsync(e.Context, $"- This command can only be invoked with the following prefixes: {string.Join(" ", pa.Prefixes)}.", ResponseType.Error).ConfigureAwait(false);
                                break;

                            default:
                                await BotServices.SendResponseAsync(e.Context, "Unknown check triggered. Please notify the developer using the command *.bot report*", ResponseType.Error).ConfigureAwait(false);
                                break;
                        }

                    break;

                case ArgumentNullException:
                case ArgumentException:
                    await BotServices.SendResponseAsync(e.Context, $"Invalid or missing parameters. For help, use command `.help {e.Command?.QualifiedName}`", ResponseType.Warning);
                    break;

                case UnauthorizedException:
                    await BotServices.SendResponseAsync(e.Context, "One of us does not have the required permissions.", ResponseType.Warning);
                    break;

                case NullReferenceException:
                case InvalidDataException:
                    e.Context.Client.Logger.LogWarning(eventId, e.Exception, $"[{e.Context.Guild.Name} : {e.Context.Channel.Name}] {e.Context.User.Username} executed the command '{e.Command?.QualifiedName ?? "<unknown>"}' but it threw an error: ");
                    await BotServices.SendResponseAsync(e.Context, e.Exception.Message, ResponseType.Error);
                    break;

                default:
                    e.Context.Client.Logger.LogError(eventId, $"[{e.Exception.GetType()}] Unhandled Exception. {e.Exception.Message}");
                    break;
            }
        }
    }
}