using datastructures_project.Search.Trie;

namespace datastructures_project.Search.Index;

public interface IIndex
{
    ITrie Trie { get; }
    string Tag { get; }
    double AverageDoclength { get; }
    void Add(int docId, string[] words);
    int DocumentCount();
    int DocumentWordsCount(int docId);
    (int, int)[] WordDocuments(string word);
    int DocumentLength(int docId);
    List<int> DocumentIds();
    List<string> Tokens(int docId);
    void Remove(int docId);
}