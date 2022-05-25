using System.CommandLine.Reflection;
using System.CommandLine.Reflection.Base;
using ChildProcess;

namespace DemoCliApp.Commands.Git;

[CliCommand("Git that status!")]
public class StatusCommand : ICliCommand
{
    public void SetHandler(ICliHandler handler) => handler.Set((
        [Binder] IChildProcessFactory childProcess) =>
    {
        return childProcess
            .Sh("git status")
            .OnLineReceived(Console.WriteLine)
            .ToExitCodeAsync();
    });
}