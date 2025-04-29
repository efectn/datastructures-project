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

        dfs(tmp, prefix, words);
        return words;
    }
    
    private void dfs(Node node, string prefix, List<string> words)
    {
        if (node._isEnd) words.Add(prefix);

        foreach (var (character, child) in node.children)
        {
            dfs(child, prefix + character, words);
        }
    }
}