namespace ChildProcess.Configuration;

public class ChildProcessConfig : IChildProcess
{
    private readonly GlobalChildProcessConfig _globalConfig;
    private readonly IChildProcess _executor;

    public ChildProcessConfig(
        string fileName,
        IEnumerable<string> argumentsList,
        GlobalChildProcessConfig globalConfig,
        Func<ChildProcessConfig, IChildProcess> executorFactory)
    {
        FileName = fileName;
        WorkingDirectory = globalConfig.DefaultWorkingDirectory;
        _globalConfig = globalConfig;
        _executor = executorFactory(this);
        
        ArgumentsList.AddRange(argumentsList);
    }

    #region Properties

    public string FileName { get; }
    public string WorkingDirectory { get; private set; }
    public List<string> ArgumentsList { get; } = new();
    public List<(string Name, string Value)> EnvVars { get; } = new();
    public List<Action<DataReceivedType, string>> LineReceivedActions { get; } = new();
    public CancellationToken GlobalCancelToken => _globalConfig.GlobalCancelToken;

    #endregion
    
    #region Fluent Set Methods

    public ChildProcessConfig SetWorkingDirectory(string workingDirectory)
    {
        var fullPath = Path.Combine(_globalConfig.DefaultWorkingDirectory, workingDirectory);
        
        if (Directory.Exists(fullPath))
        {
            WorkingDirectory = workingDirectory;
            return this;
        }

        if (Directory.Exists(workingDirectory))
        {
            WorkingDirectory = workingDirectory;
            return this;
        }

        throw new DirectoryNotFoundException(workingDirectory);
    }
    
    public ChildProcessConfig AddArguments(params string[] arguments)
    {
        ArgumentsList.AddRange(arguments);
        return this;
    }
    
    public ChildProcessConfig AddEnvVars(params (string Name, string Value)[] envVars)
    {
        EnvVars.AddRange(envVars);
        return this;
    }
    
    #endregion

    #region LineReceivedActions
    
    public ChildProcessConfig OnLineReceived(Action<string> outputHandler)
    {
        LineReceivedActions.Add((_, line) =>
        {
            outputHandler(line);
        });
        
        return this;
    }
    
    public ChildProcessConfig OnOutputLineReceived(Action<string> outputHandler)
    {
        LineReceivedActions.Add((data, line) =>
        {
            if (data == DataReceivedType.Output)
                outputHandler(line);
        });
        
        return this;
    }
    
    public ChildProcessConfig OnErrorLineReceived(Action<string> outputHandler)
    {
        LineReceivedActions.Add((data, line) =>
        {
            if (data == DataReceivedType.Error)
                outputHandler(line);
        });
        
        return this;
    }
    
    #endregion

    #region IChildProcess

    public Task<int> ToExitCodeAsync(CancellationToken cancelToken = default) =>
        _executor.ToExitCodeAsync(cancelToken);

    public Task<(int ExitCode, string Text)> ToTextAndExitCodeAsync(CancellationToken cancelToken = default) =>
        _executor.ToTextAndExitCodeAsync(cancelToken);

    public Task<(int ExitCode, List<string> Lines)> ToLinesAndExitCodeAsync(CancellationToken cancelToken = default) =>
        _executor.ToLinesAndExitCodeAsync(cancelToken);

    #endregion
    
    public enum DataReceivedType
    {
        Output,
        Error
    }
}
