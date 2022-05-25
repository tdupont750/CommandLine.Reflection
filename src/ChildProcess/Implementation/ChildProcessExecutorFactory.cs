using ChildProcess.Configuration;

namespace ChildProcess.Implementation;

public class ChildProcessExecutorFactory : IChildProcessFactory
{
    private readonly GlobalChildProcessConfig _config;

    public ChildProcessExecutorFactory(GlobalChildProcessConfig config) => _config = config;
    
    public ChildProcessConfig Exe(string fileName, IEnumerable<string> arguments)
    {
        if (!File.Exists(fileName))
            throw new FileNotFoundException(fileName);
        
        return new(fileName,
            arguments,
            _config,
            config => new ChildProcessExecutor(config));
    }

    public ChildProcessConfig Sh(IEnumerable<string> arguments)
    {
        if (string.IsNullOrWhiteSpace(_config.ShFilePath))
            throw new NotSupportedException(
                $"{nameof(GlobalChildProcessConfig)}.{nameof(GlobalChildProcessConfig.ShFilePath)} not set");
        
        var argumentsList = new List<string>
        {
            // login
            "-l",
            // command
            "-c"
        };
        
        argumentsList.AddRange(arguments);

        return new ChildProcessConfig(
            _config.ShFilePath,
            argumentsList,
            _config,
            config => new ChildProcessExecutor(config));
    }
}
