using System.Buffers;
using System.Security.Cryptography;

namespace LsHash.Services.Implementation;

public class LsHashService : ILsHashService
{
    public static readonly ILsHashService Instance = new LsHashService();
    
    private static readonly DateTime Epoch = new(1970, 1, 1);
    private static readonly int BytesPerMd5;
    private static readonly int BytesPerInt;

    static LsHashService()
    {
        using var md5 = MD5.Create();
        BytesPerMd5 = md5.HashSize / 8;
        BytesPerInt = BitConverter.GetBytes(default(int)).Length;
    }
    
    public string[] GetHashesFromSearchInputs(IReadOnlyList<SearchInput> searchInputs)
    {
        var hashes = new string[searchInputs.Count];

        if (searchInputs.Count == 1)
            ProcessSearchInput(0);

        Parallel.For(0, searchInputs.Count, ProcessSearchInput);

        return hashes;

        void ProcessSearchInput(int index)
        {
            var results = GetResultsFromSearchInputs(searchInputs[index]);
            var bytes = GetBytesFromSearchResults(results);
            var hash = GetHashFromBytes(bytes);
            hashes[index] = $"{results.Length.ToString()}_{hash}";
        }
    }

    private static SearchResult[] GetResultsFromSearchInputs(SearchInput searchInput)
    {
        return Directory
            .EnumerateFiles(searchInput.Path, searchInput.Pattern, SearchOption.AllDirectories)
            .AsParallel()
            .Select(f => f.Replace('\\', '/'))
            .Where(f => !searchInput.Excludes.Any(f.Contains))
            .Select(f =>
            {
                var lastWrite = File.GetLastWriteTime(f);
                var timeSpan = lastWrite - Epoch;
                var epoch = Convert.ToInt32(timeSpan.TotalSeconds);
                return new SearchResult(f, epoch);
            })
            .OrderBy(f => f.FullName)
            .ToArray();
    }

    private static byte[] GetBytesFromSearchResults(SearchResult[] files)
    {
        var allBytes = new byte[BytesPerInt * files.Length];
        
        for (var index = 0; index < files.Length; index++)
        {
            var file = files[index];
            var startIndex = index * BytesPerInt;
            var secondsSpan = allBytes.AsSpan(startIndex, BytesPerInt);
            if (!BitConverter.TryWriteBytes(secondsSpan, file.LastWriteEpoch))
                throw new InvalidOperationException("Unable to write seconds bytes");
        }
        
        return allBytes;
    }

    private static string GetHashFromBytes(byte[] bytes)
    {
        using var md5 = MD5.Create();
        using var hashLease = MemoryPool<byte>.Shared.Rent(BytesPerMd5);
        
        if (!md5.TryComputeHash(bytes, hashLease.Memory.Span, out var hashBytesWritten))
            throw new ArgumentOutOfRangeException(nameof(hashLease), "Pooled array for MD5 hash computation not big enough");

        return Convert.ToHexString(hashLease.Memory.Span[..hashBytesWritten]).ToLowerInvariant();
    }

    private class SearchResult
    {
        public SearchResult(string fullName, int lastWriteEpoch)
        {
            FullName = fullName;
            LastWriteEpoch = lastWriteEpoch;
        }

        public string FullName { get; }
        public int LastWriteEpoch { get; }

    }
}