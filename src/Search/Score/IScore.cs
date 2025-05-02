using datastructures_project.Search.Trie;

namespace datastructures_project.Search.Score;

public interface IScore
{
    ITrie Trie { get; }
    public Dictionary<int, double> Calculate(string[] tokens);
}