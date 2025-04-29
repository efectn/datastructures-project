namespace datastructures_project.Search.Trie;

public interface ITrie
{
    void AddWord(string word);
    bool SearchWord(string word);
    List<string> GetWords(string prefix);
}