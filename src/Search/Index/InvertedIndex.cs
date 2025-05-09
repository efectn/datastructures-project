using System.Collections;

namespace datastructures_project.Search.Index;

using datastructures_project.Search.Trie;
public class InvertedIndex : IIndex
{
    private readonly IDictionary<string, HashSet<(int, int)>> _index; // term -> (docId, termFrequency)
    private readonly ITrie _trie;
    public IDictionary<string, HashSet<(int, int)>> Index => _index;
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
    
    public InvertedIndex(ITrie trie, IDictionary<string, HashSet<(int, int)>> index, string tag)
    {
        _index = index ?? new Dictionary<string, HashSet<(int, int)>>();
        _trie = trie;
        _tag = tag;
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
            
            if (!_index.ContainsKey(word))
            {
                _index[word] = new HashSet<(int, int)>();
            }
            _index[word].Add((docId, termFrequency));
        }
        
        // Update average document length
        var docLength = DocumentLength(docId);
        var newTotalLength = _averageDocLength * (DocumentCount()-1) + docLength;
        _averageDocLength = newTotalLength / DocumentCount();
    }
    
    public int DocumentCount()
    {
        return _index.Values.SelectMany(x => x).Select(p => p.Item1).Distinct().Count();
    }
    
    public int DocumentWordsCount(int docId)
    {
        return _index.Values.SelectMany(x => x).Where(p => p.Item1 == docId).Select(p => p.Item2).Sum();
    }
    
    public (int, int)[] WordDocuments(string word)
    {
        if (_index.ContainsKey(word))
        {
            return _index[word].ToArray();
        }

        return [];
    }
    
    public int DocumentLength(int docId)
    {
        var length = 0;
        foreach (var (_, docs) in _index)
        {
            if (docs.Any(d => d.Item1 == docId))
            {
                length += docs.First(d => d.Item1 == docId).Item2;
            }
        }
        
        return length;
    }
    
    public List<int> DocumentIds()
    {
        return _index.Values.SelectMany(x => x).Select(p => p.Item1).Distinct().ToList();
    }
    
    public List<string> Tokens(int docId)
    {
        var tokens = new List<string>();
        foreach (var (word, docs) in _index)
        {
            if (docs.Any(d => d.Item1 == docId))
            {
                tokens.AddRange(Enumerable.Repeat(word, docs.First(d => d.Item1 == docId).Item2));
            }
        }
        
        return tokens;
    }
    
    public void Remove(int docId)
    {
        // Update average document length
        var docLength = DocumentLength(docId);
        if (docLength == 0) return; // Document not found
        
        var newTotalLength = _averageDocLength * DocumentCount() - docLength;
        _averageDocLength = DocumentCount() > 1 ? newTotalLength / (DocumentCount()-1) : 0;

        // Remove the document from the list of each word
        foreach (var (_, docs) in _index)
        {
            // Skip if the document is not in the postings list
            if (!docs.Any(d => d.Item1 == docId)) continue;
            
            // TODO: remove the words from trie in case they are not used by any other document
            // Remove the document from the postings list of each word
            docs.RemoveWhere(d => d.Item1 == docId);
        }
    }
}