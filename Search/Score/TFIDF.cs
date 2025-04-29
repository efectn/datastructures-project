using datastructures_project.Search.Index;

namespace datastructures_project.Search.Score;

public class TFIDF
{
    private readonly IIndex _index;

    public TFIDF(IIndex index)
    {
        _index = index;
    }

    public double calculateIDF(string token)
    {
        var wordDocs = _index.WordDocuments(token);
        if (wordDocs == null) return 0;
        
        var totalDocs = _index.DocumentCount();
        var idf = Math.Log((double)totalDocs/(1+wordDocs.Length))+1; // idf smooth normalization

        return idf;
    }

    public double calculateTF(int docId, int wordFreq)
    {
        return 0.5 + 0.5 * ((double)wordFreq/_index.DocumentWordsCount(docId)); // k = 0.5 normalization
    }

    public Dictionary<int, double> Calculate(string[] tokens)
    {
        var termFreqs = new Dictionary<int, double>();

        foreach (var token in tokens)
        {
            var idf = calculateIDF(token); 
            
            foreach (var (doc, freq) in _index.WordDocuments(token))
            {
                var tfidf = calculateTF(doc, freq) * idf;
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