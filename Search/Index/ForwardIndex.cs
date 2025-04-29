using datastructures_project.Search.Trie;

namespace datastructures_project.Search.Index;

public class ForwardIndex : IIndex
{
    private readonly Dictionary<int, HashSet<(string, int)>> _index; // term -> (word, termFrequency)
    private readonly ITrie _trie;
    public Dictionary<int, HashSet<(string, int)>> Index => _index;
    
    public ForwardIndex(ITrie trie)
    {
        _index = new Dictionary<int, HashSet<(string, int)>>();
        _trie = trie;
    }
    
    public void Add(int docId, string[] words)
    {
        // Count the term frequency of each word in the document and remove duplications
        var newWords = words.Select(word => (word, words.Count(w => w == word)))
            .Distinct()
            .ToArray();
        
        foreach (var (word, termFrequency) in newWords) 
        {
            // Add the word to the trie
            _trie.AddWord(word);
            
            if (!_index.ContainsKey(docId))
            {
                _index[docId] = new HashSet<(string, int)>();
            }
            _index[docId].Add((word, termFrequency));
        }
    }
    
    public int DocumentCount()
    {
        return _index.Count;
    }
    
    public int DocumentWordsCount(int docId)
    {
        if (_index.ContainsKey(docId))
        {
            return _index[docId].Select(p => p.Item2).Sum();
        }
        return 0;
    }
    
    public (int, int)[]? WordDocuments(string word)
    {
        var result = new List<(int, int)>();
        foreach (var (docId, docWords) in _index)
        {
            foreach (var (w, termFrequency) in docWords)
            {
                if (w == word)
                {
                    result.Add((docId, termFrequency));
                }
            }
        }
        
        return result.ToArray();
    }
    
    public int DocumentLength(int docId)
    {
        if (_index.ContainsKey(docId))
        {
            var length = 0;
            
            return _index[docId].Select(p => p.Item2 * p.Item1.Length).Sum();
        }
        return 0;
    }
    
    public List<int> DocumentIds()
    {
        return _index.Keys.ToList();
    }
    
    public List<string> Tokens(int docId)
    {
        var tokens = new List<string>();
        if (_index.ContainsKey(docId))
        {
            foreach (var (word, termFrequency) in _index[docId])
            {
                tokens.AddRange(Enumerable.Repeat(word, termFrequency));
            }
        }
        
        return tokens;
    }
}