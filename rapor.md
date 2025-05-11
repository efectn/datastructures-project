# .NET ile Collision Resolution Teknikleri Kullanarak Arama Motoru Geliştirilmesi

## Proje Açıklaması

Projenin amacı hashtable fonksiyonları kullanılarak belirli anahtar kelimelerle eşleşen içeriklerin hızlıca bulunabilmesi için hash tablosu kullanılarak bir arama motoru prototipi geliştirilmesidir. Bu amaç doğrultusunda çeşitli arama algoritmaları ve hashtable fonksiyonları implement edilip performans, bellek kullanımı gibi metrikler bakımından karşılaştırılmıştır. Ayrıca proje kapsamıyla sınırlı kalmayarak hashtable'lara ek olarak dengeli arama ağaçları da eklenmiş ve performans bakımından karşılaştırılmıştır.

Ek olarak, sisteme eklenen auto-completion, wildcard arama, levenshtein distance gibi özelliklerde de gerçek hayat senaryolarına uygun bir arama motoru geliştirilmesi sağlanmıştır.

## Kullanılan Teknolojiler

- **.NET 9.0:** C# framework'ü ile geliştirilmiştir.
- **ASP.NET Core:** Backend tarafının geliştirilmesinde kullanılmıştır.
- **SQLite:** Çalışmak için ayrı bir daemona ihtiyaç duymadığından PostgreSQL, MariaDB gibi veri tabanlarının yerine kullanılmıştır.
- **Entity Framework Core:** SQLite ile veri tabanı işlemlerini gerçekleştirmek için varsayılan ORM olarak kullanılmıştır.
- **Scriban:** Projede varsayılan template motoru olarak kullanılmıştır.
- **xUnit:** Projedeki veri yapıları ve algoritmaların test edilmesi için kullanılmıştır. Yazılan unit testler sayesinde projenin her aşamasında ekstra bir kontrole ihtiyaç kalmadan kodun doğru çalıştığı garanti edilmiştir.
- **BenchmarkDotNet:** Projedeki veri yapıları ve algoritmaların performans testleri için kullanılmıştır.
- **Prometheus:** Projenin bellek, CPU kullanımı, arama sayısı gibi metriklerini izlemek için kullanılmıştır.
- **Grafana:** Prometheus ile toplanan metriklerin görselleştirilmesi için kullanılmıştır.
- **Docker:** Projenin konteyner ortamında çalıştırılması için kullanılmıştır. Bu sayede proje izole bir ortamda sorunsuz çalışabilecektir.

## Kurulum

## Web Arayüzü ve Açıklamalar

## Arama Mekanizması

Arama işlemi temelde 3 yapıya ayrılmıştır: tokenizer, indexleme, arama skor algoritmaları.

### Tokenizer

Tokenizer'ın temel amacı, verilen metni tokenize ederek kelime listesine dönüştürmektir. 

Bu işlem sırasında durdurma kelimeleri (stop-words) filtrelenebilir ve sözcükler köklerine indirgenebilir (stemmer). Bu işlemler, yapılandırma ayarlarına bağlı olarak devreye alınır. Aşağıda tokenizer aşamaları örnekle verilmiştir:

- Tokenizer ilk olarak verilen metni küçük parçalara ayırır. `"Bu, bir deneme metnidir. " => "bu, bir deneme metnidir."`
- Noktalama işaretleri ve bazı özel karakterler temizlenir. Bu sayede arama sonuçları daha doğru olur. `"bu, bir deneme metnidir." => "bu bir deneme metnidir"`
- Noktalama işaretleri silinen ve küçük harfe dönüştürülen metin boşluk karakterine göre tokenlere ayrılır. `"bu bir deneme metnidir" => ["bu", "bir", "deneme", "metnidir"]`
- Durdurma kelimeleri (stop-words) filtrelenir. Hangi kelimelerin stop-words olacağı `Resources/stop-words.txt` dosyasından belirlenebilir. `"bu bir deneme metnidir" => ["deneme", "metnidir"]`
- Son olarak kelimeler [Snowball](https://snowballstem.org/algorithms/turkish/stemmer.html) stemmer algoritması ile köklerine indirilir ve çekim ekleri silinmiş tokenler tokenizer tarafından döndürülür. `"deneme", "metnidir" => ["denem", "metni"]`

**Not:** Snowball stemmer Türkçe için çok doğru çalışmadığı için bazen kelimeleri yanlış köke indirgeyebilmektedir. Bunun önüne geçmek için stemmer tarafından değiştirilmemesi istenen kelimeler ve kökler `Resources/ignore-words.txt` dosyasından belirlenebilir.
**Not:** Stemmer ve stop-words aşamaları `appsetting.json` dosyasından devre dışı bırakılabilir.

### Indeksleme

Projede indexleme işlemi için Elasticsearch, Solr gibi arama motorlarında da kullanılan inverted index ve forward index algoritmaları kullanılmıştır. Hangi veri yapısının kullanılacağı `appsettings.json` dosyasından belirlenebilir.

#### Inverted Index

InvertedIndex, her kelime için dökümantaların ve terim frekanslarının tutulduğu bir ters indeks yapısını temsil eder. Temelde bir hash-table veya ağaç yapısı kullanılarak oluşturulmuş olan bir `IDictionary` yapısı üzerine kurulu olan bu veri yapısı, metin verilerinin indekslenmesi, kelimelerin sıklıklarının hesaplanması, dökümantaların yönetilmesi ve arama yapılması gibi temel işlevleri yerine getirir. Örneğin `["deneme", "metni", "deneme"]` ve `["bilgisayar", "telefon"]` dökümanları inverted index veri yapısında  aşağıdaki tablodaki gibi tutulur:

| Kelime | Döküman ID'leri | Terim Frekansı |
|--------|------------------|----------------|
| deneme | 1, 1, 2            | 3              |
| metni  | 1                | 1              |
| bilgisayar | 2            | 1              |
| telefon | 2                | 1              |
- `Döküman ID'leri`: Kelimenin geçtiği dökümanların ID'leri
- `Terim Frekansı`: Kelimenin döküman içinde kaç kez geçtiği
- `InvertedIndex` veri yapısı, kelimelerin döküman ID'leri ve terim frekansları ile birlikte tutulmasını sağlar. Bu sayede arama işlemleri daha hızlı ve verimli bir şekilde gerçekleştirilebilir.

Projede geliştirmiş olduğumuz Inverted Index yapısına ait metodların algoritmik zaman karmaşıklığı aşağıdaki tablodaki gibidir:

| Metod | Best Case | Worst Case |
|--------|-----------|------------|
| Add | O(n)          | O(n^2)       |
| DocumentCount | O(n)         | O(n^2)        |
| DocumentWordsCount | O(1)          | O(n^2)         |
| WordDocuments **(Search)** | O(1)          | O(1)         |
| DocumentIds | O(n^2)          | O(n^2)         |
| Tokens        | O(n^2)          | O(n^3)         |
| DocumentIds | O(n^2)          | O(n^2)         |
| Remove | O(1)          | O(n^2)         |

**Not:** Karmaşıklık hesabı yapılırken Trie veri yapısının etkisi göz ardı edilmiştir.

#### Forward Index

ForwardIndex, her döküman için kelimeleri ve terim frekanslarını tutan bir veri yapısını temsil eder. InvertedIndex'in aksine ForwardIndex döküman ID'lerini key, tokenleri ve terim frekanslarını value olarak tutar. Bu yapı, döküman eklemede hızlı bir şekilde çalışır. Ancak, kelimenin hangi dökümanlarda geçtiğine dair bir sorgulama yapılacaksa, bu işlem inverted index yapısına göre daha verimsiz olacaktır.

Bu yapı, dökümanların kelime dağılımını hızlı bir şekilde gözlemlemek için kullanılır. Örneğin `["deneme", "metni"]` ve `["bilgisayar", "telefon"]` dökümanları forward index veri yapısında aşağıdaki tablodaki gibi tutulur:

| Döküman ID | Kelime | Terim Frekansı |
|------------|--------|----------------|
| 1          | deneme | 2              |
| 1          | metni  | 1              |
| 2          | bilgisayar | 1          |
| 2          | telefon | 1              |

- `Döküman ID`: Kelimenin bulunduğu dökümanın kimliği
- `Kelime`: Dökümanda geçen kelime
- `Terim Frekansı`: Kelimenin dökümanda kaç kez geçtiği

Projede geliştirilmiş olan Forward Index yapısının algoritmik zaman karmaşıklığı aşağıdaki tablodaki gibidir:

| Metod              | Best Case | Worst Case |
| ------------------ | --------- | ---------- |
| Add                | O(n)      | O(n^2)     |
| DocumentCount      | O(1)      | O(1)       |
| DocumentWordsCount | O(1)      | O(n)       |
| WordDocuments **(Search)**      | O(n)      | O(n^2)       |
| DocumentLength     | O(1)      | O(n)       |
| Tokens             | O(n)      | O(n)     |
| DocumentIds        | O(1)      | O(1)       |
| Remove             | O(1)      | O(1)       |


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
| InvertedIndex_WordDocuments_Empty  | 1000 |   5.427 us |  3.111 us |  47.218 us |   0.6010 us |   0.4610 us |   577.4 us |     736 B |
| InvertedIndex_DocumentLength_Empty | 1000 |  36.714 us |  2.119 us |  32.162 us |  31.1395 us |  28.8550 us |   408.8 us |    1296 B |
| InvertedIndex_GetWords_Empty       | 1000 |  11.479 us |  3.561 us |  54.043 us |   5.1600 us |   4.4380 us |   668.2 us |    1072 B |
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
| ForwardIndex_WordDocuments_Empty  | 1000 | 30.448 us | 6.325 us |  95.995 us | 19.5675 us | 15.5700 us | 1,117.9 us |     824 B |
| ForwardIndex_DocumentLength_Empty | 1000 |  3.359 us | 1.913 us |  29.039 us |  0.4110 us |  0.2600 us |   360.1 us |     736 B |
| ForwardIndex_GetWords_Empty       | 1000 | 10.983 us | 3.382 us |  51.323 us |  5.0500 us |  4.3890 us |   613.3 us |    1072 B |
| ForwardIndex_DocumentIds          | 1000 | 12.689 us | 6.725 us | 102.061 us |  1.9630 us |  1.6630 us | 1,539.7 us |     992 B |
| ForwardIndex_Tokens               | 1000 | 15.547 us | 6.993 us | 106.134 us |  4.3680 us |  3.7370 us | 1,336.4 us |    1464 B |
| ForwardIndex_AddRemove            | 1000 | 42.757 us | 3.453 us |  52.402 us | 33.2280 us | 29.8070 us |   643.4 us |    8008 B |
```

Benchmark sonuçlarından görülebileceği gibi inverted index algoritması eleman arama işlemlerinde 6 kat daha hızlı çalışırken eleman ekleme işleminde 7 kat, eleman silme işleminde ise 13 kat daha yavaş çalışmaktadır. Bu sonuçlara göre de algoritmaların karmaşıklık hesapları doğrulanmış olmaktadır.ü,

### Arama Skor Algoritmaları

Projede arama sonuçlarının hesaplanması için TF-IDF ve Okapi BM25 algoritmaları tercih edilmiştir.

## TF-IDF

## Okapi BM25

### Auto-completion ve Wildcard Arama

Sistemde auto-completion ve wildcard özelliklerini sunmak için Trie (Prefix-tree) veri yapısı kullanılmıştır. 

Indexleme sırasında kelimelerin kökleri Trie veri yapısına eklenir. Kullanıcı arama yaptığında, Trie veri yapısı kullanılarak kelimenin kökü ile başlayan kelimeler listelenir. Bu sayede kullanıcıya hızlı bir şekilde öneriler sunulabilir.


### Levenshtein Distance

...

## Indekslenen Verinin Tutulması için Kullanılan Veri Yapıları

Arama mekanizmasının indexleme kısmında aşağıdaki veri yapıları kullanılmıştır:

### Linear Probing Hash Table

### Quadratic Probing Hash Table

### Double Hashing Hash Table

### Separate Chaining Hash Table

### AVL Tree

### Red-Black Tree

### B Tree

### C#'ın Varsayılan Dictionary, SortedList ve SortedDictionary Yapıları