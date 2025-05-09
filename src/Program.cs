using datastructures_project.Database;
using datastructures_project.Database.Repository;
using datastructures_project.Document;
using datastructures_project.Handler;
using datastructures_project.HashTables;
using datastructures_project.Search.Index;
using datastructures_project.Search.Score;
using datastructures_project.Search.Tokenizer;
using datastructures_project.Search.Trie;
using datastructures_project.Template;

using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;

var builder = WebApplication.CreateBuilder(args);

var tokenizer = new Tokenizer(builder.Configuration);
var trie = new Trie();

// Initialize the index
IIndex[] indexes = new IIndex[8];
switch (builder.Configuration["Search:Index"])
{
    case "inverted":
        var invertedIndexDictionaries = new Dictionary<string, IDictionary<string, HashSet<(int, int)>>>
        {
            { "Dictionary", new Dictionary<string, HashSet<(int, int)>>(300) },
            { "SortedList", new SortedList<string, HashSet<(int, int)>>(300) },
            { "SortedDictionary", new SortedDictionary<string, HashSet<(int, int)>>() },
            { "DoubleHashing", new DoubleHashingHashTable<string, HashSet<(int, int)>>(300) },
            { "LinearProbing", new LinearProbingHashTable<string, HashSet<(int, int)>>(300000) },
            { "QuadraticProbing", new QuadraticProbingHashTable<string, HashSet<(int, int)>>(300000) },
            { "SeparateChaining", new SeparateChainingHashTable<string, HashSet<(int, int)>>(300) },
            { "AVL", new AVLTreeDictionary<string, HashSet<(int, int)>>() }
        };

        var i = 0;
        foreach (var dict in invertedIndexDictionaries)
        {
            indexes[i] = new InvertedIndex(trie, dict.Value, dict.Key);
            i++;
        }
        
        break;
    case "forward":
        var forwardIndexDictionaries = new Dictionary<string, IDictionary<int, HashSet<(string, int)>>>
        {
            { "Dictionary", new Dictionary<int, HashSet<(string, int)>>(300) },
            { "SortedList", new SortedList<int, HashSet<(string, int)>>(300) },
            { "SortedDictionary", new SortedDictionary<int, HashSet<(string, int)>>() },
            { "DoubleHashing", new DoubleHashingHashTable<int, HashSet<(string, int)>>(300) },
            { "LinearProbing", new LinearProbingHashTable<int, HashSet<(string, int)>>(300000) },
            { "QuadraticProbing", new QuadraticProbingHashTable<int, HashSet<(string, int)>>(300000) },
            { "SeparateChaining", new SeparateChainingHashTable<int, HashSet<(string, int)>>(300) },
            { "AVL", new AVLTreeDictionary<int, HashSet<(string, int)>>() }
        };
        
        i = 0;
        foreach (var dict in forwardIndexDictionaries)
        {
            indexes[i] = new ForwardIndex(trie, dict.Value, dict.Key);
            i++;
        }
        
        break;
    default:
        Console.WriteLine("Search:Index configuration is wrong. Avaialable options: inverted, forward");
        Environment.Exit(1);
        break;
}
// Initialize the score
IScore[] score = new IScore[8];
switch (builder.Configuration["Search:Score"])
{
    case "bm25":
        for (var j = 0; j < indexes.Length; j++)
        {
            score[j] = new BM25(indexes[j], indexes[j].Tag);
        }
        break;
    case "tfidf":
        for (var j = 0; j < indexes.Length; j++)
        {
            score[j] = new TFIDF(indexes[j], indexes[j].Tag);
        }
        break;
    default:
        Console.WriteLine("Search:Score configuration is wrong. Avaialable options: bm25, tfidf");
        Environment.Exit(1);
        break;
    
}

// Initialize the database context
var databaseCtx = new ApplicationDbContext();
databaseCtx.Database.EnsureCreated();

// Initialize the repositories
var documentRepository = new DocumentRepository(databaseCtx);

// Initialize the services
var documentService = new DocumentService(indexes, tokenizer, documentRepository);


// Register score service for searching
builder.Services.AddSingleton<ITokenizer>(tokenizer);
builder.Services.AddSingleton<IScore[]>(score);
builder.Services.AddSingleton<IDocumentService>(documentService);

// Register Scriban tempalte engine service
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

// Register document handlers
DocumentHandler.RegisterHandlers(app);

app.Run();

