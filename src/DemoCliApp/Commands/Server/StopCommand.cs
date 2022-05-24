using System.CommandLine.Reflection;

namespace DemoCliApp.Commands.Server;

[CliCommand("Stop those servers")]
public class StopCommand : ICliCommand
{
    public void SetHandler(ICliHandler handler) => handler.Set((
        [CliFlag] bool force) =>
    {
        Console.WriteLine($"{force}");
        return Task.FromResult(0);
    });
}