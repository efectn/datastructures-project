using datastructures_project.Document;
using datastructures_project.Dto;
using datastructures_project.HashTables;
using datastructures_project.Search.Score;
using datastructures_project.Search.Tokenizer;

namespace datastructures_project.Handler
{
    public class SearchTypeHandler
    {
        public static List<SearchResponseDto>? _searchDoubleHashing(IScore score, ITokenizer tokenizer, IDocumentService documentService, string query)
        {
            // Arama sayacını artır
            SearchHandler.searchCounter.Add(1);

            // Sorguyu tokenlara ayır
            var tokens = tokenizer.Tokenize(query);
            if (tokens.Count == 0)
            {
                return null;
            }

            // Double hashing tablosu: aranan kelimeleri tutar
            var queryTokensTable = new DoubleHashingHashTable<string, bool>();
            foreach (var token in tokens)
            {
                if (!queryTokensTable.ContainsKey(token))
                {
                    queryTokensTable.Add(token, true);
                }
            }

            // Trie ile wildcard ve benzer kelimeleri al
            var expandedTokens = score.Trie.GetTokens(tokens);

            // Her kelime için doküman skorlarını hesapla
            var termFreqs = score.Calculate(expandedTokens.ToArray());
            if (termFreqs.Count == 0)
            {
                return null;
            }

            // Skorlara göre sırala
            var sortedResults = termFreqs.OrderByDescending(x => x.Value).ToList();
            var results = new List<SearchResponseDto>();

            // Dokümanları sırayla kontrol et
            foreach (var (docId, scoreValue) in sortedResults)
            {
                var document = documentService.GetDocument(docId);

                // Doküman başlık ve açıklamasındaki kelimelerden herhangi biri aranan tokenlardan biriyle eşleşiyorsa ekle
                var combinedText = $"{document.Title} {document.Description}";
                var wordsInDoc = tokenizer.Tokenize(combinedText);

                bool containsQueryToken = wordsInDoc.Any(word => queryTokensTable.ContainsKey(word));

                if (containsQueryToken)
                {
                    results.Add(new SearchResponseDto
                    {
                        Title = document.Title,
                        Description = document.Description,
                        Url = document.Url,
                        Score = scoreValue
                    });
                }
            }

            return results;
        }

        public static List<SearchResponseDto>? _searchLineerProbing(IScore score, ITokenizer tokenizer, IDocumentService documentService, string query)
        {
            // Arama sayacını artır
            SearchHandler.searchCounter.Add(1);

            // Sorguyu tokenlara ayır
            var tokens = tokenizer.Tokenize(query);
            if (tokens.Count == 0)
            {
                return null;
            }

            // Double hashing tablosu: aranan kelimeleri tutar
            var queryTokensTable = new LinearProbingHashTable<string, bool>();
            foreach (var token in tokens)
            {
                if (!queryTokensTable.ContainsKey(token))
                {
                    queryTokensTable.Add(token, true);
                }
            }

            // Trie ile wildcard ve benzer kelimeleri al
            var expandedTokens = score.Trie.GetTokens(tokens);

            // Her kelime için doküman skorlarını hesapla
            var termFreqs = score.Calculate(expandedTokens.ToArray());
            if (termFreqs.Count == 0)
            {
                return null;
            }

            // Skorlara göre sırala
            var sortedResults = termFreqs.OrderByDescending(x => x.Value).ToList();
            var results = new List<SearchResponseDto>();

            // Dokümanları sırayla kontrol et
            foreach (var (docId, scoreValue) in sortedResults)
            {
                var document = documentService.GetDocument(docId);

                // Doküman başlık ve açıklamasındaki kelimelerden herhangi biri aranan tokenlardan biriyle eşleşiyorsa ekle
                var combinedText = $"{document.Title} {document.Description}";
                var wordsInDoc = tokenizer.Tokenize(combinedText);

                bool containsQueryToken = wordsInDoc.Any(word => queryTokensTable.ContainsKey(word));

                if (containsQueryToken)
                {
                    results.Add(new SearchResponseDto
                    {
                        Title = document.Title,
                        Description = document.Description,
                        Url = document.Url,
                        Score = scoreValue
                    });
                }
            }

            return results;
        }

        public static List<SearchResponseDto>? _searchQuadraticProbing(IScore score, ITokenizer tokenizer, IDocumentService documentService, string query)
        {
            // Arama sayacını artır
            SearchHandler.searchCounter.Add(1);

            // Sorguyu tokenlara ayır
            var tokens = tokenizer.Tokenize(query);
            if (tokens.Count == 0)
            {
                return null;
            }

            // Double hashing tablosu: aranan kelimeleri tutar
            var queryTokensTable = new QuadraticProbingHashTable<string, bool>();
            foreach (var token in tokens)
            {
                if (!queryTokensTable.ContainsKey(token))
                {
                    queryTokensTable.Add(token, true);
                }
            }

            // Trie ile wildcard ve benzer kelimeleri al
            var expandedTokens = score.Trie.GetTokens(tokens);

            // Her kelime için doküman skorlarını hesapla
            var termFreqs = score.Calculate(expandedTokens.ToArray());
            if (termFreqs.Count == 0)
            {
                return null;
            }

            // Skorlara göre sırala
            var sortedResults = termFreqs.OrderByDescending(x => x.Value).ToList();
            var results = new List<SearchResponseDto>();

            // Dokümanları sırayla kontrol et
            foreach (var (docId, scoreValue) in sortedResults)
            {
                var document = documentService.GetDocument(docId);

                // Doküman başlık ve açıklamasındaki kelimelerden herhangi biri aranan tokenlardan biriyle eşleşiyorsa ekle
                var combinedText = $"{document.Title} {document.Description}";
                var wordsInDoc = tokenizer.Tokenize(combinedText);

                bool containsQueryToken = wordsInDoc.Any(word => queryTokensTable.ContainsKey(word));

                if (containsQueryToken)
                {
                    results.Add(new SearchResponseDto
                    {
                        Title = document.Title,
                        Description = document.Description,
                        Url = document.Url,
                        Score = scoreValue
                    });
                }
            }

            return results;
        }

        public static List<SearchResponseDto>? _searchSeperateChaining(IScore score, ITokenizer tokenizer, IDocumentService documentService, string query)
        {
            // Arama sayacını artır
            SearchHandler.searchCounter.Add(1);

            // Sorguyu tokenlara ayır
            var tokens = tokenizer.Tokenize(query);
            if (tokens.Count == 0)
            {
                return null;
            }

            // Aranacak Hast Table Türü burada Belirlenir, yeni hash türlerini değiştir
            var queryTokensTable = new SeparateChainingHashTable<string, bool>();
            foreach (var token in tokens)
            {
                if (!queryTokensTable.ContainsKey(token))
                {
                    queryTokensTable.Add(token, true);
                }
            }

            // Trie ile wildcard ve benzer kelimeleri al
            var expandedTokens = score.Trie.GetTokens(tokens);

            // Her kelime için doküman skorlarını hesapla
            var termFreqs = score.Calculate(expandedTokens.ToArray());
            if (termFreqs.Count == 0)
            {
                return null;
            }

            // Skorlara göre sırala
            var sortedResults = termFreqs.OrderByDescending(x => x.Value).ToList();
            var results = new List<SearchResponseDto>();

            // Dokümanları sırayla kontrol et
            foreach (var (docId, scoreValue) in sortedResults)
            {
                var document = documentService.GetDocument(docId);

                // Doküman başlık ve açıklamasındaki kelimelerden herhangi biri aranan tokenlardan biriyle eşleşiyorsa ekle
                var combinedText = $"{document.Title} {document.Description}";
                var wordsInDoc = tokenizer.Tokenize(combinedText);

                bool containsQueryToken = wordsInDoc.Any(word => queryTokensTable.ContainsKey(word));

                if (containsQueryToken)
                {
                    results.Add(new SearchResponseDto
                    {
                        Title = document.Title,
                        Description = document.Description,
                        Url = document.Url,
                        Score = scoreValue
                    });
                }
            }

            return results;
        }

        public static List<SearchResponseDto>? _searchLevenshtein(IScore score, ITokenizer tokenizer, IDocumentService documentService, string query)
        {
            // Increment the search counter
            SearchHandler.searchCounter.Add(1);

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
}
