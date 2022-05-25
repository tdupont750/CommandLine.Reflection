namespace ChildProcess;

public static partial class ChildProcessExtensions
{
    private const bool EnsureCleanExit = true;

    #region Execute

    public static void Execute(
        this IChildProcess childProcess,
        CancellationToken cancelToken) =>
        childProcess.Execute(EnsureCleanExit, cancelToken);

    public static void Execute(
        this IChildProcess childProcess,
        bool ensureCleanExit = EnsureCleanExit,
        CancellationToken cancelToken = default) =>
        childProcess.ExecuteAsync(ensureCleanExit, cancelToken).GetAwaiter().GetResult();

    #endregion
    
    #region ExecuteAsync

    public static Task ExecuteAsync(
        this IChildProcess childProcess,
        CancellationToken cancelToken) =>
        childProcess.ExecuteAsync(EnsureCleanExit, cancelToken);

    public static async Task ExecuteAsync(
        this IChildProcess childProcess,
        bool ensureCleanExit = EnsureCleanExit,
        CancellationToken cancelToken = default)
    {
         var exitCode = await childProcess
             .ToExitCodeAsync(cancelToken)
             .ConfigureAwait(false);
         
         if (ensureCleanExit && exitCode != 0)
             throw new ExitCodeException(exitCode);
    }
    
    #endregion
}
