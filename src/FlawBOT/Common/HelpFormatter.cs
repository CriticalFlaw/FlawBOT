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
        private string name;
        private string description;
        private readonly DiscordEmbedBuilder output;

        public HelpFormatter(CommandsNextExtension cnext) : base(cnext)
        {
            output = new DiscordEmbedBuilder()
                .WithColor(DiscordColor.Turquoise)
                .WithUrl("https://docs.google.com/spreadsheets/d/15c0Q7Cm07wBRNeSFwkagwDOe6zk9rVMvlM7H_Y7nGUs/edit?usp=sharing");
        }

        public override CommandHelpMessage Build()
        {
            string desc = $"Listing all commands and groups. Use {Formatter.InlineCode("!help <command>")} for detailed information.";
            if (!string.IsNullOrWhiteSpace(name))
            {
                output.WithTitle(name);
                desc = string.IsNullOrWhiteSpace(description) ? "No description provided." : description;
            }
            output.WithDescription(desc);
            return new CommandHelpMessage(embed: output);
        }

        public override BaseHelpFormatter WithCommand(Command cmd)
        {
            name = cmd is CommandGroup ? $"Group: {cmd.QualifiedName}" : $"Command: {cmd.QualifiedName}";
            description = cmd.Description;

            if (cmd.Overloads?.Any() ?? false)
            {
                foreach (var overload in cmd.Overloads.OrderByDescending(o => o.Priority))
                {
                    var ab = new StringBuilder();
                    foreach (var arg in overload.Arguments)
                    {
                        if (arg.IsOptional)
                            ab.Append("(optional) ");
                        ab.Append(Formatter.InlineCode($"[{CommandsNext.GetUserFriendlyTypeName(arg.Type)}]"));
                        ab.Append(" ");
                        ab.Append(string.IsNullOrWhiteSpace(arg.Description) ? "No description provided." : arg.Description);
                        if (arg.IsOptional)
                            ab.Append(" (def: ").Append(Formatter.InlineCode(arg.DefaultValue is null ? "None" : arg.DefaultValue.ToString())).Append(")");
                        ab.AppendLine();
                    }
                    string args = ab.ToString();
                    output.AddField($"{(cmd.Overloads.Count > 1 ? $"Overload #{overload.Priority}" : "Arguments")}", string.IsNullOrWhiteSpace(args) ? "No arguments." : args);
                }
            }

            if (cmd.Aliases?.Any() ?? false)
                output.AddField("Aliases", string.Join(", ", cmd.Aliases.Select(a => Formatter.InlineCode(a))), inline: true);
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