using System.CommandLine.Binding;
using System.Reflection;

namespace System.CommandLine.Reflection.Services;

public interface ICommandLineService
{
    void Bootstrap(
        RootCommand root,
        string rootNamespace,
        IEnumerable<Assembly> assemblies);

    void Bootstrap(
        RootCommand root,
        string rootNamespace,
        IEnumerable<Type> types);
}