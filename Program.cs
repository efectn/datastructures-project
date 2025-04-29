using datastructures_project.Search.Index;
using datastructures_project.Search.Score;
using datastructures_project.Search.Tokenizer;
using datastructures_project.Search.Trie;

var tokenizer = new Tokenizer();

var trie = new Trie();
var invertedIndex = new InvertedIndex(trie);
var forwardIndex = new ForwardIndex(trie);

var text = "ve ile  , test gelecek yaptığında bilgisayar?., test geleceğinde yaptıklarında masa yaz yanar yazı yaza yazar test test test";
var tokens = tokenizer.Tokenize(text);
Console.WriteLine("Tokens:");

foreach (var token in tokens)
{
    Console.WriteLine(token);
}

invertedIndex.Add(1, tokens.ToArray());
forwardIndex.Add(1, tokens.ToArray());

text = "ve ile gelecek yapmak yap yazı yazar test ev klavye masa";
tokens = tokenizer.Tokenize(text);
invertedIndex.Add(2, tokens.ToArray());
forwardIndex.Add(2, tokens.ToArray());

Console.WriteLine("Inverted Index:");
foreach (var (word, postings) in invertedIndex.Index)
{
    Console.WriteLine($"{word}: {string.Join(", ", postings.Select(p => $"({p.Item1}, {p.Item2})"))}");
}

// Test InvertedIndex
Console.WriteLine("doc count: {0}", invertedIndex.DocumentCount());
Console.WriteLine("doc1 words count: {0}, doc2 words count {1}", invertedIndex.DocumentWordsCount(1), invertedIndex.DocumentWordsCount(2));
Console.WriteLine("word gelecek documents: {0}", string.Join(", ", invertedIndex.WordDocuments("gelecek")?.Select(p => $"({p.Item1}, {p.Item2})") ?? Array.Empty<string>()));
Console.WriteLine("document 0 length: {0}", invertedIndex.DocumentLength(1));
Console.WriteLine("document 1 length: {0}", invertedIndex.DocumentLength(2));
Console.WriteLine("document ids: {0}", string.Join(", ", invertedIndex.DocumentIds()));
Console.WriteLine("document 0 tokens: {0}", string.Join(", ", invertedIndex.Tokens(1)));
Console.WriteLine("document 1 tokens: {0}", string.Join(", ", invertedIndex.Tokens(2)));

Console.WriteLine("Forward Index:");
foreach (var (docId, words) in forwardIndex.Index)
{
    Console.WriteLine($"{docId}: {string.Join(", ", words.Select(p => $"({p.Item1}, {p.Item2})"))}");
}

Console.WriteLine("doc count: {0}", forwardIndex.DocumentCount());
Console.WriteLine("doc1 words count: {0}, doc2 words count {1}", forwardIndex.DocumentWordsCount(1), forwardIndex.DocumentWordsCount(2));
Console.WriteLine("document 0 length: {0}", forwardIndex.DocumentLength(1));
Console.WriteLine("document 1 length: {0}", forwardIndex.DocumentLength(2));
Console.WriteLine("document ids: {0}", string.Join(", ", forwardIndex.DocumentIds()));
Console.WriteLine("document 0 tokens: {0}", string.Join(", ", forwardIndex.Tokens(1)));
Console.WriteLine("document 1 tokens: {0}", string.Join(", ", forwardIndex.Tokens(2)));

var tfidf = new TFIDF(invertedIndex);
var bm25 = new BM25(invertedIndex);

var freqs = tfidf.Calculate(new []{
    "test", "bilgisayar", "yaz"
});

Console.WriteLine("freqs:");

foreach (var freq in freqs)
{
    Console.Write("{0} => {1}, ", freq.Key, freq.Value);
}

freqs = bm25.Calculate(new []{
    "test", "bilgisayar", "yaz"
});

Console.WriteLine("freqs:");

foreach (var freq in freqs)
{
    Console.Write("{0} => {1}, ", freq.Key, freq.Value);
}

Console.WriteLine("trie autocompleteion for ya:");
var trieResults = trie.GetWords("ya");
foreach (var result in trieResults)
{
    Console.WriteLine(result);
}

Console.WriteLine("{0}, {1}", trie.SearchWord("ma"), trie.SearchWord("masa"));

Console.WriteLine("wildcard search");
var wildcardResults = trie.WildcardSearch("yaz*");
foreach (var result in wildcardResults)
{
    Console.WriteLine(result);
}
Console.WriteLine("wildcard search");
var wildcardResults2 = trie.WildcardSearch("*a*ar");
foreach (var result in wildcardResults2)
{
    Console.WriteLine(result);
}
Console.WriteLine("wildcard search");
var wildcardResults3 = trie.WildcardSearch("yazar");
foreach (var result in wildcardResults3)
{
    Console.WriteLine(result);
}

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();

