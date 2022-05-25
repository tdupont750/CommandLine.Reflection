using System.CommandLine.Binding;
using System.CommandLine.Help;
using System.CommandLine.Invocation;
using System.CommandLine.IO;
using System.CommandLine.Reflection.Handlers;
using System.Reflection;

namespace System.CommandLine.Reflection.Services.Implementation;

/// <remarks>
/// https://docs.microsoft.com/en-us/dotnet/standard/commandline/
/// </remarks>>
public class CommandLineService : ICommandLineService
{
    private readonly IDictionary<Type, IValueDescriptor>? _binders;
    private readonly Func<Type, object?> _commandFactory;

    public CommandLineService(
        IDictionary<Type, IValueDescriptor>? binders = default,
        Func<Type, object?>? commandFactory = null)
    {
        _binders = binders;
        _commandFactory = commandFactory ?? Activator.CreateInstance;
    }
    
    public void Bootstrap(
        RootCommand root,
        string rootNamespace, 
        IEnumerable<Assembly> assemblies)
    {
        var types = assemblies
            .SelectMany(a => a.GetTypes())
            .ToArray();
        
        Bootstrap(root, rootNamespace, types);
    }
    
    public void Bootstrap(
        RootCommand root,
        string rootNamespace,
        IEnumerable<Type> types)
    {
        var commandTypes = types
            .Select(t => (Type: t, Attr: t.GetCustomAttribute<CliCommandAttribute>()))
            .Where(p => p.Attr != null)
            .Select(p => (p.Type, p.Attr!))
            .ToArray();

        BootstrapChildCommand(root, rootNamespace, commandTypes);
        
        root.SetHandler((InvocationContext context) => EmptyCommandHandler(context, root));
    }

    private void BootstrapChildCommand(
        Command parentCommand, 
        string ns, 
        (Type Type, CliCommandAttribute Attr)[] types)
    {
        foreach (var (type, attr) in types)
        {
            if (attr.GetParentNamespace(type) != ns)
                continue;

            var name = attr.GetName(type);
            var childCommand = new Command(name, attr.Description);
            parentCommand.AddCommand(childCommand);

            if (_commandFactory(type) is ICliCommand cliCommand)
            {
                var cliHandler = new CliCommandHandler(childCommand, _binders);
                cliCommand.SetHandler(cliHandler);
            }
            else
                childCommand.SetHandler((InvocationContext context) => EmptyCommandHandler(context, childCommand));

            var childNamespace =  attr.GetChildNamespace(type);
            BootstrapChildCommand(childCommand, childNamespace, types);
        }
    }

    private static Task<int> EmptyCommandHandler(InvocationContext context, Command command)
    {
        var output = context.Console.Out.CreateTextWriter();
        context.HelpBuilder.Write(new HelpContext(context.HelpBuilder, command, output));
        return Task.FromResult(0);
    }
}