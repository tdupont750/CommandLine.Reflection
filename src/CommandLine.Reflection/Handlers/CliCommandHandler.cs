using System.CommandLine.Binding;
using System.CommandLine.Reflection.Base;
using System.Reflection;

namespace System.CommandLine.Reflection.Handlers;

internal class CliCommandHandler : ICliHandler
{
    private readonly Command _command;
    private readonly IDictionary<Type, IValueDescriptor>? _binders;

    public CliCommandHandler(Command command, IDictionary<Type, IValueDescriptor>? binders)
    {
        _command = command;
        _binders = binders;
    }

    private IValueDescriptor[] GetValueDescriptors(MethodInfo method)
    {
        var nameToInput = new SortedDictionary<string, Action>();
        var valueDescriptors = new List<IValueDescriptor>();
        
        var parameters = method.GetParameters();
        foreach (var parameter in parameters)
        {
            var optionAttribute = parameter.GetCustomAttribute<CliValueDescriptorAttribute>();
            if (optionAttribute != null)
            {
                var (name, addToCommand, valueDescriptor) = optionAttribute.Register(_command, parameter);
                nameToInput[name.Replace("-", string.Empty)] = addToCommand;
                valueDescriptors.Add(valueDescriptor);
                continue;
            }

            if (_binders == null) 
                continue;
            
            var binderAttribute = parameter.GetCustomAttribute<BinderAttribute>();
            if (binderAttribute == null) 
                continue;
            
            if (!_binders.TryGetValue(parameter.ParameterType, out var binder))
                continue;

            valueDescriptors.Add(binder);
        }

        // Register in alphabetical order
        foreach (var pair in nameToInput)
            pair.Value();

        return valueDescriptors.ToArray();
    }

    #region Set Overloads
    
    public void Set<T1>(Func<T1, Task<int>> handler) => 
        _command.SetHandler(handler, GetValueDescriptors(handler.Method));

    public void Set<T1, T2>(Func<T1, T2, Task<int>> handler) => 
        _command.SetHandler(handler, GetValueDescriptors(handler.Method));

    public void Set<T1, T2, T3>(Func<T1, T2, T3, Task<int>> handler) => 
        _command.SetHandler(handler, GetValueDescriptors(handler.Method));

    public void Set<T1, T2, T3, T4>(Func<T1, T2, T3, T4, Task<int>> handler) => 
        _command.SetHandler(handler, GetValueDescriptors(handler.Method));

    public void Set<T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5, Task<int>> handler) => 
        _command.SetHandler(handler, GetValueDescriptors(handler.Method));

    public void Set<T1, T2, T3, T4, T5, T6>(Func<T1, T2, T3, T4, T5, T6, Task<int>> handler) => 
        _command.SetHandler(handler, GetValueDescriptors(handler.Method));

    public void Set<T1, T2, T3, T4, T5, T6, T7>(Func<T1, T2, T3, T4, T5, T6, T7, Task<int>> handler) => 
        _command.SetHandler(handler, GetValueDescriptors(handler.Method));

    public void Set<T1, T2, T3, T4, T5, T6, T7, T8>(Func<T1, T2, T3, T4, T5, T6, T7, T8, Task<int>> handler) => 
        _command.SetHandler(handler, GetValueDescriptors(handler.Method));

    public void Set<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, Task<int>> handler) => 
        _command.SetHandler(handler, GetValueDescriptors(handler.Method));

    public void Set<T1, T2, T3, T4, T5, T6, T7, T8, T9, T0>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T0, Task<int>> handler) => 
        _command.SetHandler(handler, GetValueDescriptors(handler.Method));

    #endregion
}