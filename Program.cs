using datastructures_project.Document;
using datastructures_project.Handler;
using datastructures_project.Search.Index;
using datastructures_project.Search.Score;
using datastructures_project.Search.Tokenizer;
using datastructures_project.Search.Trie;
using datastructures_project.Template;

var documents = new Dictionary<int, Document>
{
    { 0, new Document("Sağlıklı Beslenme Rehberi", "https://example.com/saglikli-beslenme", 
        "Dengeli ve sağlıklı beslenme için temel ipuçları.") },

    { 1, new Document("Dünya Dünya Tarihindeki Önemli Olaylar", "https://example.com/tarih-onemli-olaylar", 
        "Tarihte dönüm noktası olmuş olaylara genel bir bakış.") },

    { 2, new Document("Etkili Dünya Zaman Yönetimi yönet", "https://example.com/zaman-yonetimi", 
        "Günlük hayatınızı daha verimli hale getirmek için zaman yönetimi stratejileri.") },

    { 3, new Document("Kişisel Finans Yönetimi", "https://example.com/finans-yonetimi", 
        "Bütçenizi nasıl yöneteceğinize dair pratik ipuçları.") },

    { 4, new Document("Dünyanın En Güzel Seyahat Rotaları", "https://example.com/en-guzel-seyahat-rotalari", 
        "Dünyada mutlaka görülmesi gereken yerler.") },

    { 5, new Document("Mutluluk ve Pozitif Düşünce", "https://example.com/mutluluk-pozitif-dusunce", 
        "Mutlu ve pozitif bir yaşam için bilimsel öneriler.") },

    { 6, new Document("Verimli Uyku İçin İpuçları", "https://example.com/verimli-uyku", 
        "Kaliteli uyku için uygulanabilecek pratik yöntemler.") },

    { 7, new Document("Doğa ve Çevreyi Koruma", "https://example.com/cevreyi-koruma", 
        "Çevre bilinci oluşturmak ve doğayı korumak için öneriler.") },

    { 8, new Document("Evde Bitki Yetiştirme Rehberi", "https://example.com/bitki-bakimi", 
        "Evde bitki bakımı ve yetiştirme konusunda temel bilgiler.") },

    { 9, new Document("Stresi Azaltma Teknikleri", "https://example.com/stres-azaltma", 
        "Günlük hayatın stresini azaltmak için bilimsel yöntemler.") }
};

var tokenizer = new Tokenizer();
var trie = new Trie();
var invertedIndex = new InvertedIndex(trie);
//var forwardIndex = new ForwardIndex(trie);
var documentService = new DocumentService(invertedIndex, tokenizer);

foreach (var (id, document) in documents)
{
    documentService.AddDocument(id, document);
}

// TODO: Initialize BM25 before documentservice and update doclength everytime new doc added, not only during initialization
var bm25 = new BM25(invertedIndex); 

/*
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

// Create app instance
var app = builder.Build();
app.UseStaticFiles();

// Register search handlers
SearchHandler.RegisterHandlers(app);

app.Run();

