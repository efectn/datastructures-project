using datastructures_project.Search.Index;
using datastructures_project.Search.Trie;

namespace datastructures_project.Search.Score;

public class TFIDF: IScore
{
    private readonly IIndex _index;
    private readonly string _tag;
    
    public ITrie Trie => _index.Trie;
    public string Tag => _tag;

    public TFIDF(IIndex index, string tag)
    {
        _index = index;
        _tag = tag;
    }
    
    public TFIDF(IIndex index)
    {
        _index = index;
        _tag = "";
    }

    private double _calculateIdf(string token, int docsCount)
    {
        var wordDocs = _index.WordDocuments(token);
        if (wordDocs == null) return 0;
        
        var idf = Math.Log((double)docsCount/(1+wordDocs.Length))+1; // idf smooth normalization

        return idf;
    }

    private double _calculateTf(int docId, int wordFreq)
    {
        return 0.5 + 0.5 * ((double)wordFreq/_index.DocumentWordsCount(docId)); // k = 0.5 normalization
    }

    public Dictionary<int, double> Calculate(string[] tokens)
    {
        var termFreqs = new Dictionary<int, double>();
        var totalDocsCount = _index.DocumentCount();

        foreach (var token in tokens)
        {
            var idf = _calculateIdf(token, totalDocsCount); 
            
            foreach (var (doc, freq) in _index.WordDocuments(token))
            {
                var tfidf = _calculateTf(doc, freq) * idf;
                if (termFreqs.ContainsKey(doc))
                {
                    termFreqs[doc] += tfidf;
                }
                else
                {
                    termFreqs[doc] = tfidf;
                }
            }
        }

        return termFreqs;
    }
}