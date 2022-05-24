namespace System.CommandLine.Reflection;

public interface ICliCommand
{
    void SetHandler(ICliHandler handler);
}