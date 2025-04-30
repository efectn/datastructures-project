using Scriban.Parsing;

namespace datastructures_project.Template;

using Scriban;
using Scriban.Runtime;
using System.IO;
using Microsoft.AspNetCore.Routing;


public class ScribanTemplateService
{
    private readonly string _viewsPath;
    private readonly string _layoutPath;
    private readonly LinkGenerator _linkGenerator;
    private ScriptObject _scribanObj;

    public ScribanTemplateService(LinkGenerator linkGenerator, string viewsPath, string layoutPath)
    {
        _linkGenerator = linkGenerator;
        _viewsPath = viewsPath;
        _layoutPath = layoutPath;
        
        // Add route path getter function with name
        _scribanObj = new ScriptObject();
        _scribanObj.Import("route", new Func<string, string>(GetRouteUrl));
    }

    public string RenderView(string viewName, Dictionary<string, object>? vars = null)
    {
        string viewPath = Path.Combine(_viewsPath, $"{viewName}.html");
        if (!File.Exists(viewPath))
            throw new FileNotFoundException($"View file '{viewName}.html' not found!");

        string templateContent = File.ReadAllText(viewPath);
        var template = Template.Parse(templateContent);
        
        var obj = _scribanObj.Clone(true);
        obj.Import(vars);
        
        var ctx = new TemplateContext();
        ctx.PushGlobal(obj);
        
        return template.Render(ctx);
    }

    public string RenderWithLayout(string viewName, Dictionary<string, object>? vars = null)
    {
        string content = RenderView(viewName, vars);

        if (!File.Exists(_layoutPath))
            return content;

        string layoutContent = File.ReadAllText(_layoutPath);
        var layoutTemplate = Template.Parse(layoutContent);
        
        var obj = _scribanObj.Clone(true);
        obj.Import(vars);
        obj.Import(new {content});
        
        var ctx = new TemplateContext();
        ctx.PushGlobal(obj);
        
        return layoutTemplate.Render(ctx);
    }

    private string GetRouteUrl(string route)
    {
        return _linkGenerator.GetPathByName(route) ?? "";
    }
}
