using System.Net;
using System.Text.RegularExpressions;
using datastructures_project.Search.Tokenizer.Snowball;

namespace datastructures_project.Search.Tokenizer;

public class Tokenizer : ITokenizer
{
    private readonly string[] stopWords;
    private readonly string[] protectedWords;
    private readonly TurkishStemmer stemmer;
    private readonly IConfiguration configuration;
    
    public Tokenizer(IConfiguration configuration, string stopWordsFile = "Resources/stop-words.txt", string protectedWordsFile = "Resources/stemmer-protected-words.txt")
    {
        this.configuration = configuration;
        
        // Initialize stemmer
        stemmer = new TurkishStemmer();
        
        // Check if the file exists
        if (!File.Exists(stopWordsFile))
        {
            stopWords = new string[0];
        }
        else
        {
            // Read the stop words from the file
            var stopWordsText = File.ReadAllText(stopWordsFile);
            stopWords = stopWordsText.Split(new[] { ' ', '\t', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        }
        
        // Check if the file exists
        if (!File.Exists(protectedWordsFile))
        {
            protectedWords = new string[0];
        } else {
            var protectedWordsText = File.ReadAllText(protectedWordsFile);
            protectedWords = protectedWordsText.Split(new[] { ' ', '\t', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
    
    public List<string> Tokenize(string text)
    {
        var enableStemmer = Boolean.Parse(configuration["Search:Tokenizer:EnableStemmer"]);
        var enableStopWords = Boolean.Parse(configuration["Search:Tokenizer:EnableStopWords"]);
        
        // Normalize the text to lowercase
        text = text.ToLower().Trim();
        
        // Remove punctuation and special characters except * which is necessary for wildcard search
        text = Regex.Replace(text, @"[^a-zA-ZçÇğĞıİöÖşŞüÜ *]+", "");

        // Split the text into tokens
        var tokens = text.Split(new[] { ' ', '\t', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        // Remove stop words
        var filteredTokens = tokens.Where(token => enableStopWords ? !stopWords.Contains(token) : true).ToList();

        // Stem the tokens and return the result
        return filteredTokens.Select(token => !protectedWords.Contains(token) && enableStemmer ? stemmer.Stem(token) : token).ToList();
    }
}