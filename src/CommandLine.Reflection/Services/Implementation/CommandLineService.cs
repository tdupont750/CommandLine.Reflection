using System.CommandLine.Help;
using System.CommandLine.Invocation;
using System.CommandLine.IO;
using System.CommandLine.Reflection.Handlers;
using System.Reflection;

namespace System.CommandLine.Reflection.Services.Implementation;

/// <remarks>
/// https://docs.microsoft.com/en-us/dotnet/standard/commandline/model-binding#built-in-argument-validation
/// https://docs.microsoft.com/en-us/dotnet/standard/commandline/define-commands
/// </remarks>>
public class CommandLineService : ICommandLineService
{
    private static readonly Lazy<CommandLineService> LazyInstance = new(() => new CommandLineService());

    public static ICommandLineService Instance => LazyInstance.Value;
    
    public void Bootstrap(
        RootCommand root,
        string rootNamespace,
        Func<Type, object?>? commandFactory,
        Assembly[]? assemblies)
    {
        if (assemblies == null || assemblies.Length == 0) 
            assemblies = new[] {Assembly.GetCallingAssembly()};

        var types = assemblies
            .SelectMany(a => a.GetTypes())
            .ToArray();
        
        Bootstrap(root, rootNamespace, commandFactory, types);
    }
    
    public void Bootstrap(
        RootCommand root,
        string rootNamespace,
        Func<Type, object?>? commandFactory,
        Type[] types)
    {
        var commandTypes = types
            .Select(t => (Type: t, Attr: t.GetCustomAttribute<CliCommandAttribute>()))
            .Where(p => p.Attr != null)
            .Select(p => (p.Type, p.Attr!))
            .ToArray();

        BootstrapChildCommand(commandFactory ?? Activator.CreateInstance, root, rootNamespace, commandTypes);
        
        root.SetHandler((InvocationContext context) => EmptyCommandHandler(context, root));
    }

    private static void BootstrapChildCommand(Func<Type, object?> commandFactory, Command parentCommand, string ns, (Type Type, CliCommandAttribute Attr)[] types)
    {
        foreach (var (type, attr) in types)
        {
            if (attr.GetParentNamespace(type) != ns)
                continue;

            var name = attr.GetName(type);
            var childCommand = new Command(name, attr.Description);
            parentCommand.AddCommand(childCommand);

            if (commandFactory(type) is ICliCommand cliCommand)
            {
                var cliHandler = new CliCommandHandler(childCommand);
                cliCommand.SetHandler(cliHandler);
            }
            else
                childCommand.SetHandler((InvocationContext context) => EmptyCommandHandler(context, childCommand));

            var childNamespace =  attr.GetChildNamespace(type);
            BootstrapChildCommand(commandFactory, childCommand, childNamespace, types);
        }
    }

    private static Task<int> EmptyCommandHandler(InvocationContext context, Command command)
    {
        var output = context.Console.Out.CreateTextWriter();
        context.HelpBuilder.Write(new HelpContext(context.HelpBuilder, command, output));
        return Task.FromResult(0);
    }
    
}