using datastructures_project.Database;
using datastructures_project.Database.Repository;
using datastructures_project.Document;
using datastructures_project.Handler;
using datastructures_project.Search.Index;
using datastructures_project.Search.Score;
using datastructures_project.Search.Tokenizer;
using datastructures_project.Search.Trie;
using datastructures_project.Template;

using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;

var tokenizer = new Tokenizer();
var trie = new Trie();
var invertedIndex = new InvertedIndex(trie);
//var forwardIndex = new ForwardIndex(trie);
var bm25 = new BM25(invertedIndex); 

// Initialize the database context
var databaseCtx = new ApplicationDbContext();
databaseCtx.Database.EnsureCreated();

// Initialize the repositories
var documentRepository = new DocumentRepository(databaseCtx);

// Initialize the services
var documentService = new DocumentService(invertedIndex, tokenizer, documentRepository);

// Add old documents to the index
var documents = documentRepository.GetAllDocuments();
foreach (var document in documents)
{
    var tokens = tokenizer.Tokenize(document.Title);
    invertedIndex.Add(document.Id, tokens.ToArray());
    //forwardIndex.Add(document.Id, tokens.ToArray());
}

// Add sample docs using service
documentService.AddDocument("test dökümanı", "http://example.com/test", "test.");
// documentService.RemoveDocument(2);

/*
Console.WriteLine(invertedIndex.AverageDoclength);

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

Console.WriteLine("Levenstein distance search");
var levenshteinResults = trie.LevenshteinSearch("ypz", 2);
foreach (var result in levenshteinResults)
{
    Console.WriteLine(result);
}

Console.WriteLine("Levenstein distance search");
levenshteinResults = trie.LevenshteinSearch("yaz", 2);
foreach (var result in levenshteinResults)
{
    Console.WriteLine(result);
}*/

var builder = WebApplication.CreateBuilder(args);

// Register score service for searching
// TODO: Make BM25 and TF-IDF selection optional from config
builder.Services.AddSingleton<ITokenizer>(tokenizer);
builder.Services.AddSingleton<IScore>(bm25);
builder.Services.AddSingleton<IDocumentService>(documentService);

builder.Services.AddSingleton(provider =>
{
    var linkGenerator = provider.GetRequiredService<LinkGenerator>();

    return new ScribanTemplateService(linkGenerator,
        Path.Combine(Directory.GetCurrentDirectory(), "Views"),
        Path.Combine(Directory.GetCurrentDirectory(), "Views", "Layout.html")
    );
});

// Create OpenTelemetry instance
// Add prometheus metrics exporter
var openTelemetryBuilder = builder.Services.AddOpenTelemetry();

openTelemetryBuilder.ConfigureResource(resource => resource
    .AddService(builder.Environment.ApplicationName));

openTelemetryBuilder.WithMetrics(metrics => metrics
    // .AddAspNetCoreInstrumentation()
    .AddMeter("MyApp.Metrics")
    .AddRuntimeInstrumentation()
    .AddPrometheusExporter());

// Add logger 
builder.Host.ConfigureLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
});

// Create app instance
var app = builder.Build();
app.MapPrometheusScrapingEndpoint();
app.UseStaticFiles();

// Register search handlers
SearchHandler.RegisterHandlers(app);

app.Run();

