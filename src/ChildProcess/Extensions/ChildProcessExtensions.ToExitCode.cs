namespace ChildProcess;

public static partial class ChildProcessExtensions
{
    public static int ToExitCode(
        this IChildProcess childProcess,
        CancellationToken cancelToken) =>
        childProcess.ToExitCodeAsync(cancelToken).GetAwaiter().GetResult();
}
