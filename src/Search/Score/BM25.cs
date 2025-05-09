using datastructures_project.Search.Index;
using datastructures_project.Search.Trie;

namespace datastructures_project.Search.Score;

public class BM25 : IScore
{
    private readonly IIndex _index;
    private readonly double _k1;
    private readonly double _b;
    private readonly string _tag;
    
    public ITrie Trie => _index.Trie;
    public string Tag => _tag;

    public BM25(IIndex index, string tag)
    {
        _index = index;
        _k1 = 1.2;
        _b = 0.75;
        _tag = tag;
    }

    public double CalculateIDF(string token)
    {
        var wordDocs = _index.WordDocuments(token);
        if (wordDocs == null) return 0;

        var totalDocs = _index.DocumentCount();
        return Math.Log((totalDocs - wordDocs.Length + 0.5) / (wordDocs.Length + 0.5) + 1); // +1 for smoothing
    }

    public Dictionary<int, double> Calculate(string[] tokens)
    {
        var termFreqs = new Dictionary<int, double>();

        foreach (var token in tokens)
        {
            var idf = CalculateIDF(token);

            foreach (var (doc, freq) in _index.WordDocuments(token))
            {
                var docLen = _index.DocumentLength(doc);
                var score = idf * (freq * (_k1 + 1) / (freq + _k1 * (1 - _b + _b * docLen / _index.AverageDoclength)));

                if (termFreqs.ContainsKey(doc))
                {
                    termFreqs[doc] += score;
                }
                else
                {
                    termFreqs[doc] = score;
                }
            }
        }

        return termFreqs;
    }
}