using System.Diagnostics;
using System.Text;
using ChildProcess.Configuration;

namespace ChildProcess.Implementation;

public class ChildProcessExecutor : IChildProcess
{
    private readonly ChildProcessConfig _config;

    public ChildProcessExecutor(ChildProcessConfig config) => _config = config;

    public async Task<(int ExitCode, string Text)> ToTextAndExitCodeAsync(CancellationToken cancelToken)
    {
        var sb = new StringBuilder();

        _config.OnLineReceived(s =>
        {
            lock (sb) sb.AppendLine(s);
        });

        var exitCode = await ToExitCodeAsync(cancelToken).ConfigureAwait(false);

        return (exitCode, sb.ToString());
    }

    public async Task<(int ExitCode, List<string> Lines)> ToLinesAndExitCodeAsync(CancellationToken cancelToken)
    {
        var lines = new List<string>();

        _config.OnLineReceived(s =>
        {
            lock (lines) lines.Add(s);
        });

        var exitCode = await ToExitCodeAsync(cancelToken).ConfigureAwait(false);

        return (exitCode, lines);
    }

    public async Task<int> ToExitCodeAsync(CancellationToken cancelToken)
    {
        using var process = CreateProcess();

        var tcsOutput = new TaskCompletionSource();
        process.OutputDataReceived += (_, e) =>
            OnDataReceived(e.Data, ChildProcessConfig.DataReceivedType.Output, tcsOutput);

        var tcsError = new TaskCompletionSource();
        process.ErrorDataReceived += (_, e) =>
            OnDataReceived(e.Data, ChildProcessConfig.DataReceivedType.Error, tcsError);

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(
            _config.GlobalCancelToken, 
            cancelToken);

        await using (cts.Token.Register(() =>
                     {
                         try
                         {
                             // ReSharper disable once AccessToDisposedClosure
                             process.Kill(true);
                         }
                         catch
                         {
                             // TODO Don't ignore?
                         }
                     }))
        {
            if (!process.Start())
                return -1;

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            // Use CancellationToken.None because registered process kill will exit the process
            await process.WaitForExitAsync(CancellationToken.None).ConfigureAwait(false);
        }

        await tcsOutput.Task.ConfigureAwait(false);
        await tcsError.Task.ConfigureAwait(false);

        return process.ExitCode;
    }

    private Process CreateProcess()
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = _config.FileName,
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            WorkingDirectory = _config.WorkingDirectory
        };

        foreach (var arg in _config.ArgumentsList)
            startInfo.ArgumentList.Add(arg);

        foreach(var pair in _config.EnvVars)
            startInfo.EnvironmentVariables[pair.Name] = pair.Value;

        return new Process
        {
            StartInfo = startInfo,
            EnableRaisingEvents = true
        };
    }

    private void OnDataReceived(
        string? line,
        ChildProcessConfig.DataReceivedType receivedType,
        TaskCompletionSource cts)
    {
        if (line == null)
        {
            cts.TrySetResult();
            return;
        }

        foreach (var action in _config.LineReceivedActions)
            action(receivedType, line);
    }
}
