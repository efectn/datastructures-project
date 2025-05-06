using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace datastructures_project.HashTables
{
    public class LinearProbingHashTable<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private KeyValuePair<TKey, TValue>?[] _table;
        private int _size;
        private int _count;

        public LinearProbingHashTable(int size = 16)
        {
            _size = size;
            _table = new KeyValuePair<TKey, TValue>?[_size];
        }

        private int Hash(TKey key) => Math.Abs(key!.GetHashCode()) % _size;

        public void Add(TKey key, TValue value)
        {
            if (ContainsKey(key))
                throw new ArgumentException("Key already exists.");

            int index = Hash(key);
            int i = 0;

            while (_table[(index + i) % _size].HasValue)
            {
                i++;
                if (i >= _size) throw new InvalidOperationException("Hash table is full.");
            }

            _table[(index + i) % _size] = new KeyValuePair<TKey, TValue>(key, value);
            _count++;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            int index = Hash(key);
            int i = 0;

            while (_table[(index + i) % _size].HasValue)
            {
                var kv = _table[(index + i) % _size]!.Value;
                if (kv.Key!.Equals(key))
                {
                    value = kv.Value;
                    return true;
                }
                i++;
                if (i >= _size) break;
            }

            value = default!;
            return false;
        }

        public bool ContainsKey(TKey key)
        {
            return TryGetValue(key, out _);
        }

        public bool Remove(TKey key)
        {
            int index = Hash(key);
            int i = 0;

            while (_table[(index + i) % _size].HasValue)
            {
                var kv = _table[(index + i) % _size]!.Value;
                if (kv.Key!.Equals(key))
                {
                    _table[(index + i) % _size] = null;
                    _count--;
                    return true;
                }
                i++;
                if (i >= _size) break;
            }

            return false;
        }

        public TValue this[TKey key]
        {
            get
            {
                if (TryGetValue(key, out TValue val))
                    return val;

                throw new KeyNotFoundException();
            }
            set
            {
                // Update if exists
                int index = Hash(key);
                int i = 0;

                while (_table[(index + i) % _size].HasValue)
                {
                    if (_table[(index + i) % _size]!.Value.Key!.Equals(key))
                    {
                        _table[(index + i) % _size] = new KeyValuePair<TKey, TValue>(key, value);
                        return;
                    }
                    i++;
                }

                // Insert new
                Add(key, value);
            }
        }

        public ICollection<TKey> Keys => _table.Where(x => x.HasValue).Select(x => x!.Value.Key).ToList();

        public ICollection<TValue> Values => _table.Where(x => x.HasValue).Select(x => x!.Value.Value).ToList();

        public int Count => _count;

        public bool IsReadOnly => false;

        public void Clear()
        {
            _table = new KeyValuePair<TKey, TValue>?[_size];
            _count = 0;
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return TryGetValue(item.Key, out TValue value) && EqualityComparer<TValue>.Default.Equals(value, item.Value);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0 || arrayIndex > array.Length)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            if (array.Length - arrayIndex < Count)
                throw new ArgumentException("Insufficient space.");

            foreach (var pair in this)
            {
                array[arrayIndex++] = pair;
            }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            if (Contains(item))
            {
                return Remove(item.Key);
            }
            return false;
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach (var kv in _table)
            {
                if (kv.HasValue)
                    yield return kv.Value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
