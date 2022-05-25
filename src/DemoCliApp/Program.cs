using System.CommandLine;
using System.CommandLine.Binding;
using System.CommandLine.Reflection.Services.Implementation;
using System.Reflection;
using ChildProcess;
using ChildProcess.Configuration;
using ChildProcess.Implementation;

namespace DemoCliApp;

public static class Program
{
    public static async Task<int> Main(string[] args)
    {
        var rootCommand = new RootCommand("Sample app for System.CommandLine") {Name = "demo"};
        rootCommand.AddOption(new Option("--add-alias", "Add alias for bash file to ~/.bashrc"));
        rootCommand.AddOption(new Option("--build-cli", "Rebuild the CLI tool"));
        rootCommand.AddOption(new Option("--print-log", "Print the log form the last run"));
        
        Bootstrap(rootCommand);

        return await rootCommand.InvokeAsync(args);
    }

    /// <summary>
    /// Our custom bootstrapper that uses System.Commandline.Reflection
    /// </summary>
    private static void Bootstrap(RootCommand rootCommand)
    {
        // Create a map of type to binders for dependency injection via System.CommandLine
        var binders = new Dictionary<Type, IValueDescriptor>
        {
            {typeof(IChildProcessFactory), new ChildProcessFactoryBinder()}
        };

        // Create the CommandLineService
        var commandLineService = new CommandLineService(binders);
        
        // Make all the magic reflection happen
        commandLineService.Bootstrap(
            rootCommand,
            $"{nameof(DemoCliApp)}.{nameof(Commands)}",
            new[] {Assembly.GetExecutingAssembly()});
    }
    
    /// <remarks>
    /// I really don't like that you have to create an entire class for reach CLI binder.
    /// An alternative would be to use DI via the command constructors, which is what the CommandFactory arg is for. 
    /// </remarks>>
    private class ChildProcessFactoryBinder : BinderBase<IChildProcessFactory>
    {
        protected override IChildProcessFactory GetBoundValue(BindingContext bindingContext) => 
            ChildProcessFactory.Value;
    }

    /// <summary>
    /// This is the global wire up for child process support
    /// </summary>
    private static readonly Lazy<IChildProcessFactory> ChildProcessFactory = new(() =>
    {
        // I would STRONGLY RECOMMEND making this configurable and/or dynamic
        var config = GlobalChildProcessConfig.CreateWithSh(@"C:\Program Files\Git\usr\bin\sh.exe");
        return new ChildProcessExecutorFactory(config);
    });
}