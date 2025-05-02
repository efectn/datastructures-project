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

var builder = WebApplication.CreateBuilder(args);

var tokenizer = new Tokenizer(builder.Configuration);
var trie = new Trie();

// Initialize the index
IIndex index = null;
switch (builder.Configuration["Search:Index"])
{
    case "inverted":
        index = new InvertedIndex(trie);
        break;
    case "forward":
        index = new ForwardIndex(trie);
        break;
    default:
        Console.WriteLine("Search:Index configuration is wrong. Avaialable options: inverted, forward");
        Environment.Exit(1);
        break;
}
// Initialize the score
IScore score = null;
switch (builder.Configuration["Search:Score"])
{
    case "bm25":
        score = new BM25(index);
        break;
    case "tfidf":
        score = new TFIDF(index);
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
var documentService = new DocumentService(index, tokenizer, documentRepository);


// Register score service for searching
builder.Services.AddSingleton<ITokenizer>(tokenizer);
builder.Services.AddSingleton<IScore>(score);
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

