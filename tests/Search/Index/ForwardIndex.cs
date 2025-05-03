namespace tests.Search.Index;

public class ForwardIndexTest
{
    [Fact]
    public void ForwardIndex_Add()
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
        
        // Check the document count
        Assert.Equal(4, forwardIndex.DocumentCount());
        
        // Check the document words count
        Assert.Equal(2, forwardIndex.DocumentWordsCount(1));
        Assert.Equal(2, forwardIndex.DocumentWordsCount(2));
        Assert.Equal(2, forwardIndex.DocumentWordsCount(3));
        Assert.Equal(5, forwardIndex.DocumentWordsCount(4));
        
        // Check the average document length
        Assert.Equal(14.25, forwardIndex.AverageDoclength, 2);
    }

    [Fact]
    public void ForwardIndex_DocumentLength()
    {
        // Initialize the trie
        var trie = new datastructures_project.Search.Trie.Trie();
        
        // Initialize the forward index
        var forwardIndex = new datastructures_project.Search.Index.ForwardIndex(trie);
        
        // Add some documents to the forward index
        forwardIndex.Add(1, new[] {"hello", "world"});
        forwardIndex.Add(2, new[] {"hello", "everyone"});
        forwardIndex.Add(3, new[] {"hello", "hello", "everyone", "everyone"});

        // Check the document length
        Assert.Equal(10, forwardIndex.DocumentLength(1));
        Assert.Equal(13, forwardIndex.DocumentLength(2));
        Assert.Equal(26, forwardIndex.DocumentLength(3));
    }

    [Fact]
    public void ForwardIndex_DocumentIds()
    {
        // Initialize the trie
        var trie = new datastructures_project.Search.Trie.Trie();
        
        // Initialize the forward index
        var forwardIndex = new datastructures_project.Search.Index.ForwardIndex(trie);
        
        // Add some documents to the forward index
        forwardIndex.Add(1, new[] {"hello", "world"});
        forwardIndex.Add(2, new[] {"hello", "everyone"});
        forwardIndex.Add(3, new[] {"goodbye", "world"});
        
        // Check the document IDs for a word
        Assert.Contains(("hello", 1), forwardIndex.Index[1]);
        Assert.Contains(("hello", 1), forwardIndex.Index[2]);
        Assert.Contains(("goodbye", 1), forwardIndex.Index[3]);
        Assert.Contains(("world", 1), forwardIndex.Index[1]);
        Assert.Contains(("world", 1), forwardIndex.Index[3]);
        Assert.Contains(("everyone", 1), forwardIndex.Index[2]);
        

        var ids = forwardIndex.DocumentIds();
        Assert.Contains(1, ids);
        Assert.Contains(2, ids);
        Assert.Contains(3, ids);
        
        Assert.Equal(3, ids.Count);
    }

    [Fact]
    public void ForwardIndex_Tokens()
    {
        // Initialize the trie
        var trie = new datastructures_project.Search.Trie.Trie();
        
        // Initialize the forward index
        var forwardIndex = new datastructures_project.Search.Index.ForwardIndex(trie);
        
        // Add some documents to the forward index
        forwardIndex.Add(1, new[] {"hello", "world", "hello", "test", "world"});
        forwardIndex.Add(2, new[] {"hello", "everyone"});
        forwardIndex.Add(3, new[] {"goodbye", "world"});
        
        // Check the tokens
        var tokens = forwardIndex.Tokens(1);
        Assert.Contains("hello", tokens);
        Assert.Contains("world", tokens);
        Assert.Contains("test", tokens);
        Assert.Equal(2, tokens.Count(t => t == "hello"));
        Assert.Equal(2, tokens.Count(t => t == "world"));
        
        Assert.Equal(5, tokens.Count);
        
        tokens = forwardIndex.Tokens(2);
        Assert.Contains("hello", tokens);
        Assert.Contains("everyone", tokens);
        
        Assert.Equal(2, tokens.Count);
        
        tokens = forwardIndex.Tokens(3);
        Assert.Contains("goodbye", tokens);
        Assert.Contains("world", tokens);
        
        Assert.Equal(2, tokens.Count);
    }
    
    [Fact]
    public void ForwardIndex_Remove_AverageDocLength()
    {
        // Initialize the trie
        var trie = new datastructures_project.Search.Trie.Trie();
        
        // Initialize the forward index
        var forwardIndex = new datastructures_project.Search.Index.ForwardIndex(trie);
        
        Assert.Equal(0, forwardIndex.AverageDoclength);
        
        // Add some documents to the forward index
        forwardIndex.Add(1, new[] {"hello", "world"});
        Assert.Equal(10.0, forwardIndex.AverageDoclength, 0.1);
        
        forwardIndex.Add(2, new[] {"hello", "everyone", "hello", "everyone"});
        Assert.Equal(18, forwardIndex.AverageDoclength, 0.1);
        
        forwardIndex.Add(3, new[] {"goodbye", "world", "test"});
        Assert.Equal(17.33, forwardIndex.AverageDoclength, 0.1);
        
        // Remove non-existing document
        forwardIndex.Remove(15);
        Assert.Equal(17.33, forwardIndex.AverageDoclength, 0.1);
        
        // Remove a document
        forwardIndex.Remove(1);
        Assert.Equal(21, forwardIndex.AverageDoclength, 0.1);
        
        // Remove another document
        forwardIndex.Remove(2);
        Assert.Equal(16.0, forwardIndex.AverageDoclength, 0.1);
        
        // Remove the last document
        forwardIndex.Remove(3);
        Assert.Equal(0.0, forwardIndex.AverageDoclength, 0.1);
        
        // Remove the last document again
        forwardIndex.Remove(3);
        Assert.Equal(0.0, forwardIndex.AverageDoclength, 0.1);
    }

    [Fact]
    public void ForwardIndex_Trie_WordsAdded()
    {
        // Initialize the trie
        var trie = new datastructures_project.Search.Trie.Trie();
        
        // Initialize the forward index
        var forwardIndex = new datastructures_project.Search.Index.ForwardIndex(trie);
        
        // Add some documents to the forward index
        forwardIndex.Add(1, new[] {"hello", "world"});
        forwardIndex.Add(2, new[] {"hello", "everyone"});
        forwardIndex.Add(3, new[] {"goodbye", "world"});
        
        // Check the words in the trie
        Assert.Contains("hello", trie.GetWords("h"));
        Assert.Contains("world", trie.GetWords("w"));
        Assert.Contains("everyone", trie.GetWords("e"));
        Assert.Contains("goodbye", trie.GetWords("g"));
        
        Assert.DoesNotContain("test", trie.GetWords("t"));
        Assert.DoesNotContain("everyone", trie.GetWords("g"));
        
        Assert.Equal(1, trie.GetWords("h").Count);
        Assert.Equal(1, trie.GetWords("w").Count);
        Assert.Equal(1, trie.GetWords("e").Count);
    }
    
    [Fact]
    public void ForwardIndex_EmptyIndex()
    {
        // Initialize the trie
        var trie = new datastructures_project.Search.Trie.Trie();
        
        // Initialize the forward index
        var forwardIndex = new datastructures_project.Search.Index.ForwardIndex(trie);
        
        // Check the document count
        Assert.Equal(0, forwardIndex.DocumentCount());
        
        // Check the average document length
        Assert.Equal(0, forwardIndex.AverageDoclength);
        
        // Check the document length
        Assert.Equal(0, forwardIndex.DocumentLength(1));
    }
}