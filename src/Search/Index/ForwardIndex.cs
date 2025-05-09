using datastructures_project.Search.Trie;

namespace datastructures_project.Search.Index;

public class ForwardIndex : IIndex
{
    private readonly IDictionary<int, HashSet<(string, int)>> _index; // term -> (word, termFrequency)
    private readonly ITrie _trie;
    public IDictionary<int, HashSet<(string, int)>> Index => _index;
    private double _averageDocLength;
    private string _tag;
    
    public double AverageDoclength
    {
        get => _averageDocLength;
    }
    
    public ITrie Trie
    {
        get => _trie;
    }
    
    public string Tag => _tag;
    
    public ForwardIndex(ITrie trie, IDictionary<int, HashSet<(string, int)>> index, string tag)
    {
        _index = index ?? new Dictionary<int, HashSet<(string, int)>>();
        _trie = trie;
        _tag = tag;
    }
    
    public ForwardIndex(ITrie trie)
    {
        _index = new Dictionary<int, HashSet<(string, int)>>();
        _trie = trie;
        _tag = "";
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
        
        // Update average document length
        var docLength = DocumentLength(docId);
        var newTotalLength = _averageDocLength * (DocumentCount()-1) + docLength;
        _averageDocLength = newTotalLength / DocumentCount();
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
    
    public (int, int)[] WordDocuments(string word)
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
            return _index[docId].Select(p => p.Item2).Sum();
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
    
    public void Remove(int docId)
    {
        if (_index.ContainsKey(docId))
        {
            // Update average document length
            var docLength = DocumentLength(docId);
            var newTotalLength = _averageDocLength * DocumentCount() - docLength;
            _averageDocLength = DocumentCount() > 1 ? newTotalLength / (DocumentCount() - 1) : 0;
            
            // TODO: remove the words from trie in case they are not used by any other document
            // Remove the document
            _index.Remove(docId);
        }
    }
}