using System.CommandLine.Reflection.Services.Implementation;
using System.Reflection;

namespace System.CommandLine.Reflection;

public static class CommandExtensions
{
    public static void Bootstrap(
        this RootCommand root,
        string rootNamespace, 
        Func<Type, object?>? commandFactory = null,
        params Assembly[]? assemblies) =>
        CommandLineService.Instance.Bootstrap(root, rootNamespace, commandFactory, assemblies);
}