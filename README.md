# Todo List

- [ ] Create tokenizer.
    - [ ] Add Turkish stemmer support to tokenizer.
    - [x] Add Turkish stop words to tokenizer.
    - [ ] Create unit-tests for tokenizer.
- [ ] Create indexers.
    - [ ] Add inverted index.
      - [ ] Create unit-tests and benchmarks.
    - [ ] Add forward index.
      - [ ] Create unit-tests and benchmarks.
    - [ ] Store indexed data in SQL or NoSQL database.
      - [ ] Create unit-tests and benchmarks.
- [ ] Add search scoring algorithms.
  - [ ] Implement BM25.
  - [ ] Implement TF-IDF.
  - [ ] Create unit-tests and benchmarks, compare them with each other.
- [ ] Add support for Trie data structure.
  - [ ] Implement autocompletion.
  - [ ] Add support for wildcard search queries.
  - [ ] Add unit-tests.
- [ ] Add Levenshtein distance algorithm for word similarity check.
  - [ ] Create unit-tests and benchmarks.
- [ ] Backend & frontend.
  - [ ] Add backend using ASP.NET Core.
  - [ ] Add simple frontend using the files in static directory with Scriban template engine.
  - [ ] Add logging support.
  - [ ] Prometheus integration for tracing.
  - [ ] Docker integration.
- [ ] Implement hashtable functions.
  - [ ] Separate chaining.
  - [ ] Linear probing.
  - [ ] Quadratic probing.
  - [ ] Double hashing.
  - [ ] Create unit-tests for all hashtable functions.
  - [ ] Create benchmarks and compare hashtable functions of in terms of performance, cluttering etc.
  - [ ] Add hashtable support to indexers as well as .NET's Dictionary, SortedList implementations and compare them.
- [ ] Implement B-tree for indexers.
  - [ ] Create unit-tests for B-tree.
  - [ ] Create benchmarks and compare B-tree with other hashtable functions.

# Used Data Structures

- Hashtable
- Linked List (for linked list implementation of hashtable)
- Trie
- B-tree