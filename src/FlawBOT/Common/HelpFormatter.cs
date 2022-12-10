using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.CommandsNext.Entities;
using DSharpPlus.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlawBOT.Common
{
    public sealed class HelpFormatter : BaseHelpFormatter
    {
        private readonly DiscordEmbedBuilder _output;
        private string _description;
        private string _name;

        public HelpFormatter(CommandContext ctx) : base(ctx)
        {
            _output = new DiscordEmbedBuilder()
                .WithColor(DiscordColor.Turquoise)
                .WithUrl(Program.Settings.GitHubLink + "wiki");
        }

        public override CommandHelpMessage Build()
        {
            var desc =
                $"Listing all commands and groups. Use {Formatter.InlineCode(".help <command>")} for more details.";
            if (!string.IsNullOrWhiteSpace(_name))
            {
                _output.WithTitle(_name);
                desc = _description ?? "No description provided.";
            }

            _output.WithDescription(desc);
            return new CommandHelpMessage(embed: _output);
        }

        public override BaseHelpFormatter WithCommand(Command cmd)
        {
            _name = (cmd is CommandGroup ? "Group: " : "Command: ") + cmd.QualifiedName;
            _description = cmd.Description;

            if (cmd.Aliases?.Any() ?? false)
                _output.AddField("Aliases", string.Join(", ", cmd.Aliases.Select(Formatter.InlineCode)), true);

            if (!(cmd.Overloads?.Any() ?? false)) return this;
            foreach (var overload in cmd.Overloads.OrderByDescending(o => o.Priority))
            {
                if (overload.Arguments.Count == 0) continue;

                var args = new StringBuilder();
                foreach (var arg in overload.Arguments)
                {
                    args.Append(Formatter.InlineCode($"[{CommandsNext.GetUserFriendlyTypeName(arg.Type)}]"));
                    args.Append(' ');
                    args.Append(arg.Description ?? "No description provided.");
                    if (arg.IsOptional)
                    {
                        args.Append(" (def: ")
                            .Append(Formatter.InlineCode(arg.DefaultValue is null
                                ? "None"
                                : arg.DefaultValue.ToString())).Append(')');
                        args.Append(" (optional)");
                    }

                    args.AppendLine();
                }

                _output.AddField($"{(cmd.Overloads.Count > 1 ? $"Overload #{overload.Priority}" : "Arguments")}",
                    args.ToString() ?? "No arguments.");
            }

            return this;
        }

        public override BaseHelpFormatter WithSubcommands(IEnumerable<Command> subcommands)
        {
            var enumerable = subcommands.ToList();
            if (enumerable.Any())
                _output.AddField(_name is null ? "Commands" : "Subcommands",
                    string.Join(", ", enumerable.Select(c => Formatter.InlineCode(c.Name))));
            return this;
        }
    }
}