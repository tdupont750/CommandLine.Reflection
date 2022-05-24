namespace System.CommandLine.Reflection;

[AttributeUsage(AttributeTargets.Class)]
public class CliCommandAttribute : Attribute
{
    public CliCommandAttribute(string description) => Description = description;

    public string Description { get; }
    public string? Name { get; init; }
    public string? ChildNamespace { get; init; }
    public string? ParentNamespace { get; init; }

    public string GetName(Type type) =>
        !string.IsNullOrWhiteSpace(Name) 
            ? Name 
            : RemoveCommand(type).ToLower();
    
    public string GetChildNamespace(Type type) =>
        !string.IsNullOrWhiteSpace(ChildNamespace)
            ? ChildNamespace 
            : $"{type.Namespace}.{RemoveCommand(type)}";
    
    public string GetParentNamespace(Type type) =>
        !string.IsNullOrWhiteSpace(ParentNamespace)
            ? ParentNamespace 
            : type.Namespace!;

    private static string RemoveCommand(Type type)
    {
        const string command = "Command";
        return type.Name.EndsWith(command) ? type.Name[..^command.Length] : type.Name;
    }
}