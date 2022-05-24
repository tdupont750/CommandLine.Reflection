using LsHash.Services.Implementation;

namespace LsHash;

public static class Program
{
    private static readonly string[] Help = {
        "DESCRIPTION",
        "lshash creates a hash of a recursive directory search from the resulting files and their modified timestamp",
        "The intended use is to create a unique version hash of your files",
        "",
        "ARGUMENTS",
        "<Search Path> <File Pattern> <Exclude|Exclude>",
        "Search path is standard path, file pattern may contain wild cards, exclude is just a pipe delimited string",
        "Multiple sets of arguments are supported, but must be in complete groups of 3",
        "",
        "EXAMPLE",
        "'/d/code/src/' '*.cs' 'bin|obj'",
    };
    
    public static int Main(string[] args)
    {
        if (args.Contains("--help"))
        {
            PrintHelp(false);
            return 0;
        }

        if (args.Length == 0 || args.Length % 3 != 0)
        {
            Console.WriteLine($"INVALID ARGUMENTS");
            PrintHelp();
            return 1;
        }
        
        try
        {
            var searchInputs = GetSearchInputs(args);
            var hashes = LsHashService.Instance.GetHashesFromSearchInputs(searchInputs);
            var output =  string.Join(' ', hashes);
            Console.WriteLine(output);
            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine("ERROR");
            Console.WriteLine(ex.GetBaseException());
            PrintHelp();
            return 1;
        }
    }

    private static void PrintHelp(bool addEmptyLine = true)
    {
        if (addEmptyLine)
            Console.WriteLine();
        
        foreach (var line in Help) 
            Console.WriteLine(line);
    }

    private static IReadOnlyList<SearchInput> GetSearchInputs(string[] args)
    {
        var results = new List<SearchInput>();
        
        for (var i = 0; i < args.Length; i += 3)
        {
            results.Add(new SearchInput(
                args[i],
                args[i + 1].Split('|', StringSplitOptions.RemoveEmptyEntries), 
                args[i + 2]));
        }

        return results;
    }
}