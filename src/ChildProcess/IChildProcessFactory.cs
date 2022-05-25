using ChildProcess.Configuration;

namespace ChildProcess;

/// <summary>
/// A default implementation of the generic IChildProcessFactory<>
/// </summary>
public interface IChildProcessFactory : IChildProcessFactory<ChildProcessConfig>
{
}

public interface IChildProcessFactory<out T>
    where T : IChildProcess
{
    public T Exe(string fileName, IEnumerable<string> arguments);
    
    public T Sh(IEnumerable<string> arguments);
}
