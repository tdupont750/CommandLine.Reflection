namespace ChildProcess;

public interface IChildProcess
{
    public Task<int> ToExitCodeAsync(CancellationToken cancelToken = default);

    public Task<(int ExitCode, string Text)> ToTextAndExitCodeAsync(CancellationToken cancelToken = default);

    public Task<(int ExitCode, List<string> Lines)> ToLinesAndExitCodeAsync(CancellationToken cancelToken = default);
}
