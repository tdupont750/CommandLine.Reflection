using System.Reflection;

namespace System.CommandLine.Reflection.Services;

public interface ICommandLineService
{
    void Bootstrap(
        RootCommand root,
        string rootNamespace,
        Func<Type, object?>? commandFactory = null,
        params Assembly[]? assemblies);

    void Bootstrap(
        RootCommand root,
        string rootNamespace,
        Func<Type, object?>? commandFactory = null,
        params Type[] types);
}