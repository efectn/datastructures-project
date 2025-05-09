namespace datastructures_project.Tests.Search;

using System;
using System.Collections;
using System.Collections.Generic;

public class BTreeDictionary<TKey, TValue> : IDictionary<TKey, TValue> where TKey : IComparable<TKey>
{
    private class Node
    {
        public List<TKey> Keys = new();
        public List<TValue> Values = new();
        public List<Node> Children = new();
        public bool IsLeaf = true;
    }

    private readonly int _t; // minimum degree
    private Node _root;

    public BTreeDictionary(int degree = 2)
    {
        if (degree < 2) throw new ArgumentException("Degree must be >= 2.");
        _t = degree;
        _root = new Node();
    }

    public void Add(TKey key, TValue value)
    {
        if (_root.Keys.Count == 2 * _t - 1)
        {
            var newRoot = new Node { IsLeaf = false };
            newRoot.Children.Add(_root);
            SplitChild(newRoot, 0);
            _root = newRoot;
        }

        InsertNonFull(_root, key, value);
    }

    private void InsertNonFull(Node node, TKey key, TValue value)
    {
        int i = node.Keys.Count - 1;

        if (node.IsLeaf)
        {
            while (i >= 0 && key.CompareTo(node.Keys[i]) < 0) i--;
            node.Keys.Insert(i + 1, key);
            node.Values.Insert(i + 1, value);
        }
        else
        {
            while (i >= 0 && key.CompareTo(node.Keys[i]) < 0) i--;
            i++;
            if (node.Children[i].Keys.Count == 2 * _t - 1)
            {
                SplitChild(node, i);
                if (key.CompareTo(node.Keys[i]) > 0) i++;
            }
            InsertNonFull(node.Children[i], key, value);
        }
    }

    private void SplitChild(Node parent, int index)
    {
        var fullChild = parent.Children[index];
        var newChild = new Node { IsLeaf = fullChild.IsLeaf };

        for (int j = 0; j < _t - 1; j++)
        {
            newChild.Keys.Add(fullChild.Keys[_t + j]);
            newChild.Values.Add(fullChild.Values[_t + j]);
        }

        if (!fullChild.IsLeaf)
        {
            for (int j = 0; j < _t; j++)
                newChild.Children.Add(fullChild.Children[_t + j]);
        }

        fullChild.Keys.RemoveRange(_t - 1, _t);
        fullChild.Values.RemoveRange(_t - 1, _t);
        if (!fullChild.IsLeaf)
            fullChild.Children.RemoveRange(_t, _t);

        parent.Keys.Insert(index, fullChild.Keys[_t - 1]);
        parent.Values.Insert(index, fullChild.Values[_t - 1]);
        parent.Children.Insert(index + 1, newChild);

        fullChild.Keys.RemoveAt(_t - 1);
        fullChild.Values.RemoveAt(_t - 1);
    }

    public bool ContainsKey(TKey key) => TryGetValue(key, out _);

    public bool TryGetValue(TKey key, out TValue value)
    {
        return Search(_root, key, out value);
    }

    private bool Search(Node node, TKey key, out TValue value)
    {
        int i = 0;
        while (i < node.Keys.Count && key.CompareTo(node.Keys[i]) > 0) i++;

        if (i < node.Keys.Count && key.CompareTo(node.Keys[i]) == 0)
        {
            value = node.Values[i];
            return true;
        }

        if (node.IsLeaf)
        {
            value = default;
            return false;
        }

        return Search(node.Children[i], key, out value);
    }

    public TValue this[TKey key]
    {
        get
        {
            if (TryGetValue(key, out var val))
                return val;
            throw new KeyNotFoundException();
        }
        set => Add(key, value); // Overwrites existing keys (you can change this to prevent duplicates)
    }

    #region Not Fully Implemented IDictionary Members
    public ICollection<TKey> Keys => throw new NotImplementedException();
    public ICollection<TValue> Values => throw new NotImplementedException();
    public int Count => throw new NotImplementedException();
    public bool IsReadOnly => false;
    public void Add(KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);
    public void Clear() => throw new NotImplementedException();
    public bool Remove(TKey key) => throw new NotImplementedException();
    public bool Remove(KeyValuePair<TKey, TValue> item) => throw new NotImplementedException();
    public bool Contains(KeyValuePair<TKey, TValue> item) => throw new NotImplementedException();
    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => throw new NotImplementedException();
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => throw new NotImplementedException();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    #endregion
}
