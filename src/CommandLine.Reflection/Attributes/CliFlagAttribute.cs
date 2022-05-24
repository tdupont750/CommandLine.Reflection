using System.CommandLine.Binding;
using System.CommandLine.Reflection.Base;
using System.Reflection;

namespace System.CommandLine.Reflection;

[AttributeUsage(AttributeTargets.Parameter)]
public class CliFlagAttribute : CliOptionAttributeBase
{
    public override (string name, Action addToCommand, IValueDescriptor valueDescriptor) Register(
        Command command, 
        ParameterInfo parameter)
    {
        if (parameter.ParameterType != typeof(bool))
            throw new InvalidOperationException(
                $"{parameter.Name} has [{nameof(CliFlagAttribute)}], and must be of type {nameof(Boolean)}");
        
        return base.Register(command, parameter);
    }

    protected override void SetProperties(Option option)
    {
        option.Arity = ArgumentArity.Zero;
    }
}