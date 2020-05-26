using System.Collections.Generic;
using System.Linq;
using System.Text;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.CommandsNext.Entities;
using DSharpPlus.Entities;

namespace FlawBOT.Common
{
    public sealed class HelpFormatter : BaseHelpFormatter
    {
        private readonly DiscordEmbedBuilder output;
        private string description;
        private string name;

        public HelpFormatter(CommandContext ctx) : base(ctx)
        {
            output = new DiscordEmbedBuilder()
                .WithColor(DiscordColor.Turquoise)
                .WithUrl(SharedData.GitHubLink + "wiki");
        }

        public override CommandHelpMessage Build()
        {
            var desc = $"Listing all commands and groups. Use {Formatter.InlineCode(".help <command>")} for more details.";
            if (!string.IsNullOrWhiteSpace(name))
            {
                output.WithTitle(name);
                desc = description ?? "No description provided.";
            }

            output.WithDescription(desc);
            return new CommandHelpMessage(embed: output);
        }

        public override BaseHelpFormatter WithCommand(Command cmd)
        {
            name = (cmd is CommandGroup ? "Group: " : "Command: ") + cmd.QualifiedName;
            description = cmd.Description;

            if (cmd.Overloads?.Any() ?? false)
                foreach (var overload in cmd.Overloads.OrderByDescending(o => o.Priority))
                {
                    var args = new StringBuilder();
                    foreach (var arg in overload.Arguments)
                    {
                        args.Append(Formatter.InlineCode($"[{CommandsNext.GetUserFriendlyTypeName(arg.Type)}]"));
                        args.Append(" ");
                        args.Append(arg.Description ?? "No description provided.");
                        if (arg.IsOptional)
                        {
                            args.Append(" (def: ")
                                .Append(Formatter.InlineCode(arg.DefaultValue is null
                                    ? "None"
                                    : arg.DefaultValue.ToString())).Append(")");
                            args.Append(" (optional)");
                        }

                        args.AppendLine();
                    }

                    output.AddField($"{(cmd.Overloads.Count > 1 ? $"Overload #{overload.Priority}" : "Arguments")}", args.ToString() ?? "No arguments.");
                }

            if (cmd.Aliases?.Any() ?? false)
                output.AddField("Aliases", string.Join(", ", cmd.Aliases.Select(a => Formatter.InlineCode(a))), true);
            return this;
        }

        public override BaseHelpFormatter WithSubcommands(IEnumerable<Command> subcommands)
        {
            if (subcommands.Any())
                output.AddField(name is null ? "Commands" : "Subcommands", string.Join(", ", subcommands.Select(c => Formatter.InlineCode(c.Name))));
            return this;
        }
    }
}