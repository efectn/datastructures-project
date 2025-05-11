using System.Text.Json;
using datastructures_project.HashTables;

namespace datastructures_project.Handler;

public class HashEntry<TKey>
{
    public int Index { get; set; }
    public TKey Key { get; set; }
    public bool IsTombstone { get; set; }
    public bool IsInCollide { get; set; }
    public bool IsEmpty { get; set; }
}

public class HashTableHandler
{
    public static void RegisterHandlers(WebApplication app)
    {
        app.MapGet("/hashtable/{type}", _testHandler).WithName("hashtable.test");
    }
    
    public static IResult _testHandler(HttpContext ctx, IServiceProvider serviceProvider)
    {
        var forwardIndexes = serviceProvider.GetService<Dictionary<string, IDictionary<int, HashSet<(string, int)>>>>();
        if (forwardIndexes != null)
        {
            return _forwardIndexHandler(ctx, forwardIndexes);
        }
        
        var invertedIndexes = serviceProvider.GetService<Dictionary<string, IDictionary<string, HashSet<(int, int)>>>>();
        if (invertedIndexes != null)
        {
            return _invertedIndexHandler(ctx, invertedIndexes);
        }
        
        return Results.NotFound("Hashtables not found.");
    }

    public static IResult _invertedIndexHandler(HttpContext ctx, Dictionary<string, IDictionary<string, HashSet<(int, int)>>> indexes)
    {
        var type = ctx.Request.RouteValues["type"]?.ToString();
        string[] notWantedTypes = ["Dictionary", "SortedList", "SortedDictionary", "SeparateChaining", "AVL", "BTree", "RedBlack"];
        if (type == "SeparateChaining" && indexes.ContainsKey(type))
        {
            return _invertedIndexSeparateChainingHandler(ctx, indexes);
        }
        
        if (type == null || notWantedTypes.Contains(type) || !indexes.ContainsKey(type))
        {
            return Results.NotFound("Invalid type.");
        }
        
        var ht = (ILinearQuadraticDoubleHashing<string, HashSet<(int, int)>>)indexes[type];
        var tombstones = ht.GetTombstones();
        
        var hashTableElements = new List<HashEntry<string>>(); // index, key, isTombstone, isInCollide
    
        var enumerable = ht.GetEnumeratorWithIndex();
        while (enumerable.MoveNext())
        {
            var current = enumerable.Current;
            if (current.Value.Key == null && current.Value.Value == null)
            {
                hashTableElements.Add(new HashEntry<string>
                {
                    Index = current.Key,
                    IsEmpty = true
                });
                continue;
            }
            
            var isTombstone = tombstones.Contains(current.Key);
            var isInCollide = ht.IsInCollide(current.Value.Key);
            hashTableElements.Add(new HashEntry<string>
            {
                Index = current.Key,
                Key = current.Value.Key,
                IsTombstone = isTombstone,
                IsInCollide = isInCollide
            });
        }
        
        return Results.Ok(hashTableElements);
    }
    
    public static IResult _forwardIndexHandler(HttpContext ctx, Dictionary<string, IDictionary<int, HashSet<(string, int)>>> indexes)
    {
        var type = ctx.Request.RouteValues["type"]?.ToString();
        string[] notWantedTypes = ["Dictionary", "SortedList", "SortedDictionary", "SeparateChaining", "AVL", "BTree", "RedBlack"];
        if (type == "SeparateChaining" && indexes.ContainsKey(type))
        {
            return _forwardIndexSeparateChainingHandler(ctx, indexes);
        }
        
        if (type == null || notWantedTypes.Contains(type) || !indexes.ContainsKey(type))
        {
            return Results.NotFound("Invalid type.");
        }
        
        var ht = (ILinearQuadraticDoubleHashing<int, HashSet<(string, int)>>)indexes[type];
        var tombstones = ht.GetTombstones();
        
        var hashTableElements = new List<HashEntry<int>>(); // index, key, isTombstone, isInCollide
    
        var enumerable = ht.GetEnumeratorWithIndex();
        while (enumerable.MoveNext())
        {
            var current = enumerable.Current;
            if (current.Value.Key == null && current.Value.Value == null)
            {
                hashTableElements.Add(new HashEntry<int>
                {
                    Index = current.Key,
                    IsEmpty = true
                });
                continue;
            }
            
            var isTombstone = tombstones.Contains(current.Key);
            var isInCollide = ht.IsInCollide(current.Value.Key);
            hashTableElements.Add(new HashEntry<int>
            {
                Index = current.Key,
                Key = current.Value.Key,
                IsTombstone = isTombstone,
                IsInCollide = isInCollide
            });
        }
        
        return Results.Ok(hashTableElements);
    }
    
    public static IResult _invertedIndexSeparateChainingHandler(HttpContext ctx, Dictionary<string, IDictionary<string, HashSet<(int, int)>>>
        indexes)
    {
        var type = ctx.Request.RouteValues["type"]?.ToString();
        if (type == null || !indexes.ContainsKey(type))
        {
            return Results.NotFound("Invalid type.");
        }
        
        var ht = (ISeparateChaining<string, HashSet<(int, int)>>)indexes[type];
        var buckets = ht.GetBuckets();
        
        var bucketsHttp = new List<HashEntry<string>>[buckets.Length];
    
        for (int i = 0; i < buckets.Length; i++)
        {
            var bucket = buckets[i];
            if (bucket == null)
            {
                continue;
            }
            
            foreach (var entry in bucket)
            {
                if (bucketsHttp[i] == null)
                {
                    bucketsHttp[i] = new List<HashEntry<string>>();
                }
                
                bucketsHttp[i].Add(new HashEntry<string>
                {
                    Index = i,
                    Key = entry.Key,
                    IsTombstone = false,
                    IsInCollide = false
                });
            }
        }
        
        return Results.Ok(bucketsHttp);
    }
    
    public static IResult _forwardIndexSeparateChainingHandler(HttpContext ctx, Dictionary<string, IDictionary<int, HashSet<(string, int)>>>
        indexes)
    {
        var type = ctx.Request.RouteValues["type"]?.ToString();
        if (type == null || !indexes.ContainsKey(type))
        {
            return Results.NotFound("Invalid type.");
        }
        
        var ht = (ISeparateChaining<int, HashSet<(string, int)>>)indexes[type];
        var buckets = ht.GetBuckets();
        
        var bucketsHttp = new List<HashEntry<int>>[buckets.Length];
    
        for (int i = 0; i < buckets.Length; i++)
        {
            var bucket = buckets[i];
            if (bucket == null)
            {
                continue;
            }
            
            foreach (var entry in bucket)
            {
                if (bucketsHttp[i] == null)
                {
                    bucketsHttp[i] = new List<HashEntry<int>>();
                }
                
                bucketsHttp[i].Add(new HashEntry<int>
                {
                    Index = i,
                    Key = entry.Key,
                    IsTombstone = false,
                    IsInCollide = false
                });
            }
        }
        
        return Results.Ok(bucketsHttp);
    }
}