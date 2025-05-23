﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace datastructures_project.HashTables
{
    public class DoubleHashingHashTable<TKey, TValue> : ILinearQuadraticDoubleHashing<TKey, TValue>
    {
        private struct Entry
        {
            public TKey Key;
            public TValue Value;
            public bool IsTombstone; // Tombstone flag
        }

        private Entry?[] _entries;
        private int _count;
        private int _size;
        private readonly IEqualityComparer<TKey> _comparer;
        private const double LoadFactorThreshold = 0.75;

        public DoubleHashingHashTable(int capacity = 16, IEqualityComparer<TKey> comparer = null)
        {
            _size = GetNextPrime(capacity);
            _entries = new Entry?[_size];
            _comparer = comparer ?? EqualityComparer<TKey>.Default;
        }

        // Primary hash function
        private int Hash1(TKey key) => Math.Abs(_comparer.GetHashCode(key) & 0x7FFFFFFF) % _size;

        // Secondary hash function (must never return 0)
        private int Hash2(TKey key) => 1 + (Math.Abs(_comparer.GetHashCode(key) & 0x7FFFFFFF) % (_size - 1));

        private static int GetNextPrime(int min)
        {
            foreach (int candidate in Primes)
            {
                if (candidate >= min) return candidate;
            }
            
            for (int i = min | 1; i < int.MaxValue; i += 2)
            {
                if (IsPrime(i)) return i;
            }
            return min;
        }

        private static bool IsPrime(int number)
        {
            if (number < 2) return false;
            if (number % 2 == 0) return number == 2;
            
            int boundary = (int)Math.Sqrt(number);
            for (int i = 3; i <= boundary; i += 2)
            {
                if (number % i == 0) return false;
            }
            return true;
        }

        private static readonly int[] Primes = {
            3, 7, 11, 17, 23, 29, 37, 47, 59, 71, 89, 107, 131, 163, 197, 239, 293, 353, 431, 521, 631, 761,
            919, 1103, 1327, 1597, 1931, 2333, 2801, 3371, 4049, 4861, 5839, 7013, 8419, 10103, 12143, 14591,
            17519, 21023, 25229, 30293, 36353, 43627, 52361, 62851, 75431, 90523, 108631, 130363, 156437,
            187751, 225307, 270371, 324449, 389357, 467237, 560689, 672827, 807403, 968897, 1162687, 1395263,
            1674319, 2009191, 2411033, 2893249, 3471899, 4166287, 4999559, 5999471, 7199369
        };

        private void Resize()
        {
            int newSize = GetNextPrime(_size * 2);
            var oldEntries = _entries;
            _entries = new Entry?[newSize];
            _size = newSize;
            _count = 0;

            foreach (var entry in oldEntries)
            {
                if (entry.HasValue && !entry.Value.IsTombstone) // Only add non-tombstone entries
                {
                    AddInternal(entry.Value.Key, entry.Value.Value);
                }
            }
        }

        private void AddInternal(TKey key, TValue value)
        {
            int hash1 = Hash1(key);
            int hash2 = Hash2(key);
            int i = 0;

            while (i < _size)
            {
                int index = (hash1 + i * hash2) % _size;
                if (!_entries[index].HasValue || _entries[index]!.Value.IsTombstone)
                {
                    _entries[index] = new Entry { Key = key, Value = value, IsTombstone = false };
                    _count++;
                    return;
                }

                i++;
            }
            
            throw new InvalidOperationException("Hash table is full");
        }


        public void Add(TKey key, TValue value)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (ContainsKey(key)) throw new ArgumentException("Key already exists");

            if (_count >= _size * LoadFactorThreshold)
            {
                Resize();
            }

            AddInternal(key, value);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            int hash1 = Hash1(key);
            int hash2 = Hash2(key);
            int i = 0;

            while (i < _size)
            {
                int index = (hash1 + i * hash2) % _size;
                
                var entry = _entries[index];

                if (!entry.HasValue)
                    break;

                if (!entry.Value.IsTombstone && EqualityComparer<TKey>.Default.Equals(entry.Value.Key, key))
                {
                    value = entry.Value.Value;
                    return true;
                }

                i++;
            }

            value = default!;
            return false;
        }

        public bool ContainsKey(TKey key) => TryGetValue(key, out _);

        public bool Remove(TKey key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            int hash1 = Hash1(key);
            int hash2 = Hash2(key);
            int i = 0;

            while (i < _size)
            {
                int index = (hash1 + i * hash2) % _size;
                
                var entry = _entries[index];

                if (!entry.HasValue)
                    break;

                if (!entry.Value.IsTombstone && EqualityComparer<TKey>.Default.Equals(entry.Value.Key, key))
                {
                    _entries[index] = new Entry { Key = entry.Value.Key, Value = default!, IsTombstone = true };
                    _count--;
                    return true;
                }

                i++;
            }

            return false;
        }
        
        public bool IsInCollide(TKey key) 
        {
            int hash1 = Hash1(key);
            int hash2 = Hash2(key);
            int i = 0;

            while (i < _size)
            {
                int index = (hash1 + i * hash2) % _size;
                var entry = _entries[index];
                
                if (entry.HasValue && !entry.Value.IsTombstone && EqualityComparer<TKey>.Default.Equals(entry.Value.Key, key))
                {
                    return i > 0;
                }

                i++;
            }

            return false;
        }
        
        public List<int> GetTombstones() 
        {
            var tombstones = new List<int>();
            for (int i = 0; i < _size; i++)
            {
                if (_entries[i].HasValue && _entries[i]!.Value.IsTombstone)
                {
                    tombstones.Add(i);
                }
            }
            return tombstones;
        }
        
        public IEnumerator<KeyValuePair<int, KeyValuePair<TKey, TValue>>> GetEnumeratorWithIndex()
        {
            for (int i = 0; i < _size; i++)
            {
                if (_entries[i].HasValue)
                {
                    yield return new KeyValuePair<int, KeyValuePair<TKey, TValue>>(i, new KeyValuePair<TKey, TValue>(_entries[i]!.Value.Key, _entries[i]!.Value.Value));
                }
                else
                {
                    yield return new KeyValuePair<int, KeyValuePair<TKey, TValue>>(-1, new KeyValuePair<TKey, TValue>(default!, default!));
                }
            }
        }

        public TValue this[TKey key]
        {
            get => TryGetValue(key, out TValue value) ? value : throw new KeyNotFoundException();
            set
            {
                if (key == null) throw new ArgumentNullException(nameof(key));

                int hash1 = Hash1(key);
                int hash2 = Hash2(key);
                int i = 0;

                while (i < _size)
                {
                    int index = (hash1 + i * hash2) % _size;
                    var entry = _entries[index];

                    if (!entry.HasValue)
                        break;

                    if (!entry.Value.IsTombstone && EqualityComparer<TKey>.Default.Equals(entry.Value.Key, key))
                    {
                        _entries[index] = new Entry { Key = key, Value = value, IsTombstone = false };
                        return;
                    }
                    i++;
                }

                Add(key, value);
            }
        }

        public ICollection<TKey> Keys => _entries
            .Where(e => e.HasValue && !e.Value.IsTombstone)
            .Select(e => e!.Value.Key).ToList();

        public ICollection<TValue> Values => _entries
            .Where(e => e.HasValue && !e.Value.IsTombstone)
            .Select(e => e!.Value.Value).ToList();

        public int Count => _count;
        public bool IsReadOnly => false;

        public void Clear()
        {
            Array.Clear(_entries, 0, _entries.Length);
            _count = 0;
        }

        public void Add(KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);

        public bool Contains(KeyValuePair<TKey, TValue> item) => 
            TryGetValue(item.Key, out TValue value) && EqualityComparer<TValue>.Default.Equals(value, item.Value);

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0 || arrayIndex >= array.Length) throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            if (array.Length - arrayIndex < Count) throw new ArgumentException("Not enough space");

            foreach (var entry in _entries)
            {
                if (entry.HasValue && !entry.Value.IsTombstone)
                {
                    array[arrayIndex++] = new KeyValuePair<TKey, TValue>(entry.Value.Key, entry.Value.Value);
                }
            }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item) => Contains(item) && Remove(item.Key);

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach (var entry in _entries)
            {
                if (entry.HasValue && !entry.Value.IsTombstone)
                {
                    yield return new KeyValuePair<TKey, TValue>(entry.Value.Key, entry.Value.Value);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
