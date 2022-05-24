using System.CommandLine.Reflection;

namespace DemoCliApp.Commands.Server;

[CliCommand("Start your engines")]
public class StartCommand : ICliCommand
{
    public void SetHandler(ICliHandler handler) => handler.Set((
        [CliOption(IsRequired = true)] string version) =>
    {
        Console.WriteLine($"{version}");
        return Task.FromResult(0);
    });
}