using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace datastructures_project.HashTables
{
    public class SeparateChainingHashTable<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private LinkedList<KeyValuePair<TKey, TValue>>[] _buckets;
        private int _count;
        private int _size;
        private readonly IEqualityComparer<TKey> _comparer;
        private const double LoadFactorThreshold = 0.75;

        public SeparateChainingHashTable(int capacity = 16, IEqualityComparer<TKey> comparer = null)
        {
            _size = GetNextPrime(capacity);
            _buckets = new LinkedList<KeyValuePair<TKey, TValue>>[_size];
            _comparer = comparer ?? EqualityComparer<TKey>.Default;
        }

        private int GetHash(TKey key) => Math.Abs(_comparer.GetHashCode(key)) % _size;

        private static int GetNextPrime(int min)
        {
            foreach (int prime in Primes)
            {
                if (prime >= min) return prime;
            }
            
            // Fallback for very large sizes
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
            var oldBuckets = _buckets;
            _buckets = new LinkedList<KeyValuePair<TKey, TValue>>[newSize];
            _size = newSize;
            _count = 0;

            foreach (var bucket in oldBuckets)
            {
                if (bucket != null)
                {
                    foreach (var item in bucket)
                    {
                        Add(item.Key, item.Value);
                    }
                }
            }
        }

        public void Add(TKey key, TValue value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (ContainsKey(key))
                throw new ArgumentException("Key already exists");

            if (_count >= _size * LoadFactorThreshold)
            {
                Resize();
            }

            int index = GetHash(key);
            if (_buckets[index] == null)
            {
                _buckets[index] = new LinkedList<KeyValuePair<TKey, TValue>>();
            }

            _buckets[index].AddLast(new KeyValuePair<TKey, TValue>(key, value));
            _count++;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            int index = GetHash(key);
            if (_buckets[index] != null)
            {
                foreach (var item in _buckets[index])
                {
                    if (_comparer.Equals(item.Key, key))
                    {
                        value = item.Value;
                        return true;
                    }
                }
            }

            value = default;
            return false;
        }

        public bool ContainsKey(TKey key) => TryGetValue(key, out _);

        public bool Remove(TKey key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            int index = GetHash(key);
            if (_buckets[index] != null)
            {
                var node = _buckets[index].First;
                while (node != null)
                {
                    if (_comparer.Equals(node.Value.Key, key))
                    {
                        _buckets[index].Remove(node);
                        _count--;
                        return true;
                    }
                    node = node.Next;
                }
            }

            return false;
        }

        public TValue this[TKey key]
        {
            get
            {
                if (TryGetValue(key, out TValue value))
                    return value;
                throw new KeyNotFoundException();
            }
            set
            {
                if (key == null)
                    throw new ArgumentNullException(nameof(key));

                int index = GetHash(key);
                if (_buckets[index] != null)
                {
                    var node = _buckets[index].First;
                    while (node != null)
                    {
                        if (_comparer.Equals(node.Value.Key, key))
                        {
                            _buckets[index].Remove(node);
                            _buckets[index].AddLast(new KeyValuePair<TKey, TValue>(key, value));
                            return;
                        }
                        node = node.Next;
                    }
                }

                Add(key, value);
            }
        }

        public ICollection<TKey> Keys => 
            _buckets.Where(b => b != null)
                   .SelectMany(b => b.Select(i => i.Key))
                   .ToList();

        public ICollection<TValue> Values => 
            _buckets.Where(b => b != null)
                   .SelectMany(b => b.Select(i => i.Value))
                   .ToList();

        public int Count => _count;
        public bool IsReadOnly => false;

        public void Clear()
        {
            _buckets = new LinkedList<KeyValuePair<TKey, TValue>>[_size];
            _count = 0;
        }

        public void Add(KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);

        public bool Contains(KeyValuePair<TKey, TValue> item) => 
            TryGetValue(item.Key, out TValue value) && EqualityComparer<TValue>.Default.Equals(value, item.Value);

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0 || arrayIndex >= array.Length)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            if (array.Length - arrayIndex < Count)
                throw new ArgumentException("Not enough space");

            foreach (var bucket in _buckets)
            {
                if (bucket != null)
                {
                    foreach (var item in bucket)
                    {
                        array[arrayIndex++] = item;
                    }
                }
            }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item) => Contains(item) && Remove(item.Key);

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach (var bucket in _buckets)
            {
                if (bucket != null)
                {
                    foreach (var item in bucket)
                    {
                        yield return item;
                    }
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}