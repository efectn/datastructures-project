using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace datastructures_project.HashTables
{
    public class LinearProbingHashTable<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private struct Entry
        {
            public TKey Key;
            public TValue Value;
            public bool IsTombstone;
        }

        private Entry?[] _table;
        private int _size;
        private int _count;
        private const double LoadFactorThreshold = 0.7;

        public LinearProbingHashTable(int size = 16)
        {
            _size = size;
            _table = new Entry?[_size];
        }

        private int Hash(TKey key) => (key!.GetHashCode() & 0x7FFFFFFF) % _size;

        private void Resize()
        {
            int newSize = _size * 2;
            var oldTable = _table;

            _table = new Entry?[newSize];
            _size = newSize;
            _count = 0;

            foreach (var entry in oldTable)
            {
                if (entry.HasValue && !entry.Value.IsTombstone)
                {
                    Insert(entry.Value.Key, entry.Value.Value);
                }
            }
        }

        private void Insert(TKey key, TValue value)
        {
            int index = Hash(key);
            for (int i = 0; i < _size; i++)
            {
                int probe = (index + i) % _size;
                if (!_table[probe].HasValue || _table[probe]!.Value.IsTombstone)
                {
                    _table[probe] = new Entry { Key = key, Value = value, IsTombstone = false };
                    _count++;
                    return;
                }
            }

            throw new InvalidOperationException("Hash table is full.");
        }

        public void Add(TKey key, TValue value)
        {
            if (ContainsKey(key))
                throw new ArgumentException("Key already exists.");

            if (_count >= _size * LoadFactorThreshold)
            {
                Resize();
            }

            Insert(key, value);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            int index = Hash(key);

            for (int i = 0; i < _size; i++)
            {
                int probe = (index + i) % _size;
                var entry = _table[probe];

                if (!entry.HasValue)
                    break;

                if (!entry.Value.IsTombstone && EqualityComparer<TKey>.Default.Equals(entry.Value.Key, key))
                {
                    value = entry.Value.Value;
                    return true;
                }
            }

            value = default!;
            return false;
        }

        public bool ContainsKey(TKey key) => TryGetValue(key, out _);

        public bool Remove(TKey key)
        {
            int index = Hash(key);

            for (int i = 0; i < _size; i++)
            {
                int probe = (index + i) % _size;
                var entry = _table[probe];

                if (!entry.HasValue)
                    break;

                if (!entry.Value.IsTombstone && EqualityComparer<TKey>.Default.Equals(entry.Value.Key, key))
                {
                    _table[probe] = new Entry { Key = entry.Value.Key, Value = default!, IsTombstone = true };
                    _count--;
                    return true;
                }
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
                if (key == null) throw new ArgumentNullException(nameof(key));

                int index = Hash(key);
                int i = 0;

                while (i < _size)
                {
                    int probeIndex = (index + i) % _size;
                    var entry = _table[probeIndex];

                    if (!entry.HasValue)
                        break;

                    if (!entry.Value.IsTombstone && EqualityComparer<TKey>.Default.Equals(entry.Value.Key, key))
                    {
                        _table[probeIndex] = new Entry { Key = key, Value = value, IsTombstone = false };
                        return;
                    }
                    i++;
                }

                Add(key, value);
            }
        }

        public ICollection<TKey> Keys => _table
            .Where(e => e.HasValue && !e.Value.IsTombstone)
            .Select(e => e!.Value.Key).ToList();

        public ICollection<TValue> Values => _table
            .Where(e => e.HasValue && !e.Value.IsTombstone)
            .Select(e => e!.Value.Value).ToList();

        public int Count => _count;

        public bool IsReadOnly => false;

        public void Clear()
        {
            _table = new Entry?[_size];
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
                array[arrayIndex++] = pair;
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            if (Contains(item))
                return Remove(item.Key);
            return false;
        }

        public bool IsInCollide(TKey key)
        {
            int index = Hash(key);

            for (int i = 0; i < _size; i++)
            {
                int probe = (index + i) % _size;
                var entry = _table[probe];

                if (entry.HasValue && !entry!.Value.IsTombstone && EqualityComparer<TKey>.Default.Equals(entry!.Value.Key, key))
                {
                    return i > 0;
                }
            }
            return false;
        }

        public List<int> GetTombstones()
        {
            var tombstones = new List<int>();
            for (int i = 0; i < _size; i++)
            {
                if (_table[i].HasValue && _table[i]!.Value.IsTombstone)
                {
                    tombstones.Add(i);
                }
            }
            return tombstones;
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach (var e in _table)
            {
                if (e.HasValue && !e.Value.IsTombstone)
                    yield return new KeyValuePair<TKey, TValue>(e.Value.Key, e.Value.Value);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
