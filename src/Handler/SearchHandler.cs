using System.Diagnostics.Metrics;
using System.Text.RegularExpressions;
using datastructures_project.Document;
using datastructures_project.Dto;
using datastructures_project.HashTables;
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

    public static IResult _apiHandler(HttpContext ctx, ITokenizer tokenizer, IScore[] score, IDocumentService documentService, ILogger<SearchHandler> logger)
    {
        // Get the query parameters
        var query = ctx.Request.Query["q"];
        if (string.IsNullOrEmpty(query))
        {
            return Results.BadRequest("q property is required!");
        }
        
        // Escape HTML characters
        query = Regex.Replace(query, "<.*?>", string.Empty);
        
        // Log the request
        logger.LogInformation("Search request received for \"{query}\" query on {timestamp} at {path}",
            query, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), ctx.Request.Path);

        // Perform the search
        var processedTokens = new List<string>();
        var searchResults = _searchHandler(score[0], tokenizer, documentService, query, out processedTokens);

        if (searchResults == null)
        {
            return Results.NotFound("No results found!");
        }
        
        // Return the response
        return Results.Ok(searchResults);
    }

    public static IResult _autocompleteHandler(HttpContext ctx, IScore[] scores, ILogger<SearchHandler> logger) {
        // Increment the autocomplete counter
        autocompleteCounter.Add(1);
        
        // Get the query parameters
        var query = ctx.Request.Form["query"].ToString();
        if (string.IsNullOrEmpty(query))
        {
            return Results.Json(new {});
        }
        
        // Escape HTML characters
        query = Regex.Replace(query, "<.*?>", string.Empty);
        
        // Log the request
        logger.LogInformation("Autocomplete request received for \"{query}\" query on {timestamp} at {path}",
            query, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), ctx.Request.Path);
        
        return Results.Json(scores[0].Trie.GetWords(query));
    }

    public static IResult _indexHandler(HttpContext ctx, ScribanTemplateService scribanService)
    {
        // Render the index page
        return Results.Content(scribanService.RenderWithLayout("Index", new Dictionary<string, object>
        {
            {"Title", "Anasayfa"},
        }), "text/html");
    }

    public static IResult _searchHtmlHandler(HttpContext ctx, ITokenizer tokenizer, IScore[] scores,
        IDocumentService documentService, ScribanTemplateService scribanService, ILogger<SearchHandler> logger, IConfiguration config)
    {
        // Get the query parameters
        var query = ctx.Request.Query["q"];
        if (string.IsNullOrEmpty(query))
        {
            return Results.BadRequest("q property is required!"); // TODO: error message instead
        }
        
        // Escape HTML characters
        query = Regex.Replace(query, "<.*?>", string.Empty);
        
        var methods = ctx.Request.Query["methods[]"];

        // Log the request
        logger.LogInformation("Search request received for \"{query}\" query on {timestamp} at {path}",
            query, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), ctx.Request.Path);
        
        // Perform the search
        var results = new SortedList<string, List<SearchResponseDto>>();
        var elapsedTimes = new SortedList<string, double>
        {
            {"Dictionary", 0.0 },
            {"SortedList", 0.0},
            {"SortedDictionary", 0.0},
            {"DoubleHashing", 0.0},
            {"LinearProbing", 0.0},
            {"QuadraticProbing", 0.0},
            {"SeparateChaining", 0.0}
        };
        
        var processedTokens = new List<string>();
        foreach (var score in scores)
        {
            if (methods.Count > 0 && !methods.Contains(score.Tag))
            {
                continue;
            }
            
            var timeBefore = DateTime.Now;
            results[score.Tag] = _searchHandler(score, tokenizer, documentService, query, out processedTokens) ?? [];
            elapsedTimes[score.Tag] = (DateTime.Now - timeBefore).TotalMilliseconds;
        }
        
        // Remove 0.0 results from elapsedTimes
        foreach (var key in elapsedTimes.Keys.ToList())
        {
            if (elapsedTimes[key] == 0.0)
            {
                elapsedTimes.Remove(key);
            }
        }

        var showResultsOf = config["Search:ShowResultsOf"] ?? elapsedTimes.Keys.First();
        if (!results.ContainsKey(showResultsOf))
        {
            showResultsOf = elapsedTimes.Keys.First();
        }
        
        return Results.Content(scribanService.RenderWithLayout("Results", new Dictionary<string, object>
        {
            {"Title", query + " için Sonuçlar"},
            {"Info", $"\"{query}\" için {documentService.TotalDocumentsCount()} sonuç içinden {results[showResultsOf].Count} sonuç bulundu. Arama süresi: {elapsedTimes[showResultsOf]} ms. Sonuçlar {showResultsOf} veri yapısı kullanılarak gösterildi."},
            {"Results", results[showResultsOf]},
            {"ProcessedTokens", string.Join(", ", processedTokens)},
            {"PerfResults", elapsedTimes}
        }), "text/html");
    }


    public static List<SearchResponseDto>? _searchHandler(IScore score, ITokenizer tokenizer, IDocumentService documentService, string query, out List<string>? processedTokens)
    {
        // Increment the search counter
        searchCounter.Add(1);
        
        // Tokenize the query
        var tokens = tokenizer.Tokenize(query);
        if (tokens.Count == 0)
        {
            processedTokens = new List<string>();
            return null;
        }

        // Apply levenshtein distance and wildcard search support
        var newTokens = score.Trie.GetTokens(tokens);
        processedTokens = newTokens;
        
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
