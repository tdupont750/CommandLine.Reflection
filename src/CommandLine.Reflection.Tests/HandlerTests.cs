using System.CommandLine.Reflection;
using System.CommandLine.Reflection.Handlers;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace CommandLine.Reflection.Tests;

public class HandlerTests
{
    [Test]
    public void YouCanTestCommands()
    {
        const string input = "Hello World";
        
        // Arrange
        var handler = new CliPassthroughHandler();
        var command = new TestCommand();
        command.SetHandler(handler);

        // Act
        var exitCode = handler.InvokeHandlerAsync(input, 42);
        
        // Assert
        Assert.AreEqual(input, command.Input);
        Assert.AreEqual(21, exitCode!.Result);
    }

    private class TestCommand
    {
        public string Input { get; private set; } = string.Empty;
        
        public void SetHandler(ICliHandler handler) => handler.Set((
            [CliOption] string input,
            [CliFlag] int exitCode) =>
        {
            Input = input;
            return Task.FromResult(exitCode / 2);
        });
    }
}