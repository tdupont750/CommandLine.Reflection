namespace ChildProcess;

public static class ChildProcessFactoryExtensions
{
    public static T Exe<T>(this IChildProcessFactory<T> factory, string fileName, params string[] arguments)
        where T : IChildProcess =>
        factory.Exe(fileName, arguments);
    
    public static T Sh<T>(this IChildProcessFactory<T> factory, params string[] arguments)
        where T : IChildProcess =>
        factory.Sh(arguments);
}