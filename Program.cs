using datastructures_project.Search.Index;
using datastructures_project.Search.Tokenizer;

var tokenizer = new Tokenizer();
var invertedIndex = new InvertedIndex();

var text = "ve ile  , test gelecek yaptığında bilgisayar?., test geleceğinde yaptıklarında masa yaz yazı yaza yazar";
var tokens = tokenizer.Tokenize(text);
Console.WriteLine("Tokens:");

foreach (var token in tokens)
{
    Console.WriteLine(token);
}

invertedIndex.Add(1, tokens.ToArray());

text = "ve ile gelecek yapmak yap yazı yazar test ev klavye masa";
tokens = tokenizer.Tokenize(text);
invertedIndex.Add(2, tokens.ToArray());

Console.WriteLine("Inverted Index:");
foreach (var (word, postings) in invertedIndex.Index)
{
    Console.WriteLine($"{word}: {string.Join(", ", postings.Select(p => $"({p.Item1}, {p.Item2})"))}");
}

Console.WriteLine("doc count: {0}", invertedIndex.DocumentCount());
Console.WriteLine("doc1 words count: {0}, doc2 words count {1}", invertedIndex.DocumentWordsCount(1), invertedIndex.DocumentWordsCount(2));
Console.WriteLine("word gelecek documents: {0}", string.Join(", ", invertedIndex.WordDocuments("gelecek")?.Select(p => $"({p.Item1}, {p.Item2})") ?? Array.Empty<string>()));
Console.WriteLine("document 0 length: {0}", invertedIndex.DocumentLength(1));
Console.WriteLine("document 1 length: {0}", invertedIndex.DocumentLength(2));
Console.WriteLine("document ids: {0}", string.Join(", ", invertedIndex.DocumentIds()));
Console.WriteLine("document 0 tokens: {0}", string.Join(", ", invertedIndex.Tokens(1)));
Console.WriteLine("document 1 tokens: {0}", string.Join(", ", invertedIndex.Tokens(2)));

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();

