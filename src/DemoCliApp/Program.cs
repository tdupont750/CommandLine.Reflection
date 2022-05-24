using System.CommandLine;
using System.CommandLine.Reflection;

namespace DemoCliApp;

public static class Program
{
    public static async Task<int> Main(string[] args)
    {
        var rootCommand = new RootCommand("Sample app for System.CommandLine") {Name = "demo"};
        rootCommand.AddOption(new Option("--add-alias", "Add alias for bash file to ~/.bashrc"));
        rootCommand.AddOption(new Option("--build-cli", "Rebuild the CLI tool"));
        rootCommand.Bootstrap($"{nameof(DemoCliApp)}.{nameof(Commands)}");
        return await rootCommand.InvokeAsync(args);
    }
}