using System.Diagnostics.Metrics;
using System.Text.RegularExpressions;
using datastructures_project.Document;
using datastructures_project.Template;

namespace datastructures_project.Handler;

public class DocumentHandler
{
    public static Meter meter = new("MyApp.Metrics", "1.0");
    public static Counter<long> documentCreatedCounter = meter.CreateCounter<long>("document_created", "count", "Number of created documents");
    public static Counter<long> documentDeletedCounter = meter.CreateCounter<long>("document_deleted", "count", "Number of deleted documents");

    public static void RegisterHandlers(WebApplication app)
    {
        // Register HTML routes
        app.MapGet("/documents", _documentsIndexHandler).WithName("documents.index");
        app.MapPost("/documents/create", _documentsCreateHandler).WithName("documents.create");
        app.MapGet("/documents/delete/{id}", _documentsDeleteHandler).WithName("documents.delete");
    }
    
    public static IResult _documentsIndexHandler(HttpContext ctx, IDocumentService documentService, ScribanTemplateService scribanService, ILogger<DocumentHandler> logger)
    {
        
        // Log the request
        logger.LogInformation("Documents request received on {timestamp}", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
        
        // Perform the search
        var documents = documentService.GetAllDocuments();

        var flashMsg = "";
        var message = ctx.Request.Cookies["FlashMessage"];
        if (!string.IsNullOrEmpty(message))
        {
            flashMsg = message;
            ctx.Response.Cookies.Delete("FlashMessage");
        }
        
        // Return the response
        return Results.Content(scribanService.RenderWithLayout("Documents", new Dictionary<string, object>
        {
            {"Title", "Dökümanlar"},
            {"Documents", documents},
            {"FlashMessage", flashMsg},
        }), "text/html");
    }
    
    public static IResult _documentsDeleteHandler(HttpContext ctx, IDocumentService documentService, ScribanTemplateService scribanService, ILogger<DocumentHandler> logger)
    {
        // Get the id from the route
        var id = ctx.Request.RouteValues["id"];
        if (id == null)
        {
            return _redirectDocumentsRoute(ctx, "Döküman ID'sini girmek zorunlu!");
        }
        
        // Log the request
        logger.LogInformation("Documents delete request received for \"{id}\" query on {timestamp}", id, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
        
        // Delete the document
        try
        {
            documentService.RemoveDocument(Convert.ToInt32(id));
        } 
        catch
        {
            _redirectDocumentsRoute(ctx, "Döküman silinirken bir hata oluştu!");
        }
        documentDeletedCounter.Add(1);

        // Return the response
        return _redirectDocumentsRoute(ctx, "Döküman başarıyla silindi!");
    }
    
    public static IResult _documentsCreateHandler(HttpContext ctx, IDocumentService documentService, ScribanTemplateService scribanService, ILogger<DocumentHandler> logger)
    {
        // Get the form data
        var title = ctx.Request.Form["title"];
        var url = ctx.Request.Form["url"];
        var description = ctx.Request.Form["description"];
        
        // Escape HTML characters
        title = Regex.Replace(title, "<.*?>", string.Empty);
        url = System.Net.WebUtility.HtmlEncode(url);
        description = System.Net.WebUtility.HtmlEncode(description);

        // Log the request
        logger.LogInformation("Documents create request received for \"{title}\" query on {timestamp}", title, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
        
        // Validate the form data
        if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(url) || string.IsNullOrEmpty(description))
        {
            return _redirectDocumentsRoute(ctx, "Döküman başlığı, URL'si ve açıklaması girmek zorunlu!");
        }
        
        // Create the document
        try
        {
            documentService.AddDocument(title, url, description);
        } 
        catch
        {
            return _redirectDocumentsRoute(ctx, "Döküman eklenirken bir hata oluştu!");
        }
        documentCreatedCounter.Add(1);

        // Return the response
        return _redirectDocumentsRoute(ctx, "Döküman başarıyla eklendi!");
    }

    private static IResult _redirectDocumentsRoute(HttpContext ctx,string message)
    {
        ctx.Response.Cookies.Append("FlashMessage", message, new CookieOptions
        {
            Path = "/",
            Expires = DateTimeOffset.UtcNow.AddSeconds(15),
        });
        
        return Results.RedirectToRoute(routeName:"documents.index");
    }
}