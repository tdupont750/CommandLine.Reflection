namespace LsHash.Services;

public interface ILsHashService
{
    string[] GetHashesFromSearchInputs(IReadOnlyList<SearchInput> searchInputs);
}