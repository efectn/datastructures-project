namespace tests.Search.Score;

public class TFIDF
{
    [Fact]
    public void TFIDF_Calculate()
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
        
        // Initialize the TFIDF scorer
        var tfidf = new datastructures_project.Search.Score.TFIDF(forwardIndex);
        
        // Calculate TFIDF for hello, world separately
        var helloIdf = tfidf.Calculate(new[] {"hello"});
        var worldIdf = tfidf.Calculate(new[] {"world"});
        
        // Check the scores
        Assert.Equal(0.75, helloIdf[1]);
        Assert.Equal(0.75, helloIdf[2]);
        Assert.Equal(0.69, helloIdf[4], 0.1);
        
        Assert.Equal(0.75, worldIdf[1]);
        Assert.Equal(0.75, worldIdf[3]);
        Assert.Equal(0.69, worldIdf[4], 0.1);
        
        
        // Calculate the TFIDF scores for a set of tokens
        var tokens = new[] {"hello", "world"};
        var scores = tfidf.Calculate(tokens);
        
        // Check the scores
        Assert.Equal(helloIdf[1]+worldIdf[1], scores[1]);
        Assert.Equal(helloIdf[2], scores[2]);
        Assert.Equal(worldIdf[3], scores[3]);
        
        Assert.Equal(helloIdf[4]+worldIdf[4], scores[4]);
        
    }

    [Fact]
    public void TFIDF_Calculate_IndexRemove()
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
        
        // Initialize the TFIDF scorer
        var tfidf = new datastructures_project.Search.Score.TFIDF(forwardIndex);
        
        // Calculate TFIDF scores before removing a document
        var scores = tfidf.Calculate(["hello"]);
        Assert.Equal(0.75, scores[1]);
        Assert.Equal(0.75, scores[2]);
        Assert.Equal(0.69, scores[4], 0.1);
        
        // Remove a document from the forward index
        forwardIndex.Remove(1);
        
        // Calculate the TFIDF scores for a set of tokens
        scores = tfidf.Calculate(["hello"]);
        
        // Check the scores
        Assert.Equal(0.75, scores[2]);
        Assert.Equal(0.69, scores[4], 0.1);
        
        // Remove another document from the forward index
        forwardIndex.Remove(2);
        
        // Calculate the TFIDF scores for a set of tokens
        scores = tfidf.Calculate(["hello"]);
        
        // Check the scores
        Assert.Equal(0.69, scores[4], 0.1);
        
        // Remove the last document from the forward index
        forwardIndex.Remove(4);
        
        // Calculate the TFIDF scores for a set of tokens
        scores = tfidf.Calculate(["hello"]);
        Assert.Empty(scores);
    }
    
    [Fact]
    public void TFIDF_Calculate_Empty()
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
        
        // Initialize the TFIDF scorer
        var tfidf = new datastructures_project.Search.Score.TFIDF(forwardIndex);
        
        // Calculate the TFIDF scores for a set of tokens
        var tokens = Array.Empty<string>();
        var scores = tfidf.Calculate(tokens);
        
        // Check the scores
        Assert.Empty(scores);
    }
    
    [Fact]
    public void TFIDF_Calculate_Null()
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
        
        // Initialize the TFIDF scorer
        var tfidf = new datastructures_project.Search.Score.TFIDF(forwardIndex);
        
        // Calculate the TFIDF scores for a set of tokens
        string[] tokens = null;
        Assert.Throws<NullReferenceException>(() => tfidf.Calculate(tokens));
    }
    
    [Fact]
    public void TFIDF_Calculate_Empty_Token()
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
        
        // Initialize the TFIDF scorer
        var tfidf = new datastructures_project.Search.Score.TFIDF(forwardIndex);
        
        // Calculate the TFIDF scores for a set of tokens
        var tokens = new[] {"hello", ""};
        var scores = tfidf.Calculate(tokens);
        
        // Check the scores
        Assert.Equal(0.75, scores[1]);
        Assert.Equal(0.75, scores[2]);
        Assert.Equal(0.69, scores[4], 0.1);
    }
}