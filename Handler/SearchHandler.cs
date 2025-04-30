using datastructures_project.Document;
using datastructures_project.Dto;
using datastructures_project.Search.Score;
using datastructures_project.Search.Tokenizer;

namespace datastructures_project.Handler;

public class SearchHandler
{
    public static void RegisterHandlers(WebApplication app)
    {
        app.MapGet("/api/v1/search", _apiHandler);
    }

    public static IResult _apiHandler(HttpContext ctx, ITokenizer tokenizer, IScore score, IDocumentService documentService)
    {
        var query = ctx.Request.Query["q"];
        if (string.IsNullOrEmpty(query))
        {
            return Results.BadRequest("q property is required!");
        }

        // Tokenize the query
        var tokens = tokenizer.Tokenize(query);
        if (tokens.Count == 0)
        {
            return Results.BadRequest("q property is empty!");
        }

        // Apply levenshtein distance and wildcard search support
        var newTokens = score.Trie.GetTokens(tokens);
        
        // Calculate the score
        var termFreqs = score.Calculate(newTokens.ToArray());
        if (termFreqs.Count == 0)
        {
            return Results.NotFound("No results found!");
        }
        
        // Sort the results by score and create the response dto
        var sortedResults = termFreqs.OrderByDescending(x => x.Value).ToList();
        var response = new List<SearchResponseDto>();
        
        foreach (var (docId, scoreValue) in sortedResults)
        {
            var document = documentService.GetDocument(docId);
            response.Add(new SearchResponseDto
            {
                Title = document.Title,
                Description = document.Description,
                Url = document.Url,
                Score = scoreValue
            });
        }
        
        // Return the response
        return Results.Ok(response);
    }
}