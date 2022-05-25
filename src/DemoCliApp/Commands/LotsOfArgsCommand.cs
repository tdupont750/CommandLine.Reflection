using System.CommandLine.Reflection;

namespace DemoCliApp.Commands;

[CliCommand("A test command")]
public class LotsOfArgsCommand : ICliCommand
{
    public void SetHandler(ICliHandler handler) => handler.Set((
        [CliOption] string someWords,
        [CliOption(Description = "C")] char c,
        [CliOption(Description = "I", IsRequired = true)] int i,
        [CliFlag] bool boolean,
        [CliArgument] string thisIsAnArgument) =>
    {
        Console.WriteLine($"{c} | {i} | {boolean}");
        return Task.FromResult(i);
    });
}