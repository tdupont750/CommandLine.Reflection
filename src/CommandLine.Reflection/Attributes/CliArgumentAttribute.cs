using System.CommandLine.Binding;
using System.CommandLine.Reflection.Base;
using System.Reflection;

namespace System.CommandLine.Reflection;


[AttributeUsage(AttributeTargets.Parameter)]
public class CliArgumentAttribute : CliValueDescriptorAttribute
{
    public string? Description { get; init; }
    public ArityValue Arity { get; init; } = ArityValue.ExactlyOne;

    public override (string name, Action addToCommand, IValueDescriptor valueDescriptor) Register(
        Command command,
        ParameterInfo parameter)
    {
        var argumentType = typeof(Argument<>).MakeGenericType(parameter.ParameterType);
        
        var argument = (Argument) Activator.CreateInstance(
            argumentType, 
            parameter.Name,
            Description)!;

        argument.Arity = Arity.ToArgumentArity();
        
        return (argument.Name, () => command.Add(argument), argument);
    }
}