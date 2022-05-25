namespace ChildProcess;

public class ExitCodeException : Exception
{
    public int ExitCode { get; }

    public ExitCodeException(int exitCode, string? message = null)
        : base($"Exit Code: {exitCode} - {message ?? "Unknown Error"}") =>
        ExitCode = exitCode;
}
