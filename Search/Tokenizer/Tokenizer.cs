using System.Text.RegularExpressions;

namespace datastructures_project.Search.Tokenizer;

public class Tokenizer
{
    private readonly string[] stop_words;
    
    public Tokenizer(string file = "Resources/stop-words.txt")
    {
        // Check if the file exists
        if (!File.Exists(file))
        {
            stop_words = new string[0];
        }
        
        // Read the stop words from the file
        var stopWordsText = File.ReadAllText(file);
        stop_words = stopWordsText.Split(new[] { ' ', '\t', '\n' }, StringSplitOptions.RemoveEmptyEntries);
    }
    
    public List<string> Tokenize(string text)
    {
        // Normalize the text to lowercase
        text = text.ToLower();

        // Remove punctuation and special characters
        text = Regex.Replace(text, @"[^\w\s]", "");
            
        // Split the text into tokens
        var tokens = text.Split(new[] { ' ', '\t', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        // Remove stop words
        var filteredTokens = tokens.Where(token => !stop_words.Contains(token)).ToList();

        return filteredTokens;
    }
}