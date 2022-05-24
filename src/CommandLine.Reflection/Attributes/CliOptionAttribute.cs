using System.CommandLine.Reflection.Base;

namespace System.CommandLine.Reflection;

[AttributeUsage(AttributeTargets.Parameter)]
public class CliOptionAttribute : CliOptionAttributeBase
{
    public bool IsRequired { get; init; }
    public ArityValue Arity { get; init; } = ArityValue.ExactlyOne;
    public object? DefaultValue { get; init; }
    
    protected override void SetProperties(Option option)
    {
        option.Arity = Arity.ToArgumentArity();
        
        option.IsRequired = IsRequired;

        if (DefaultValue != null)
            option.SetDefaultValue(DefaultValue);
    }
}