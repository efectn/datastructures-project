using datastructures_project.Search.Distance;

namespace datastructures_project.Search.Trie;

public class Node
{
    public readonly Dictionary<char, Node> children;
    public bool _isEnd;

    public Node()
    {
        children = new Dictionary<char, Node>();
    }
}

public class Trie : ITrie
{
    private readonly Node _head;

    public Trie()
    {
        _head = new Node();
    }

    public void AddWord(string word)
    {
        var tmp = _head;
        foreach (var character in word)
        {
            // If the character is not present in the children, add it
            if (!tmp.children.ContainsKey(character)) tmp.children[character] = new Node();
            tmp = tmp.children[character];
        }

        tmp._isEnd = true;
    }

    public bool SearchWord(string word)
    {
        var tmp = _head;
        foreach (var character in word)
        {
            if (!tmp.children.ContainsKey(character)) return false;
            tmp = tmp.children[character];
        }

        return tmp._isEnd;
    }

    public List<string> GetWords(string prefix)
    {
        var words = new List<string>();
        var tmp = _head;
        foreach (var character in prefix)
        {
            if (!tmp.children.ContainsKey(character)) return new List<string>();

            tmp = tmp.children[character];
        }

        _dfs(tmp, prefix, words);
        return words;
    }
    
    private void _dfs(Node node, string prefix, List<string> words)
    {
        if (node._isEnd) words.Add(prefix);

        foreach (var (character, child) in node.children)
        {
            _dfs(child, prefix + character, words);
        }
    }
    
    public List<string> WildcardSearch(string pattern)
    {
        var results = new List<string>();
        _wildcardSearch(_head, pattern, 0, "", results);
        
        return results;
    }

    private void _wildcardSearch(Node node, string pattern, int index, string current, List<string> results)
    {
        if (index == pattern.Length)
        {
            if (node._isEnd) results.Add(current);
            return;
        }

        char currentChar = pattern[index];
        if (currentChar == '*')
        { 
            // skip the '*'
            _wildcardSearch(node, pattern, index + 1, current, results);
            
            // lookup all children nodes
            foreach (var (childChar, childNode) in node.children)
            {
                _wildcardSearch(childNode, pattern, index, current + childChar, results);
            }
        }
        else
        {
            if (!node.children.ContainsKey(currentChar)) return;

            _wildcardSearch(node.children[currentChar], pattern, index + 1, current + currentChar, results);
        }
    }
    
    public List<string> LevenshteinSearch(string word, int maxDistance = 2)
    {
        var results = new List<string>();
        _levenshteinSearch(_head, word, "", 0, maxDistance, results);
        
        return results;
    }
    
    private void _levenshteinSearch(Node node, string word, string currentWord, int depth, int maxDistance,
        List<string> results)
    {
        if (node._isEnd && LevenshteinDistance.Distance(word, currentWord) <= maxDistance)
            results.Add(currentWord);

        if (depth >= word.Length + maxDistance) return; // Remove irrelevant paths

        foreach (var kvp in node.children)
            _levenshteinSearch(kvp.Value, word, currentWord + kvp.Key, depth + 1, maxDistance, results);
    }
}