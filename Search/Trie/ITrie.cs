namespace datastructures_project.Search.Trie;

public interface ITrie
{
    void AddWord(string word);
    bool SearchWord(string word);
    List<string> GetWords(string prefix);
    List<string> WildcardSearch(string pattern);
    List<string> LevenshteinSearch(string word, int maxDistance = 2);
    List<string> GetTokens(List<string> tokens);
}