using System;
using System.IO;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Exceptions;
using FlawBOT.Framework.Models;
using FlawBOT.Framework.Services;
using Microsoft.Extensions.Logging;

namespace FlawBOT.Common
{
    public class Exceptions
    {
        public static async Task Process(CommandErrorEventArgs e, EventId eventId)
        {
            switch (e.Exception)
            {
                case CommandNotFoundException _:
                    await BotServices.SendEmbedAsync(e.Context, e.Exception.Message, EmbedType.Missing);
                    break;

                case InvalidOperationException _:
                    await BotServices.SendEmbedAsync(e.Context, e.Exception.Message, EmbedType.Warning);
                    break;

                case ChecksFailedException cfe:
                    await BotServices.SendEmbedAsync(e.Context,
                            $"Command **{e.Command.QualifiedName}** could not be executed.", EmbedType.Error)
                        .ConfigureAwait(false);
                    foreach (var check in cfe.FailedChecks)
                        switch (check)
                        {
                            case RequirePermissionsAttribute perms:
                                await BotServices.SendEmbedAsync(e.Context,
                                    $"- One of us does not have the required permissions ({perms.Permissions.ToPermissionString()})!",
                                    EmbedType.Error).ConfigureAwait(false);
                                break;

                            case RequireUserPermissionsAttribute perms:
                                await BotServices.SendEmbedAsync(e.Context,
                                    $"- You do not have sufficient permissions ({perms.Permissions.ToPermissionString()})!",
                                    EmbedType.Error).ConfigureAwait(false);
                                break;

                            case RequireBotPermissionsAttribute perms:
                                await BotServices.SendEmbedAsync(e.Context,
                                    $"- I do not have sufficient permissions ({perms.Permissions.ToPermissionString()})!",
                                    EmbedType.Error).ConfigureAwait(false);
                                break;

                            case RequireOwnerAttribute _:
                                await BotServices.SendEmbedAsync(e.Context,
                                        "- This command is reserved only for the bot owner.", EmbedType.Error)
                                    .ConfigureAwait(false);
                                break;

                            case RequirePrefixesAttribute pa:
                                await BotServices.SendEmbedAsync(e.Context,
                                    $"- This command can only be invoked with the following prefixes: {string.Join(" ", pa.Prefixes)}.",
                                    EmbedType.Error).ConfigureAwait(false);
                                break;

                            default:
                                await BotServices.SendEmbedAsync(e.Context,
                                    "Unknown check triggered. Please notify the developer using the command *.bot report*",
                                    EmbedType.Error).ConfigureAwait(false);
                                break;
                        }

                    break;

                case ArgumentNullException _:
                case ArgumentException _:
                    await BotServices.SendEmbedAsync(e.Context,
                        $"Invalid or missing parameters. For more info, use command `.help {e.Command?.QualifiedName}`",
                        EmbedType.Warning);
                    break;

                case NullReferenceException _:
                case InvalidDataException _:
                    e.Context.Client.Logger.LogWarning(eventId, e.Exception,
                        string.Format("[{0} : {1}] {2} executed the command '{3}' but it threw an error: ",
                            e.Context.Guild.Name, e.Context.Channel.Name, e.Context.User.Username,
                            e.Command?.QualifiedName ?? "<unknown>"));
                    await BotServices.SendEmbedAsync(e.Context, e.Exception.Message, EmbedType.Error);
                    break;

                default:
                    e.Context.Client.Logger.LogError(eventId,
                        $"[{e.Exception.GetType()}] Unhandled Exception. {e.Exception.Message}");
                    break;
            }
        }
    }
}