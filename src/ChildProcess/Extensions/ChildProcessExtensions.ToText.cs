namespace ChildProcess;

public static partial class ChildProcessExtensions
{
    #region ToText

    public static string ToText(
        this IChildProcess childProcess,
        CancellationToken cancelToken) =>
        childProcess.ToText(EnsureCleanExit, cancelToken);

    public static string ToText(
        this IChildProcess childProcess,
        bool ensureCleanExit = EnsureCleanExit,
        CancellationToken cancelToken = default) =>
        childProcess.ToTextAsync(ensureCleanExit, cancelToken).GetAwaiter().GetResult();

    #endregion

    #region ToTextAsync
    
    public static Task<string> ToTextAsync(
        this IChildProcess childProcess,
        CancellationToken cancelToken) =>
        childProcess.ToTextAsync(EnsureCleanExit, cancelToken);

    public static async Task<string> ToTextAsync(
        this IChildProcess childProcess,
        bool ensureCleanExit = EnsureCleanExit,
        CancellationToken cancelToken = default)
    {
        var (exitCode, text) = await childProcess
            .ToTextAndExitCodeAsync(cancelToken)
            .ConfigureAwait(false);

        if (ensureCleanExit && exitCode != 0)
            throw new ExitCodeException(exitCode);
        
        return text;
    }

    #endregion

    #region ToTextAndExitCode
    
    public static (int ExitCode, string Text) ToTextAndExitCode(
        this IChildProcess childProcess,
        CancellationToken cancelToken = default) =>
        childProcess.ToTextAndExitCodeAsync(cancelToken).GetAwaiter().GetResult();

    #endregion
}
