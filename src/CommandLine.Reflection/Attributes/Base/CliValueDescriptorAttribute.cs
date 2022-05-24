using System.CommandLine.Binding;
using System.Reflection;

namespace System.CommandLine.Reflection.Base;

public abstract class CliValueDescriptorAttribute : Attribute
{
    public abstract (string name, Action addToCommand, IValueDescriptor valueDescriptor) Register(
        Command command, 
        ParameterInfo parameter);
}