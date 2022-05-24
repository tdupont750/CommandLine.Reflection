namespace LsHash;

public class SearchInput
{
    public SearchInput(string path, IReadOnlyList<string> excludes, string pattern)
    {
        Path = path;
        Excludes = excludes;
        Pattern = pattern;
    }

    public string Path { get; }
    public IReadOnlyList<string> Excludes { get; }
    public string Pattern { get; }
}