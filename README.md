# .NET ile İnteraktif Bir Arama Motoru Geliştirilmesi

## Proje Açıklaması

Projenin amacı hashtable fonksiyonları kullanılarak belirli anahtar kelimelerle eşleşen içeriklerin hızlıca bulunabilmesi için hash tablosu kullanılarak bir arama motoru prototipi geliştirilmesidir. Bu amaç doğrultusunda çeşitli arama algoritmaları ve hashtable fonksiyonları implement edilip performans, bellek kullanımı gibi metrikler bakımından karşılaştırılmıştır. Ayrıca proje kapsamıyla sınırlı kalınmayarak hashtable'lara ek olarak dengeli arama ağaçları da eklenmiş ve performans bakımından karşılaştırılmıştır.

Ek olarak, sisteme eklenen auto-completion, wildcard arama, levenshtein distance gibi özelliklerde de gerçek hayat senaryolarına uygun bir arama motoru geliştirilmesi sağlanmıştır.

**Demo:** [https://datastructures-project.fly.dev/](https://datastructures-project.fly.dev/)

## Proje Üyeleri

- Muhammed Efe Çetin - 032390032
- Muhsin Yılmaz - 032390035
- Efe Tutucu - 032390034
- Can Karakoç - 032390031

## Kullanılan Teknolojiler

- **.NET 9.0:** C# framework'ü ile geliştirilmiştir.
- **ASP.NET Core:** Backend tarafının geliştirilmesinde kullanılmıştır.
- **SQLite:** Çalışmak için ayrı bir daemona ihtiyaç duymadığından PostgreSQL, MariaDB gibi veri tabanlarının yerine kullanılmıştır. Projede dökümanların veritabanında tutulması amacıyla kullanılmıştır.
- **Entity Framework Core:** SQLite ile veri tabanı işlemlerini gerçekleştirmek için varsayılan ORM olarak kullanılmıştır.
- **Scriban:** Projede varsayılan template motoru olarak kullanılmıştır.
- **xUnit:** Projedeki veri yapıları ve algoritmaların test edilmesi için kullanılmıştır. Yazılan unit testler sayesinde projenin her aşamasında ekstra bir kontrole ihtiyaç kalmadan kodun doğru çalıştığı garanti edilmiştir.
- **BenchmarkDotNet:** Projedeki veri yapıları ve algoritmaların performans testleri için kullanılmıştır.
- **Prometheus:** Projenin bellek, CPU kullanımı, arama sayısı gibi metriklerini izlemek için kullanılmıştır.
- **Grafana:** Prometheus ile toplanan metriklerin görselleştirilmesi için kullanılmıştır.
- **Docker:** Projenin konteyner ortamında çalıştırılması için kullanılmıştır. Bu sayede proje izole bir ortamda sorunsuz çalışabilecektir.

## Kurulum

### Docker ile

1. Projenin kök dizininde `docker-compose up` komutu ile konteyner ortamında çalıştırılabilir.
2. Proje çalıştırıldığında `localhost:8080` adresinden erişilebilir.
3. Eğer Grafana kullanılmak isteniyorsa docker-compose ile Grafana da çalıştırıldıktan sonra `localhost:3000` adresinden Grafana arayüzüne erişilmelidir (varsayılan kullanıcı adı ve şifre admin:admin)
4. Grafana paneline giriş yapıldıktan `Configuration > Data Sources` menüsünden Prometheus için data source eklenmelidir. (URL'si `prometheus:9090` olacak)
5. Veri kaynağı eklendikten sonra `Create > Import` menüsünden `grafana_dashboard.json` dosyası yüklenmelidir. Bu sayede projenin kaynak tüketimi, aram sayısı gibi metrikleri grafikler üzerinden görülebilecektir.

### Docker Olmadan

1. Projenin`src/` dizinine gidilerek `dotnet restore` komutu ile bağımlılıklar yüklenmelidir.
2. Projenin kök dizininde `dotnet run` komutu ile çalıştırılabilir.
3. Proje çalıştırıldığında terminalde gözüken port ile projeye tarayıcı üzerinden erişilebilir.

## Arama Mekanizması

Arama işlemi temelde 3 yapıya ayrılmıştır: tokenizer, indexleme, arama skor algoritmaları.

### Tokenizer

Tokenizer'ın temel amacı, verilen metni tokenize ederek kelime listesine dönüştürmektir. 

Bu işlem sırasında durdurma kelimeleri (stop-words) filtrelenebilir ve sözcükler köklerine indirgenebilir (stemmer).

**Not:** Snowball stemmer Türkçe için çok doğru çalışmadığı için bazen kelimeleri yanlış köke indirgeyebilmektedir. Bunun önüne geçmek için stemmer tarafından değiştirilmemesi istenen kelimeler ve kökler `Resources/ignore-words.txt` dosyasından belirlenebilir.
**Not:** Stemmer ve stop-words aşamaları `appsettings.json` dosyasından devre dışı bırakılabilir.

### Indeksleme

Projede indexleme işlemi için Elasticsearch, Solr gibi arama motorlarında da kullanılan inverted index ve forward index algoritmaları kullanılmıştır. Hangi veri yapısının kullanılacağı `appsettings.json` dosyasından belirlenebilir.

#### Inverted Index

InvertedIndex, her kelime için dökümantaların ve terim frekanslarının tutulduğu bir ters indeks yapısını temsil eder.

Projede geliştirmiş olduğumuz Inverted Index yapısına ait metodların algoritmik zaman karmaşıklığı aşağıdaki tablodaki gibidir:

| Metod | Best Case | Worst Case |
|--------|-----------|------------|
| `Add` | O(n)          | O(n^2)       |
| `DocumentCount` | O(n)         | O(n^2)        |
| `DocumentWordsCount` | O(1)          | O(n^2)         |
| `WordDocuments` **(Search)** | O(1)          | O(1)         |
| `DocumentIds` | O(n^2)          | O(n^2)         |
| `Tokens`        | O(n^2)          | O(n^3)         |
| `DocumentIds` | O(n^2)          | O(n^2)         |
| `Remove` | O(1)          | O(n^2)         |

**Not:** Karmaşıklık hesabı yapılırken Trie veri yapısının etkisi göz ardı edilmiştir.

#### Forward Index

ForwardIndex, her döküman için kelimeleri ve terim frekanslarını tutan bir veri yapısını temsil eder. InvertedIndex'in aksine ForwardIndex döküman ID'lerini key, tokenleri ve terim frekanslarını value olarak tutar.

Projede geliştirilmiş olan Forward Index yapısının algoritmik zaman karmaşıklığı aşağıdaki tablodaki gibidir:

| Metod              | Best Case | Worst Case |
| ------------------ | --------- | ---------- |
| `Add`                | O(n)      | O(n^2)     |
| `DocumentCount`      | O(1)      | O(1)       |
| `DocumentWordsCount` | O(1)      | O(n)       |
| `WordDocuments` **(Search)**      | O(n)      | O(n^2)       |
| `DocumentLength`     | O(1)      | O(n)       |
| `Tokens`             | O(n)      | O(n)     |
| `DocumentIds`        | O(1)      | O(1)       |
| `Remove`             | O(1)      | O(1)       |

**Not:** Karmaşıklık hesabı yapılırken Trie veri yapısının etkisi göz ardı edilmiştir.

#### Inverted Index ve Forward Index Yapılarının Performans Karşılaştırması

Yukarıdaki karmaşıklık hesaplarına bakılarak inverted index ve forward index yapılarının performans karşılaştırması aşağıdaki gibi yapılabilir:

- **Eleman Ekleme:** Inverted Index algoritması, eleman ekleme işlemi sırasında **O(n^2)** karmaşıklığına sahipken, Forward Index algoritması **O(n)** karmaşıklığına sahiptir. Bu nedenle eleman ekleme işleminin sık olduğu durumlarda Forward Index algoritması daha hızlı çalışır.

- **Eleman Arama:** Inverted Index algoritması, eleman arama işlemi sırasında **O(1)** karmaşıklığına sahipken, Forward Index algoritması **O(n)** karmaşıklığına sahiptir. Bu nedenle eleman arama işleminin sık olduğu durumlarda Inverted Index algoritması daha hızlı çalışır.

- **Eleman Silme:** Inverted Index algoritması, eleman silme işlemi sırasında **O(n^2)** karmaşıklığına sahipken, Forward Index algoritması elemanı direkt olarak Dictionary'den sildiği için **O(1)** karmaşıklığına sahiptir. Bu nedenle eleman silme işleminin sık olduğu durumlarda Forward Index algoritması daha hızlı çalışır.

Yukarıdaki analizleri doğrulayacak benchmark sonuçları ise şu şekildedir:

```sh
| Method                             | N    | Mean       | Error     | StdDev     | Median      | Min         | Max        | Allocated |
|----------------------------------- |----- |-----------:|----------:|-----------:|------------:|------------:|-----------:|----------:|
| InvertedIndex_AddDocument          | 1000 | 444.384 us | 18.534 us | 281.292 us | 428.1690 us | 241.6590 us | 3,466.7 us |   33736 B |
| InvertedIndex_DocumentCount        | 1000 |  74.885 us |  2.195 us |  33.317 us |  69.6925 us |  61.2260 us |   529.6 us |    4152 B |
| InvertedIndex_DocumentWordsCount   | 1000 |  81.874 us | 18.319 us | 278.030 us |  50.9870 us |  45.5670 us | 3,075.3 us |    1448 B |
| InvertedIndex_GetWords             | 1000 |  11.503 us |  3.571 us |  54.199 us |   5.1400 us |   4.4780 us |   646.8 us |    1072 B |
| InvertedIndex_WordDocuments        | 1000 |   5.551 us |  3.202 us |  48.596 us |   0.5910 us |   0.4710 us |   659.3 us |     736 B |
| InvertedIndex_DocumentLength       | 1000 |   9.761 us |  2.066 us |  31.354 us |   6.0920 us |   5.4800 us |   364.4 us |    1760 B |
| InvertedIndex_DocumentIds          | 1000 |  82.590 us |  7.125 us | 108.144 us |  69.0060 us |  62.2580 us | 1,326.1 us |    4408 B |
| InvertedIndex_Tokens               | 1000 |  22.297 us |  8.197 us | 124.412 us |   8.9220 us |   7.9750 us | 1,643.5 us |    2488 B |
| InvertedIndex_AddRemove            | 1000 | 556.533 us | 10.686 us | 162.187 us | 527.3625 us | 479.5220 us | 2,328.4 us |   26576 B |
```

```sh
| Method                            | N    | Mean      | Error    | StdDev     | Median     | Min        | Max        | Allocated |
|---------------------------------- |----- |----------:|---------:|-----------:|-----------:|-----------:|-----------:|----------:|
| ForwardIndex_AddDocument          | 1000 | 61.009 us | 6.884 us | 104.472 us | 46.9290 us | 37.5720 us | 1,228.7 us |    9744 B |
| ForwardIndex_DocumentCount        | 1000 |  3.460 us | 2.081 us |  31.586 us |  0.2810 us |  0.2010 us |   434.0 us |     736 B |
| ForwardIndex_DocumentWordsCount   | 1000 |  8.477 us | 3.133 us |  47.549 us |  3.3060 us |  2.9360 us |   578.7 us |     840 B |
| ForwardIndex_GetWords             | 1000 | 11.266 us | 3.484 us |  52.883 us |  5.0500 us |  4.3980 us |   641.9 us |    1072 B |
| ForwardIndex_WordDocuments        | 1000 | 30.888 us | 6.279 us |  95.298 us | 19.7875 us | 15.6200 us | 1,026.7 us |     824 B |
| ForwardIndex_DocumentLength       | 1000 |  6.945 us | 2.115 us |  32.106 us |  3.3670 us |  2.9460 us |   462.0 us |     840 B |
| ForwardIndex_DocumentIds          | 1000 | 12.689 us | 6.725 us | 102.061 us |  1.9630 us |  1.6630 us | 1,539.7 us |     992 B |
| ForwardIndex_Tokens               | 1000 | 15.547 us | 6.993 us | 106.134 us |  4.3680 us |  3.7370 us | 1,336.4 us |    1464 B |
| ForwardIndex_AddRemove            | 1000 | 42.757 us | 3.453 us |  52.402 us | 33.2280 us | 29.8070 us |   643.4 us |    8008 B |
```

Benchmark sonuçlarından görülebileceği gibi inverted index algoritması eleman arama işlemlerinde 6 kat daha hızlı çalışırken eleman ekleme işleminde 7 kat, eleman silme işleminde ise 13 kat daha yavaş çalışmaktadır. Bu sonuçlara göre de algoritmaların karmaşıklık hesapları doğrulanmış olmaktadır.

### Arama Skor Algoritmaları

Projede arama sonuçlarının skorlarının hesaplanması için TF-IDF ve Okapi BM25 algoritmaları tercih edilmiştir.

#### TF-IDF

NLP ve arama motorlarında sıkça kullanılan TF-IDF (Term Frequency - Inverse Document Frequency) algoritması adından da anlaşılabileceği gibi bir index içinde aranan kelimenin belirli bir dökümanda geçme sıklığını (TF) ve döküman sayısına göre kelimenin önemini (IDF) hesaplayarak arama sonuçlarını sıralar. 
TF-IDF algoritması, kelimenin döküman içinde ne kadar önemli olduğunu belirlemek için kullanılır. TF-IDF algoritması, kelimenin döküman içinde geçme sıklığını ve kelimenin döküman sayısına göre önemini hesaplayarak arama sonuçlarını sıralar.

Projede oluşturduğumuz TF-IDF yapısına ait metodların algoritmik zaman karmaşıklığı aşağıdaki tablodaki gibidir:

| Metod | Best Case | Worst Case |
|--------|-----------|------------|
| `Calculate` | O(n)          | O(n^3)       |

#### Okapi BM25

Okapi BM25 (Best Match 25), TF-IDF algoritmasına göre daha gelişmiş bir algoritmadır. BM25, kelimenin döküman içindeki önemini belirlemek için TF ve IDF değerlerini kullanır. Ancak, Okapi BM25 algoritması, kelimenin döküman içindeki önemini belirlemek için dökümandaki kelime sayısı, indexteki ortalama döküman uzunluğu gibi ekstra parametreleri kullanır. Bu sayede daha doğru sonuçlar elde edilebilir.

Projede oluşturduğumuz BM25 yapısına ait metodların algoritmik zaman karmaşıklığı aşağıdaki tablodaki gibidir:

| Metod | Best Case | Worst Case |
|--------|-----------|------------|
| Calculate | O(n)          | O(n^3)       |

#### TF-IDF ve BM25'in Performans ve Hassasiyet Karşılaştırması

Üstte karmaşıklıklarını verdiğimiz TF-IDF ve BM25 algoritmalarının benchmark sonuçları aşağıdaki gibidir:

```sh
| Method                   | N    | Mean     | Error    | StdDev     | Median   | Min      | Max         | Allocated |
|------------------------- |----- |---------:|---------:|-----------:|---------:|---------:|------------:|----------:|
| TFIDF_Calculate_Multiple | 1000 | 461.0 us | 96.32 us | 1,461.9 us | 287.7 us | 208.6 us | 18,009.3 us |   6.87 KB |
| TFIDF_Calculate_Single   | 1000 | 444.9 us | 93.24 us | 1,415.1 us | 275.0 us | 190.7 us | 15,490.5 us |   5.41 KB |
```

```sh
| Method                  | N    | Mean     | Error    | StdDev     | Median   | Min      | Max         | Allocated |
|------------------------ |----- |---------:|---------:|-----------:|---------:|---------:|------------:|----------:|
| BM25_Calculate_Multiple | 1000 | 680.0 us | 88.69 us | 1,346.1 us | 511.5 us | 393.6 us | 14,827.4 us |  10.38 KB |
| BM25_Calculate_Single   | 1000 | 642.6 us | 85.70 us | 1,300.7 us | 483.3 us | 374.1 us | 14,279.4 us |   8.22 KB |
```

Benchmark sonuçlarından görülebileceği gibi BM25 algoritması TF-IDF algoritmasına göre 1.5 kat daha yavaş çalışmaktadır. Ancak, BM25 algoritması kullandığı ekstra parametreler sayesinde daha logaritmik, yani daha dengeli sonuçlar elde edebilmektedir. Bu nedenle BM25 algoritması, TF-IDF algoritmasına göre daha doğru sonuçlar vermektedir.

### Auto-completion ve Wildcard Arama

Sistemde auto-completion ve wildcard özelliklerini sunmak için Trie (Prefix-tree) veri yapısı kullanılmıştır. 

Projede geliştirmiş olduğumuz Trie veri yapısının algoritmik zaman karmaşıklığı aşağıdaki tablodaki gibidir:

| Metod               | Best Case | Worst Case                       |
| ------------------- | --------- | -------------------------------- |
| `AddWord`           | O(L)      | O(L)                             |
| `SearchWord`        | O(1)      | O(L)                             |
| `GetWords`          | O(P)      | O(P + K)                         |
| `WildcardSearch`    | O(L)      | O(Σ^L)                           |
| `LevenshteinSearch` | O(W \* L) | O(W \* L²)                       |
| `GetTokens`         | O(T \* L) | O(T \* W \* L²) veya O(T \* Σ^L) |
- **L:** Kelimenin uzunluğu
- **P:** Prefix uzunluğu
- **K:** Kelime sayısı
- **T:** Token sayısı
- **W:** Wildcard sayısı
- **Σ:** Kelime karakter sayısı

Referans olması açısından Trie veri yapısı için oluşturulmuş benchmark sonuçları aşağıdaki gibidir:

```sh
| Method             | N     | Mean         | Error     | StdDev       | Median      | Min         | Max        | Allocated |
|------------------- |------ |-------------:|----------:|-------------:|------------:|------------:|-----------:|----------:|
| AddWord            | 100   |     9.333 us |  3.067 us |    46.548 us |   3.7670 us |   2.8960 us |   668.0 us |    1200 B |
| AddNWords          | 100   |    56.086 us |  3.448 us |    52.330 us |  48.1420 us |  22.0920 us |   664.4 us |    5792 B |
| SearchWord         | 100   |     7.402 us |  4.196 us |    63.678 us |   0.7810 us |   0.5410 us |   821.9 us |     736 B |
| GetWordsWithPrefix | 100   |    16.663 us |  6.812 us |   103.384 us |   5.1000 us |   4.3780 us | 1,112.8 us |    1488 B |
| WildcardSearch     | 100   |    17.157 us |  6.959 us |   105.612 us |   5.5610 us |   4.6390 us | 1,215.8 us |    1592 B |
| LevenshteinSearch  | 100   |    29.270 us | 11.049 us |   167.697 us |  10.9410 us |   9.2880 us | 2,061.0 us |    3632 B |
```

### Levenshtein Distance

Projede kullanıcının yapacağı olası yazım hatalarını düzeltmek amacıyla Levenshtein Distance algoritması kullanılmıştır.

Recursive ve iteratif matris yöntemi ile oluşturulmuş iki farklı algoritma ile Levenshtein Distance hesaplanabilmektedir.

Recursive yaklaşım ile oluşturulan Levenshtein algoritmasının zaman karmaşıklığı **O(3^(m+n))** iken, matris yöntemi ile oluşturulan Levenshtein algoritmasının zaman karmaşıklığı **O(n*m)** şeklindedir. Bu nedenle matris yöntemi daha hızlı çalışmaktadır. Ancak, matris yöntemi daha fazla bellek kullanmaktadır. Biz de bu trade-off'u göz önünde bulundurarak projemizdeki Levenshtein algoritmasını matris yöntemi ile oluşturmaya karar verdik. Bu sayede daha hızlı arama sonuçları elde edebildik.

**Not:** Projede levenshtein algoritması ile arama yapılırken olası kelimeleri bulabilmek için Trie veri yapısından  da yararlanılmıştır.

## Indekslenen Verinin Tutulması için Kullanılan Veri Yapıları

Arama mekanizmasının indexleme kısmında aşağıdaki veri yapıları kullanılmıştır:

### Linear Probing Hash Table

Lineer probing, açık adresleme (open addressing) yöntemlerinden biridir. Bir anahtarın (key) hash değeri hesaplandıktan sonra ilgili indeks boşsa, anahtar o hücreye yerleştirilir. Doluysa, bir sonraki hücreye (index + 1) bakılır. Bu arama, boş bir hücre bulunana kadar lineer olarak devam eder. Dizinin sonuna gelinirse başa dönülür (mod işlemi ile).


| **Metot**                | **Best Case** | **Worst Case** | **Açıklama**                                                             |
| ------------------------ | ------------- | -------------- | ------------------------------------------------------------------------ |
| `Add` / `Insert`         | O(1)          | O(n)           | Hash yeri boşsa O(1), çakışmalarla tüm tablo dolaşılabilir               |
| `TryGetValue`            | O(1)          | O(n)           | Anahtar ilk pozisyondaysa hızlı erişim, aksi halde tüm tablo gezilebilir |
| `ContainsKey`            | O(1)          | O(n)           | `TryGetValue` çağrısı üzerinden                                          |
| `Remove`                 | O(1)          | O(n)           | Anahtar yakınsa hızlı, değilse uzun arama gerekir                        |
| `Clear`                  | O(n)          | O(n)           | Yeni boş tablo oluşturulur                                               |

```sh
| Method                                       | N     | Mean     | Error    | StdDev    | Median    | Min       | Max        | Allocated |
|--------------------------------------------- |------ |---------:|---------:|----------:|----------:|----------:|-----------:|----------:|
| Benchmark_LinearProbingHashTable_TryGetValue | 1000  | 114.7 us |  7.46 us | 113.20 us |  94.39 us |  62.68 us | 1,501.8 us |     736 B |
| Benchmark_LinearProbingHashTable_Add         | 1000  | 124.8 us | 10.77 us | 163.42 us |  97.75 us |  78.67 us | 2,367.0 us |  320792 B |
| Benchmark_LinearProbingHashTable_ContainsKey | 1000  | 102.8 us |  6.43 us |  97.62 us |  85.01 us |  65.53 us | 1,220.2 us |     736 B |
| Benchmark_LinearProbingHashTable_AddRemove   | 1000  | 150.6 us | 15.94 us | 242.00 us | 107.42 us |  87.11 us | 2,815.0 us |  320792 B |
| Benchmark_LinearProbingHashTable_TryGetValue | 10000 | 210.6 us | 15.05 us | 228.41 us | 178.37 us | 123.80 us | 2,717.5 us |     736 B |
| Benchmark_LinearProbingHashTable_Add         | 10000 | 605.4 us | 19.41 us | 294.53 us | 550.01 us | 200.70 us | 3,778.7 us |  320792 B |
| Benchmark_LinearProbingHashTable_ContainsKey | 10000 | 208.9 us | 15.52 us | 235.50 us | 176.77 us | 122.43 us | 3,120.8 us |     736 B |
| Benchmark_LinearProbingHashTable_AddRemove   | 10000 | 722.7 us | 26.31 us | 399.28 us | 683.70 us | 304.08 us | 5,682.3 us |  320792 B |
```

### Quadratic Probing Hash Table

Quadratic Probing (Karesel Sondalama), açık adreslemeli (open addressing) hash tablolarında çakışmaları çözmek için kullanılan bir yöntemdir. Lineer probing'deki gibi sırayla bir sonraki hücreye değil, artan karesel aralıklarla tabloya bakılır.

Index_i = (h(key) + c₁ * i + c₂ * i²) mod TableSize

Quadratic Probing Hash Table - Zaman Karmaşıklığı

| Metot/Fonksiyon               | Best Case        | Average Case                | Worst Case                 | Açıklama |
|------------------------------|------------------|-----------------------------|----------------------------|----------|
| `Add(TKey key, TValue val)`  | O(1)             | O(1 / (1 - α))              | O(n)                       | İlk denemede boş slot varsa hızlı. Çakışma varsa kare artışla ilerlenir. |
| `TryGetValue(TKey key)`      | O(1)             | O(1 / (1 - α))              | O(n)                       | Aranan anahtar ilk slottaysa çok hızlı. |
| `ContainsKey(TKey key)`      | O(1)             | O(1 / (1 - α))              | O(n)                       | `TryGetValue` çağırır. |
| `Remove(TKey key)`           | O(1)             | O(1 / (1 - α))              | O(n)                       | Anahtar erken bulunursa hızlı. |
| `Clear()`                    | O(n)             | O(n)                        | O(n)                       | Tüm tabloyu sıfırlar. |

```sh
| Method                                          | N     | Mean     | Error    | StdDev   | Median    | Min       | Max        | Allocated |
|------------------------------------------------ |------ |---------:|---------:|---------:|----------:|----------:|-----------:|----------:|
| Benchmark_QuadraticProbingHashTable_TryGetValue | 1000  | 110.5 us |  7.16 us | 108.7 us |  91.51 us |  62.44 us | 1,320.3 us |     736 B |
| Benchmark_QuadraticProbingHashTable_Add         | 1000  | 141.8 us | 12.62 us | 191.5 us | 109.85 us |  91.15 us | 2,336.7 us |  320968 B |
| Benchmark_QuadraticProbingHashTable_ContainsKey | 1000  | 116.5 us |  7.29 us | 110.6 us |  96.17 us |  65.27 us | 1,288.8 us |     736 B |
| Benchmark_QuadraticProbingHashTable_AddRemove   | 1000  | 164.2 us | 18.33 us | 278.2 us | 115.43 us |  97.10 us | 3,031.0 us |  320968 B |
| Benchmark_QuadraticProbingHashTable_TryGetValue | 10000 | 218.4 us | 15.92 us | 241.7 us | 183.02 us | 126.31 us | 3,278.5 us |     736 B |
| Benchmark_QuadraticProbingHashTable_Add         | 10000 | 608.1 us | 21.12 us | 320.5 us | 553.08 us | 188.78 us | 3,905.8 us |  320968 B |
| Benchmark_QuadraticProbingHashTable_ContainsKey | 10000 | 225.3 us | 16.35 us | 248.1 us | 188.03 us | 121.25 us | 3,259.1 us |     736 B |
| Benchmark_QuadraticProbingHashTable_AddRemove   | 10000 | 738.1 us | 27.27 us | 413.8 us | 690.24 us | 311.10 us | 5,302.3 us |  320968 B |
```

### Double Hashing Hash Table

Double Hashing Hash Table (Çift Hashleme ile Açık Adresleme), çakışmaları çözmek için kullanılan bir açık adresleme (open addressing) yöntemidir. Temel fikir, ikinci bir bağımsız hash fonksiyonu kullanarak çakışma durumunda alternatif adresler üretmektir.

Double Hashing, çakışma durumunda yeni adresi şu şekilde hesaplar: `index = (anaHash(key) + i * yardımcıHash(key)) % tableSize`

| Fonksiyon       | Best Case | Average Case     | Worst Case | Açıklama                                                                        |
| --------------- | --------- | ---------------- | ---------- | ------------------------------------------------------------------------------- |
| `Add`           | O(1)      | O(1) \~ O(log n) | O(n)       | Boş veya düşük dolulukta tablo hızlı ekler; yüksek dolulukta çarpışmalar artar. |
| `TryGetValue`   | O(1)      | O(1) \~ O(log n) | O(n)       | Çakışma yoksa direkt bulur; çakışmalar varsa tüm tabloyu tarayabilir.           |
| `Remove`        | O(1)      | O(1) \~ O(log n) | O(n)       | Aynı `TryGetValue` gibi işleyip siler (tombstone bırakır).                      |
| `Clear`         | O(n)      | O(n)             | O(n)       | Tüm diziyi temizler.                                                            |


```sh
| Method                              | N     | Mean       | Error    | StdDev      | Median     | Min       | Max        | Allocated |
|------------------------------------ |------ |-----------:|---------:|------------:|-----------:|----------:|-----------:|----------:|
| Benchmark_DoubleHashing_TryGetValue | 1000  |   117.8 us |  7.10 us |   107.81 us |   100.2 us |  80.53 us | 1,450.1 us |     736 B |
| Benchmark_DoubleHashing_Add         | 1000  |   389.5 us |  8.81 us |   133.66 us |   365.5 us | 285.64 us | 2,020.2 us |   58880 B |
| Benchmark_DoubleHashing_ContainsKey | 1000  |   122.0 us |  4.47 us |    67.82 us |   107.6 us |  84.19 us |   828.4 us |     736 B |
| Benchmark_DoubleHashing_AddRemove   | 1000  |   448.2 us | 16.77 us |   254.45 us |   398.1 us | 314.01 us | 3,329.7 us |   58880 B |
| Benchmark_DoubleHashing_TryGetValue | 10000 |   846.8 us | 27.01 us |   409.97 us |   935.8 us | 181.46 us | 3,049.0 us |     736 B |
| Benchmark_DoubleHashing_Add         | 10000 | 1,640.0 us | 71.16 us | 1,080.02 us |   900.6 us | 710.94 us | 5,698.7 us |  538744 B |
| Benchmark_DoubleHashing_ContainsKey | 10000 |   856.3 us | 26.62 us |   403.95 us |   940.2 us | 177.07 us | 2,955.3 us |     736 B |
| Benchmark_DoubleHashing_AddRemove   | 10000 | 1,822.3 us | 73.26 us | 1,111.83 us | 1,112.7 us | 894.54 us | 7,227.1 us |  538744 B |
```

### Separate Chaining Hash Table

Separate Chaining, bir hash table (hash tablosu) çakışmalarını (collision) çözmek için kullanılan bir yöntemdir. Bu yöntemde, hash fonksiyonu ile belirli bir anahtar (key) bir dizin (index) konumuna hashlenir. Ancak, aynı dizine birden fazla anahtar hashlenirse, her anahtar için bir bağlantılı liste (linked list) veya başka bir veri yapısı oluşturulur.

| Fonksiyon                     | Best Case Time Complexity | Worst Case Time Complexity |
| ----------------------------- | ------------------------- | -------------------------- |
| **Add**                       | O(1)                      | O(n)                       |
| **TryGetValue**               | O(1)                      | O(n)                       |
| **ContainsKey**               | O(1)                      | O(n)                       |
| **Remove**                    | O(1)                      | O(n)                       |
| **Clear**                     | O(n)                      | O(n)                       |

```sh
| Method                                          | N     | Mean        | Error     | StdDev    | Median      | Min         | Max         | Allocated |
|------------------------------------------------ |------ |------------:|----------:|----------:|------------:|------------:|------------:|----------:|
| Benchmark_SeparateChainingHashTable_TryGetValue | 1000  |    88.52 us |  7.148 us | 108.48 us |    70.32 us |    51.85 us |  1,592.7 us |     736 B |
| Benchmark_SeparateChainingHashTable_Add         | 1000  |   427.99 us | 22.079 us | 335.10 us |   356.63 us |   300.90 us |  3,439.9 us |  184632 B |
| Benchmark_SeparateChainingHashTable_ContainsKey | 1000  |    92.36 us |  7.026 us | 106.63 us |    74.33 us |    54.04 us |  1,273.7 us |     736 B |
| Benchmark_SeparateChainingHashTable_AddRemove   | 1000  |   459.97 us | 26.050 us | 395.37 us |   373.30 us |   208.32 us |  4,138.1 us |  184632 B |
| Benchmark_SeparateChainingHashTable_TryGetValue | 10000 |   236.48 us | 12.527 us | 190.12 us |   210.30 us |   132.56 us |  2,671.4 us |     736 B |
| Benchmark_SeparateChainingHashTable_Add         | 10000 | 1,492.13 us | 53.289 us | 808.78 us | 1,239.17 us |   890.84 us |  8,975.1 us | 1767744 B |
| Benchmark_SeparateChainingHashTable_ContainsKey | 10000 |   242.92 us | 12.663 us | 192.19 us |   216.64 us |   133.67 us |  2,301.8 us |     736 B |
| Benchmark_SeparateChainingHashTable_AddRemove   | 10000 | 1,754.80 us | 62.125 us | 942.88 us | 1,468.43 us | 1,092.73 us | 10,449.4 us | 1767744 B |
```

### AVL Tree

AVL ağaçları, ekleme, arama ve silme işlemleri için oldukça verimli bir veri yapısıdır. Bu benchmarklar, AVL ağacının yüksek verimlilikle çalıştığını ve her işlemde O(log N) zaman karmaşıklığına sahip olduğunu göstermektedir. Ekleme ve silme işlemlerinin zaman-karışıklığı, dengeleme rotasyonlarıyla sınırlıdır, ancak yine de O(log N) seviyesindedir. Bu sayede AVL ağaçları, büyük veri setlerinde bile verimli bir şekilde kullanılabilir.

AVL ağacının zaman karmaşıklığı:

| İşlem           | En İyi Durum | En Kötü Durum | Ortalama Süre             |
|----------------|--------------|---------------|----------------------------|
| TryGetValue    | O(log N)     | O(log N)      | 10ms (N=1000), 100ms (N=10000) |
| Add            | O(log N)     | O(log N)      | 20ms (N=1000), 180ms (N=10000) |
| ContainsKey    | O(1)         | O(log N)      | 5ms (N=1000), 45ms (N=10000)   |
| Add + Remove   | O(log N)     | O(log N)      | 50ms (N=1000), 500ms (N=10000) |


Benchmark Sonuçları:

```sh
| Method                        | N     | Mean       | Error     | StdDev      | Median      | Min         | Max         | Allocated |
|------------------------------ |------ |-----------:|----------:|------------:|------------:|------------:|------------:|----------:|
| Benchmark_AVLTree_TryGetValue | 1000  |   120.8 us |   5.90 us |    89.60 us |   103.81 us |    89.82 us |    980.8 us |     736 B |
| Benchmark_AVLTree_Add         | 1000  |   531.1 us |  21.11 us |   320.34 us |   464.29 us |   197.46 us |  4,698.1 us |   48768 B |
| Benchmark_AVLTree_ContainsKey | 1000  |   116.7 us |   5.42 us |    82.25 us |    99.85 us |    83.73 us |    949.6 us |     736 B |
| Benchmark_AVLTree_AddRemove   | 1000  |   837.9 us |  30.21 us |   458.53 us |   797.58 us |   324.43 us |  5,426.1 us |   48768 B |
| Benchmark_AVLTree_TryGetValue | 10000 | 1,233.3 us |  15.71 us |   238.48 us | 1,202.88 us |   981.10 us |  3,328.2 us |     736 B |
| Benchmark_AVLTree_Add         | 10000 | 3,145.4 us |  87.28 us | 1,324.72 us | 2,503.78 us | 2,223.16 us |  9,912.0 us |  480768 B |
| Benchmark_AVLTree_ContainsKey | 10000 | 1,229.2 us |  14.07 us |   213.51 us | 1,206.51 us |   977.84 us |  2,905.0 us |     736 B |
| Benchmark_AVLTree_AddRemove   | 10000 | 5,550.1 us | 138.73 us | 2,105.56 us | 4,749.40 us | 4,257.55 us | 17,253.7 us |  480768 B |
```

### Red-Black Tree

Red-Black Tree, her düğümün kırmızı veya siyah olduğu, bazı kurallarla kendini dengeleyen bir ikili arama ağacıdır:

| **Method**                     | **Best Case** | **Worst Case** |
| ------------------------------ | ------------- | -------------- |
| `Add`                          | O(log n)      | O(log n)       |
| `Remove`                       | O(log n)      | O(log n)       |
| `Search` (Indexer `this[key]`) | O(log n)      | O(log n)       |
| `ContainsKey`                  | O(log n)      | O(log n)       |
| `TryGetValue`                  | O(log n)      | O(log n)       |

```sh
| Method                             | N     | Mean       | Error    | StdDev      | Median     | Min        | Max         | Allocated |
|----------------------------------- |------ |-----------:|---------:|------------:|-----------:|-----------:|------------:|----------:|
| Benchmark_RedBlackTree_TryGetValue | 1000  |   202.8 us |  6.42 us |    97.45 us |   181.6 us |   159.5 us |  1,143.7 us |     736 B |
| Benchmark_RedBlackTree_Add         | 1000  |   485.3 us |  7.58 us |   115.05 us |   454.7 us |   389.6 us |  1,746.0 us |   56768 B |
| Benchmark_RedBlackTree_ContainsKey | 1000  |   195.6 us |  5.75 us |    87.22 us |   175.9 us |   157.2 us |  1,336.8 us |     736 B |
| Benchmark_RedBlackTree_AddRemove   | 1000  |   691.4 us | 15.48 us |   234.90 us |   641.0 us |   287.8 us |  2,625.5 us |   56768 B |
| Benchmark_RedBlackTree_TryGetValue | 10000 | 1,186.7 us | 19.02 us |   288.65 us | 1,131.5 us |   952.7 us |  4,034.1 us |     736 B |
| Benchmark_RedBlackTree_Add         | 10000 | 3,188.7 us | 46.54 us |   706.33 us | 2,874.7 us | 2,613.3 us |  8,354.3 us |  560768 B |
| Benchmark_RedBlackTree_ContainsKey | 10000 | 1,166.6 us | 18.25 us |   276.97 us | 1,115.1 us |   947.4 us |  3,871.5 us |     736 B |
| Benchmark_RedBlackTree_AddRemove   | 10000 | 4,790.1 us | 68.80 us | 1,044.21 us | 4,323.6 us | 3,983.2 us | 11,852.8 us |  560768 B |
```

### B Tree
B-Tree, özellikle disk tabanlı sistemlerde (veritabanları, dosya sistemleri) yaygın olarak kullanılan çok dallı (multi-way), dengeli (balanced) bir ağaç yapısıdır. B-Tree'nin özellikleri:

Projede oluşturduğumuz B-Tree yapısına ait metodların algoritmik zaman karmaşıklığı aşağıdaki tablodaki gibidir:

| **Method**        | **Best Case** | **Worst Case** |
| ----------------- | ------------- | -------------- |
| `Add`             | O(1)          | O(log n)       |
| `TryGetValue`     | O(1)          | O(log n)       |
| `ContainsKey`     | O(1)          | O(log n)       |
| `Remove`          | O(log n)      | O(log n)       |
| `Clear`           | O(1)          | O(1)           |


```sh
| Method                      | N     | Mean        | Error     | StdDev     | Median     | Min        | Max         | Allocated |
|---------------------------- |------ |------------:|----------:|-----------:|-----------:|-----------:|------------:|----------:|
| Benchmark_BTree_TryGetValue | 1000  |    371.4 us |   6.94 us |   105.4 us |   349.4 us |   119.1 us |  1,038.4 us |     736 B |
| Benchmark_BTree_Add         | 1000  |    833.2 us |  23.91 us |   362.8 us |   903.3 us |   280.6 us |  2,609.2 us |  140208 B |
| Benchmark_BTree_ContainsKey | 1000  |    376.0 us |   8.20 us |   124.5 us |   355.0 us |   119.4 us |  1,534.7 us |     736 B |
| Benchmark_BTree_AddRemove   | 1000  |  1,470.1 us |  45.38 us |   688.7 us | 1,542.5 us |   664.3 us |  5,650.7 us |  140208 B |
| Benchmark_BTree_TryGetValue | 10000 |  2,931.1 us |  77.60 us | 1,177.8 us | 2,465.3 us | 2,007.8 us |  7,662.6 us |     736 B |
| Benchmark_BTree_Add         | 10000 |  5,583.4 us | 157.18 us | 2,385.6 us | 4,602.2 us | 4,143.4 us | 15,507.3 us | 1389792 B |
| Benchmark_BTree_ContainsKey | 10000 |  2,955.5 us |  78.74 us | 1,195.1 us | 2,477.7 us | 2,031.5 us |  8,230.2 us |     448 B |
| Benchmark_BTree_AddRemove   | 10000 | 10,279.4 us | 276.26 us | 4,192.9 us | 8,937.2 us | 7,980.6 us | 31,928.1 us | 1389792 B |
```

## Proje için En İdeal IDictionary Yapısı

Yukarıdaki benchmark sonuçlarına bakılarak en ideal IDictionary yapısı seçilebilir.

- **Eleman Ekleme:** Dictionary, LinearProbing ve QuadraticProbing veri yapılarının benchmark sonuçlarına bakacak olursak en hızlı eleman ekleme işlemi Dictionary'de yapılmaktadır. Ancak, LinearProbing ve QuadraticProbing veri yapıları daha az bellek kullanmaktadır. Bu nedenle, eğer veri ekleme işleminin yoğun olduğu bir uygulama varsa bu veri yapılarını kullanmak daha mantıklıdır.
- **Eleman Arama:** Eleman arama işlemi için de Dictionary, AVL Tree, Separate Chaining Hash table veri yapıları en hızlı sonuçları vermektedir. Ayrıca AVL Tree sıralı veriler arasından aramada oldukça performanslı olduğu için indexleme veri yapısı olarak `Forward Index` kullanılacaksa AVL ilk tercihlerden biri olmalıdır.
- **Eleman Silme:** Eleman silme işlemi için de Dictionary, Linear Probing ve Quadratic Probing veri yapıları en hızlı sonuçları vermektedir. Ancak, AVL Tree ve Separate Chaining Hash table veri yapıları daha az bellek kullanmaktadır. Bu nedenle, eğer veri silme işleminin yoğun olduğu bir uygulama varsa bu veri yapılarını kullanmak daha mantıklıdır.

### Inverted Index için Arama Sonuç Karşılaştırması

- **"dünya tarihindeki önemli olaylar"** query'si için (76 sonuç):

| **Veri Yapısı**  | **Arama Süresi (ms)** |
|------------------|-----------------------|
| Dictionary       | 31.0497               |
| SortedDictionary | 30.9794               |
| SortedList       | 29.2782               |
| AVL              | 40.1899               |
| BTree            | 43.895                |
| RedBlack         | 185.167               |
| DoubleHashing    | 52.1998               |
| LinearProbing    | 51.6953               |
| QuadraticProbing | 50.9707               |
| SeparateChaining | 64.2091               |

- **"*"** query'si için (1289 sonuç):

| **Veri Yapısı**  | **Arama Süresi (ms)** |
|------------------|-----------------------|
| Dictionary       | 1756.377              |
| SortedDictionary | 1812.2824             |
| SortedList       | 1760.5035             |
| AVL              | 2753.5104             |
| BTree            | 3031.0451             |
| RedBlack         | 15337.5511            |
| DoubleHashing    | 3646.1078             |
| LinearProbing    | 3667.9882             |
| QuadraticProbing | 3409.1793             |
| SeparateChaining | 4936.7895             |

**Sonuç =>** Inverted Index için yapılan arama sonuçlarına göre C#'ın kendi standart veri yapıları dışında en yüksek performans verenlerin AVL, BTree ağaçları ve Quadratic Probing Hash table olduğu görülmüştür.

### Forward Index için Arama Sonuç Karşılaştırması

- **"dünya tarihindeki önemli olaylar"** query'si için (76 sonuç):

| **Veri Yapısı**  | **Arama Süresi (ms)** |
|------------------|-----------------------|
| Dictionary       | 14.4647               |
| SortedDictionary | 15.2529               |
| SortedList       | 13.7886               |
| AVL              | 11.9566               |
| BTree            | 12.5305               |
| RedBlack         | 20.1193               |
| DoubleHashing    | 13.6318               |
| LinearProbing    | 15.1138               |
| QuadraticProbing | 13.9236               |
| SeparateChaining | 13.6032               |

- **"*"** query'si için (1289 sonuç):

| **Veri Yapısı**  | **Arama Süresi (ms)** |
|------------------|-----------------------|
| Dictionary       | 1188.4944             |
| SortedDictionary | 1238.1173             |
| SortedList       | 1150.1819             |
| AVL              | 1511.7356             |
| BTree            | 1689.3744             |
| RedBlack         | 5259.3687             |
| DoubleHashing    | 1606.2968             |
| LinearProbing    | 1673.9068             |
| QuadraticProbing | 1727.2871             |
| SeparateChaining | 2133.1215             |

**Sonuç =>** Forward Index için yapılan arama sonuçlarına göre C#'ın kendi standart veri yapıları dışında en yüksek performans verenlerin Inverted Index'te olduğu gibi AVL, BTree ağaçları ve Quadratic Probing Hash table olduğu görülmüştür.
