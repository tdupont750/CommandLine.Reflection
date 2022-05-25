namespace ChildProcess;

public static partial class ChildProcessExtensions
{
    #region ToLines

    public static List<string> ToLines(
        this IChildProcess childProcess,
        CancellationToken cancelToken) =>
        childProcess.ToLines(EnsureCleanExit, cancelToken);

    public static List<string> ToLines(
        this IChildProcess childProcess,
        bool ensureCleanExit = EnsureCleanExit,
        CancellationToken cancelToken = default) =>
        childProcess.ToLinesAsync(ensureCleanExit, cancelToken).GetAwaiter().GetResult();
    
    #endregion
    
    #region ToLinesAsync

    public static Task<List<string>> ToLinesAsync(
        this IChildProcess childProcess,
        CancellationToken cancelToken) =>
        childProcess.ToLinesAsync(EnsureCleanExit, cancelToken);

    public static async Task<List<string>> ToLinesAsync(
        this IChildProcess childProcess,
        bool ensureCleanExit = EnsureCleanExit,
        CancellationToken cancelToken = default)
    {
        var (exitCode, lines) = await childProcess
            .ToLinesAndExitCodeAsync(cancelToken)
            .ConfigureAwait(false);

        if (ensureCleanExit && exitCode != 0)
            throw new ExitCodeException(exitCode);

        return lines;
    }

    #endregion

    #region ToLinesAndExitCode

    public static (int ExitCode, List<string> Lines) ToLinesAndExitCode(
        this IChildProcess childProcess,
        CancellationToken cancelToken = default) =>
        childProcess.ToLinesAndExitCodeAsync(cancelToken).GetAwaiter().GetResult();

    #endregion
}
