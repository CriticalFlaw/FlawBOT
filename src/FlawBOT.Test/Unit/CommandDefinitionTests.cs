using System.Reflection;
using System.Text.RegularExpressions;
using DSharpPlus.SlashCommands;

namespace FlawBOT.Test.Unit;

/// <summary>
///     Discord rejects the whole command registration if any one command breaks its naming rules,
///     so validate every slash command definition up front.
/// </summary>
[TestFixture]
[Category("Unit")]
internal partial class CommandDefinitionTests
{
    private static readonly Type[] Modules = typeof(FlawBot).Assembly.GetTypes()
        .Where(t => t.IsSubclassOf(typeof(ApplicationCommandModule)) && !t.IsAbstract)
        .OrderBy(t => t.Name).ToArray();

    private static IEnumerable<MethodInfo> Commands(Type module) =>
        module.GetMethods().Where(m => m.GetCustomAttribute<SlashCommandAttribute>() is not null);

    [GeneratedRegex(@"^[-_\p{Ll}\p{N}]{1,32}$")]
    private static partial Regex NameRegex();

    [Test]
    public void ModulesAreDiscovered()
    {
        Assert.That(Modules, Has.Length.EqualTo(22));
    }

    [TestCaseSource(nameof(Modules))]
    public void NamesAndDescriptionsAreValid(Type module)
    {
        var group = module.GetCustomAttribute<SlashCommandGroupAttribute>();
        Assert.Multiple(() =>
        {
            if (group is not null)
            {
                Assert.That(group.Name, Does.Match(NameRegex()), $"Group name '{group.Name}'");
                Assert.That(group.Description, Has.Length.InRange(1, 100), $"Group '{group.Name}' description");
            }

            var commands = Commands(module).ToList();
            Assert.That(commands, Is.Not.Empty);
            Assert.That(commands, Has.Count.LessThanOrEqualTo(25));
            foreach (var method in commands)
            {
                var command = method.GetCustomAttribute<SlashCommandAttribute>()!;
                Assert.That(command.Name, Does.Match(NameRegex()), $"Command name '{command.Name}'");
                Assert.That(command.Description, Has.Length.InRange(1, 100), $"Command '{command.Name}' description");

                var parameters = method.GetParameters();
                Assert.That(parameters[0].ParameterType, Is.EqualTo(typeof(InteractionContext)), $"Command '{command.Name}' context");

                var seenOptional = false;
                foreach (var parameter in parameters.Skip(1))
                {
                    var option = parameter.GetCustomAttribute<OptionAttribute>();
                    Assert.That(option, Is.Not.Null, $"'{command.Name}' parameter '{parameter.Name}' needs an [Option]");
                    if (option is null) continue;
                    Assert.That(option.Name, Does.Match(NameRegex()), $"'{command.Name}' option name '{option.Name}'");
                    Assert.That(option.Description, Has.Length.InRange(1, 100), $"'{command.Name}' option '{option.Name}' description");

                    // Discord requires required options to come before optional ones.
                    if (parameter.HasDefaultValue) seenOptional = true;
                    else Assert.That(seenOptional, Is.False, $"'{command.Name}' required option '{option.Name}' follows an optional one");
                }
            }
        });
    }

    [Test]
    public void TopLevelCommandNamesAreUnique()
    {
        var names = Modules.SelectMany(m =>
        {
            var group = m.GetCustomAttribute<SlashCommandGroupAttribute>();
            return group is not null ? [group.Name] : Commands(m).Select(c => c.GetCustomAttribute<SlashCommandAttribute>()!.Name);
        }).ToList();
        Assert.That(names, Is.Unique);
    }

    [Test]
    public void OwnerAndPermissionChecksUseSlashCommandAttributes()
    {
        // CommandsNext attributes such as [RequireOwner] are silently ignored on slash commands,
        // which previously let anyone change the bot's avatar or rename the server.
        var commandsNextAttributes = Modules.SelectMany(m => Commands(m).Prepend(null).Select(c => (Module: m, Method: c)))
            .SelectMany(x => (x.Method?.GetCustomAttributes() ?? x.Module.GetCustomAttributes())
                .Where(a => a.GetType().Namespace?.StartsWith("DSharpPlus.CommandsNext", StringComparison.Ordinal) == true)
                .Select(a => $"{x.Module.Name}.{x.Method?.Name}: {a.GetType().Name}"))
            .ToList();
        Assert.That(commandsNextAttributes, Is.Empty);
    }
}
