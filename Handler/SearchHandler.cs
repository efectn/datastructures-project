using System.Diagnostics.Metrics;
using datastructures_project.Document;
using datastructures_project.Dto;
using datastructures_project.Search.Score;
using datastructures_project.Search.Tokenizer;
using datastructures_project.Template;

namespace datastructures_project.Handler;

public class SearchHandler
{
    public static Meter meter = new("MyApp.Metrics", "1.0");
    public static Counter<long> searchCounter = meter.CreateCounter<long>("search_counter", "count", "Number of search requests");
    public static Counter<long> autocompleteCounter = meter.CreateCounter<long>("autocomplete_counter", "count", "Number of autocomplete requests");
    
    public static void RegisterHandlers(WebApplication app)
    {
        // Register API routes
        var api = app.MapGroup("/api/v1");
        api.MapGet("/search", _apiHandler).WithName("api.search");
        api.MapPost("/autocomplete", _autocompleteHandler).WithName("api.autocomplete");

        // Register HTML routes
        app.MapGet("/search", _searchHtmlHandler).WithName("search");
        app.MapGet("/", _indexHandler).WithName("index");
    }

    public static IResult _apiHandler(HttpContext ctx, ITokenizer tokenizer, IScore score, IDocumentService documentService, ILogger<SearchHandler> logger)
    {
        // Get the query parameters
        var query = ctx.Request.Query["q"];
        if (string.IsNullOrEmpty(query))
        {
            return Results.BadRequest("q property is required!");
        }
        
        // Log the request
        logger.LogInformation("Search request received for \"{query}\" query on {timestamp} at {path}",
            query, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), ctx.Request.Path);
        
        // Perform the search
        var searchResults = _searchHandler(score, tokenizer, documentService, query);
        if (searchResults == null)
        {
            return Results.NotFound("No results found!");
        }
        
        // Return the response
        return Results.Ok(searchResults);
    }

    public static IResult _autocompleteHandler(HttpContext ctx, IScore score, ILogger<SearchHandler> logger) {
        // Increment the autocomplete counter
        autocompleteCounter.Add(1);
        
        // Get the query parameters
        var query = ctx.Request.Form["query"].ToString();
        if (string.IsNullOrEmpty(query))
        {
            return Results.Json(new {});
        }
        
        // Log the request
        logger.LogInformation("Autocomplete request received for \"{query}\" query on {timestamp} at {path}",
            query, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), ctx.Request.Path);

        return Results.Json(score.Trie.GetWords(query));
    }

    public static IResult _indexHandler(HttpContext ctx, ScribanTemplateService scribanService)
    {
        // Render the index page
        return Results.Content(scribanService.RenderWithLayout("Index", new Dictionary<string, object>
        {
            {"Title", "Anasayfa"},
        }), "text/html");
    }

    public static IResult _searchHtmlHandler(HttpContext ctx, ITokenizer tokenizer, IScore score,
        IDocumentService documentService, ScribanTemplateService scribanService, ILogger<SearchHandler> logger)
    {
        // Get the query parameters
        var query = ctx.Request.Query["q"];
        if (string.IsNullOrEmpty(query))
        {
            return Results.BadRequest("q property is required!"); // TODO: error message instead
        }
        
        // Log the request
        logger.LogInformation("Search request received for \"{query}\" query on {timestamp} at {path}",
            query, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), ctx.Request.Path);
        
        // Perform the search
        var searchResults = _searchHandler(score, tokenizer, documentService, query);
        
        return Results.Content(scribanService.RenderWithLayout("Results", new Dictionary<string, object>
        {
            {"Title", query + " için Sonuçlar"},
            {"Results", searchResults}
        }), "text/html");
    }

    public static List<SearchResponseDto>? _searchHandler(IScore score, ITokenizer tokenizer, IDocumentService documentService, string query)
    {
        // Increment the search counter
        searchCounter.Add(1);
        
        // Tokenize the query
        var tokens = tokenizer.Tokenize(query);
        if (tokens.Count == 0)
        {
            return null;
        }

        // Apply levenshtein distance and wildcard search support
        var newTokens = score.Trie.GetTokens(tokens);
        
        // Calculate the score
        var termFreqs = score.Calculate(newTokens.ToArray());
        if (termFreqs.Count == 0)
        {
            return null;
        }
        
        // Sort the results by score and create the response dto
        var sortedResults = termFreqs.OrderByDescending(x => x.Value).ToList();
        var results = new List<SearchResponseDto>();
        
        foreach (var (docId, scoreValue) in sortedResults)
        {
            var document = documentService.GetDocument(docId);
            results.Add(new SearchResponseDto
            {
                Title = document.Title,
                Description = document.Description,
                Url = document.Url,
                Score = scoreValue
            });
        }

        return results;
    }
}