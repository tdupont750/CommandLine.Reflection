using System.CommandLine.Binding;
using System.Reflection;
using System.Text;

namespace System.CommandLine.Reflection.Base;

public abstract class CliOptionAttributeBase : CliValueDescriptorAttribute
{
    public string? Description { get; init; }
    public string? NameOverride { get; init; }
    public string? AliasOverride { get; init; }
    
    public override (string name, Action addToCommand, IValueDescriptor valueDescriptor) Register(
        Command command, 
        ParameterInfo parameter)
    {
        var (name, alias) = GetNameAndAlias(parameter);

        var description = string.IsNullOrWhiteSpace(Description)
            ? parameter.Name
            : Description;
        
        var optionType = typeof(Option<>).MakeGenericType(parameter.ParameterType);
        var option = (Option) Activator.CreateInstance(optionType, name, description)!;

        option.ArgumentHelpName = parameter.ParameterType.Name;
        
        if (!string.IsNullOrWhiteSpace(alias))
            option.AddAlias(alias);
                    
        SetProperties(option);

        return (option.Name, () => command.Add(option), option);
    }

    protected abstract void SetProperties(Option option);
    
    private (string name, string alias) GetNameAndAlias(ParameterInfo parameter)
    {
        string name;
        if (!string.IsNullOrWhiteSpace(NameOverride))
            name = NameOverride;
        else if (parameter.Name!.Length == 1)
            name = $"-{parameter.Name.ToLower()}";
        else
            name = $"--{GetCliArgName(parameter.Name!)}";

        string alias;
        if (!string.IsNullOrWhiteSpace(AliasOverride) && AliasOverride != name)
            alias = AliasOverride;
        else if (name.Length > 2)
            alias = name[1..3];
        else
            alias = string.Empty;

        if (alias == name)
            alias = string.Empty;

        return (name, alias);
    }
    
    private static string GetCliArgName(string name)
    {
        // Optimize for 10 words or less
        var sb = new StringBuilder(name.Length + 10);

        sb.Append(char.ToLower(name[0]));
        
        foreach(var c in name.Skip(1))
        {
            if (char.IsUpper(c))
            {
                sb.Append('-');
                sb.Append(char.ToLower(c));
            }
            else
            {
                sb.Append(c);
            }
        }

        return sb.ToString();
    }
}