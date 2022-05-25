namespace ChildProcess.Configuration;

public class GlobalChildProcessConfig
{
    public static GlobalChildProcessConfig Create(
        CancellationToken globalCancelToken = default)
    {
        return new GlobalChildProcessConfig(Environment.CurrentDirectory, string.Empty, globalCancelToken);
    }
    
    public static GlobalChildProcessConfig Create(
        string workingDirectory,
        string shPath,
        CancellationToken globalCancelToken = default)
    {
        if (!Directory.Exists(workingDirectory))
            throw new DirectoryNotFoundException(workingDirectory);
        
        if (!File.Exists(shPath))
            throw new FileNotFoundException(shPath);
        
        return new GlobalChildProcessConfig(workingDirectory, shPath, globalCancelToken);
    }
    
    public static GlobalChildProcessConfig CreateWithSh(
        string shPath,
        CancellationToken globalCancelToken = default)
    {
        if (!File.Exists(shPath))
            throw new FileNotFoundException(shPath);
        
        return new GlobalChildProcessConfig(Environment.CurrentDirectory, shPath, globalCancelToken);
    }
    
    public static GlobalChildProcessConfig CreateWithWorkingDirectory(
        string workingDirectory,
        CancellationToken globalCancelToken = default)
    {
        if (!Directory.Exists(workingDirectory))
            throw new DirectoryNotFoundException(workingDirectory);

        return new GlobalChildProcessConfig(workingDirectory, string.Empty, globalCancelToken);
    }
    
    private GlobalChildProcessConfig(
        string defaultWorkingDirectory,
        string shFilePath,
        CancellationToken globalCancelToken)
    {
        DefaultWorkingDirectory = defaultWorkingDirectory;
        ShFilePath = shFilePath;
        GlobalCancelToken = globalCancelToken;
    }

    public string DefaultWorkingDirectory { get; }
    public string ShFilePath { get; }
    public CancellationToken GlobalCancelToken { get; }
}
