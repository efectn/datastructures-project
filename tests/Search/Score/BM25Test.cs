namespace tests.Search.Score;

public class BM25Test
{
    [Fact]
    public void BM25_Calculate()
    {
        // Initialize the trie
        var trie = new datastructures_project.Search.Trie.Trie();
        
        // Initialize the forward index
        var forwardIndex = new datastructures_project.Search.Index.ForwardIndex(trie);
        
        // Add some documents to the forward index
        forwardIndex.Add(1, new[] {"hello", "world"});
        forwardIndex.Add(2, new[] {"hello", "everyone"});
        forwardIndex.Add(3, new[] {"goodbye", "world"});
        forwardIndex.Add(4, new[] {"hello", "john", "doe", "world", "hello"});
        
        // Check average document length
        Assert.Equal(2.75, forwardIndex.AverageDoclength, 0.01);
        
        // Initialize the BM25 scorer
        var bm25 = new datastructures_project.Search.Score.BM25(forwardIndex);
        
        // Calculate BM25 for hello, world separately
        var helloIdf = bm25.Calculate(new[] {"hello"});
        var worldIdf = bm25.Calculate(new[] {"world"});
        
        // Check the scores
        Assert.Equal(0.4, helloIdf[1], 0.01);
        Assert.Equal(0.4, helloIdf[2], 0.01);
        Assert.Equal(0.39, helloIdf[4], 0.01);
        
        Assert.Equal(0.4, worldIdf[1], 0.01);
        Assert.Equal(0.40, worldIdf[3], 0.01);
        Assert.Equal(0.267, worldIdf[4], 0.01);
        
        
        // Calculate the BM25 scores for a set of tokens
        var tokens = new[] {"hello", "world"};
        var scores = bm25.Calculate(tokens);
        
        // Check the scores
        Assert.Equal(helloIdf[1]+worldIdf[1], scores[1]);
        Assert.Equal(helloIdf[2], scores[2]);
        Assert.Equal(worldIdf[3], scores[3]);
        
        Assert.Equal(helloIdf[4]+worldIdf[4], scores[4]);
        
    }

    [Fact]
    public void BM25_Calculate_IndexRemove()
    {
        // Initialize the trie
        var trie = new datastructures_project.Search.Trie.Trie();
        
        // Initialize the forward index
        var forwardIndex = new datastructures_project.Search.Index.ForwardIndex(trie);
        
        // Add some documents to the forward index
        forwardIndex.Add(1, new[] {"hello", "world"});
        forwardIndex.Add(2, new[] {"hello", "everyone"});
        forwardIndex.Add(3, new[] {"goodbye", "world"});
        forwardIndex.Add(4, new[] {"hello", "john", "doe", "world", "hello"});
        
        // Initialize the BM25 scorer
        var bm25 = new datastructures_project.Search.Score.BM25(forwardIndex);
        
        // Calculate BM25 scores before removing a document
        var scores = bm25.Calculate(["hello"]);
        Assert.Equal(0.4, scores[1], 0.01);
        Assert.Equal(0.4, scores[2], 0.01);
        Assert.Equal(0.39, scores[4], 0.05);
        
        // Remove a document from the forward index
        forwardIndex.Remove(1);
        
        // Calculate the BM25 scores for a set of tokens
        scores = bm25.Calculate(["hello"]);
        
        // Check the scores
        Assert.Equal(0.54, scores[2], 0.01);
        Assert.Equal(0.54, scores[4], 0.01);
        
        // Remove another document from the forward index
        forwardIndex.Remove(2);
        
        // Calculate the BM25 scores for a set of tokens
        scores = bm25.Calculate(["hello"]);
        
        // Check the scores
        Assert.Equal(0.85, scores[4], 0.01);
        
        // Remove the last document from the forward index
        forwardIndex.Remove(4);
        
        // Calculate the BM25 scores for a set of tokens
        scores = bm25.Calculate(["hello"]);
        Assert.Empty(scores);
    }
    
    [Fact]
    public void BM25_Calculate_Empty()
    {
        // Initialize the trie
        var trie = new datastructures_project.Search.Trie.Trie();
        
        // Initialize the forward index
        var forwardIndex = new datastructures_project.Search.Index.ForwardIndex(trie);
        
        // Add some documents to the forward index
        forwardIndex.Add(1, new[] {"hello", "world"});
        forwardIndex.Add(2, new[] {"hello", "everyone"});
        forwardIndex.Add(3, new[] {"goodbye", "world"});
        forwardIndex.Add(4, new[] {"hello", "john", "doe", "world", "hello"});
        
        // Initialize the BM25 scorer
        var bm25 = new datastructures_project.Search.Score.BM25(forwardIndex);
        
        // Calculate the BM25 scores for a set of tokens
        var tokens = Array.Empty<string>();
        var scores = bm25.Calculate(tokens);
        
        // Check the scores
        Assert.Empty(scores);
    }
    
    [Fact]
    public void BM25_Calculate_Null()
    {
        // Initialize the trie
        var trie = new datastructures_project.Search.Trie.Trie();
        
        // Initialize the forward index
        var forwardIndex = new datastructures_project.Search.Index.ForwardIndex(trie);
        
        // Add some documents to the forward index
        forwardIndex.Add(1, new[] {"hello", "world"});
        forwardIndex.Add(2, new[] {"hello", "everyone"});
        forwardIndex.Add(3, new[] {"goodbye", "world"});
        forwardIndex.Add(4, new[] {"hello", "john", "doe", "world", "hello"});
        
        // Initialize the BM25 scorer
        var bm25 = new datastructures_project.Search.Score.BM25(forwardIndex);
        
        // Calculate the BM25 scores for a set of tokens
        string[] tokens = null;
        Assert.Throws<NullReferenceException>(() => bm25.Calculate(tokens));
    }
    
    [Fact]
    public void BM25_Calculate_Empty_Token()
    {
        // Initialize the trie
        var trie = new datastructures_project.Search.Trie.Trie();
        
        // Initialize the forward index
        var forwardIndex = new datastructures_project.Search.Index.ForwardIndex(trie);
        
        // Add some documents to the forward index
        forwardIndex.Add(1, new[] {"hello", "world"});
        forwardIndex.Add(2, new[] {"hello", "everyone"});
        forwardIndex.Add(3, new[] {"goodbye", "world"});
        forwardIndex.Add(4, new[] {"hello", "john", "doe", "world", "hello"});
        
        // Initialize the BM25 scorer
        var bm25 = new datastructures_project.Search.Score.BM25(forwardIndex);
        
        // Calculate the BM25 scores for a set of tokens
        var tokens = new[] {"hello", ""};
        var scores = bm25.Calculate(tokens);
        
        // Check the scores
        Assert.Equal(0.4, scores[1], 0.01);
        Assert.Equal(0.4, scores[2], 0.01);
        Assert.Equal(0.39, scores[4], 0.01);
    }
}