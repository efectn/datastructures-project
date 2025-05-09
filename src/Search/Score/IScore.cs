using datastructures_project.Search.Trie;

namespace datastructures_project.Search.Score;

public interface IScore
{
    ITrie Trie { get; }
    string Tag { get; }
    public Dictionary<int, double> Calculate(string[] tokens);
}