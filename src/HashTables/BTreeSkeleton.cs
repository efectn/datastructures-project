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
        Count++;
    }
    void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
    {
        Add(item.Key, item.Value);
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
        set
        {
            Remove(key);
            Add(key, value);
        }
    }

    public ICollection<TKey> Keys
    {
        get
        {
            var keys = new List<TKey>();
            Traverse(_root, (k, _) => keys.Add(k));
            return keys;
        }
    }

    public ICollection<TValue> Values
    {
        get
        {
            var values = new List<TValue>();
            Traverse(_root, (_, v) => values.Add(v));
            return values;
        }
    }

    public int Count { get; private set; } = 0;

    public bool IsReadOnly => false;

    public void Clear()
    {
        _root = new Node();
        Count = 0;
    }

    public bool Remove(TKey key)
    {
        if (!ContainsKey(key)) return false;

        Remove(_root, key);
        if (_root.Keys.Count == 0 && !_root.IsLeaf)
            _root = _root.Children[0];

        Count--;
        return true;
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        if (Contains(item))
        {
            return Remove(item.Key);
        }
        return false;
    }

    private void Remove(Node node, TKey key)
    {
        int idx = node.Keys.FindIndex(k => k.CompareTo(key) >= 0);

        if (idx >= 0 && idx < node.Keys.Count && node.Keys[idx].CompareTo(key) == 0)
        {
            if (node.IsLeaf)
            {
                node.Keys.RemoveAt(idx);
                node.Values.RemoveAt(idx);
            }
            else
            {
                if (node.Children[idx].Keys.Count >= _t)
                {
                    (TKey predKey, TValue predVal) = GetPredecessor(node.Children[idx]);
                    node.Keys[idx] = predKey;
                    node.Values[idx] = predVal;
                    Remove(node.Children[idx], predKey);
                }
                else if (node.Children[idx + 1].Keys.Count >= _t)
                {
                    (TKey succKey, TValue succVal) = GetSuccessor(node.Children[idx + 1]);
                    node.Keys[idx] = succKey;
                    node.Values[idx] = succVal;
                    Remove(node.Children[idx + 1], succKey);
                }
                else
                {
                    Merge(node, idx);
                    Remove(node.Children[idx], key);
                }
            }
        }
        else if (!node.IsLeaf)
        {
            if (idx < 0) idx = node.Keys.Count;
            if (node.Children[idx].Keys.Count < _t)
                Fill(node, idx);
            Remove(node.Children[idx], key);
        }
    }

    private void Fill(Node node, int idx)
    {
        if (idx > 0 && node.Children[idx - 1].Keys.Count >= _t)
            BorrowFromPrev(node, idx);
        else if (idx < node.Children.Count - 1 && node.Children[idx + 1].Keys.Count >= _t)
            BorrowFromNext(node, idx);
        else
        {
            if (idx < node.Children.Count - 1)
                Merge(node, idx);
            else
                Merge(node, idx - 1);
        }
    }

    private void BorrowFromPrev(Node node, int idx)
    {
        var child = node.Children[idx];
        var sibling = node.Children[idx - 1];

        child.Keys.Insert(0, node.Keys[idx - 1]);
        child.Values.Insert(0, node.Values[idx - 1]);

        if (!child.IsLeaf)
            child.Children.Insert(0, sibling.Children[^1]);

        node.Keys[idx - 1] = sibling.Keys[^1];
        node.Values[idx - 1] = sibling.Values[^1];

        sibling.Keys.RemoveAt(sibling.Keys.Count - 1);
        sibling.Values.RemoveAt(sibling.Values.Count - 1);
        if (!sibling.IsLeaf)
            sibling.Children.RemoveAt(sibling.Children.Count - 1);
    }

    private void BorrowFromNext(Node node, int idx)
    {
        var child = node.Children[idx];
        var sibling = node.Children[idx + 1];

        child.Keys.Add(node.Keys[idx]);
        child.Values.Add(node.Values[idx]);

        if (!child.IsLeaf)
            child.Children.Add(sibling.Children[0]);

        node.Keys[idx] = sibling.Keys[0];
        node.Values[idx] = sibling.Values[0];

        sibling.Keys.RemoveAt(0);
        sibling.Values.RemoveAt(0);
        if (!sibling.IsLeaf)
            sibling.Children.RemoveAt(0);
    }

    private void Merge(Node node, int idx)
    {
        var child = node.Children[idx];
        var sibling = node.Children[idx + 1];

        child.Keys.Add(node.Keys[idx]);
        child.Values.Add(node.Values[idx]);

        child.Keys.AddRange(sibling.Keys);
        child.Values.AddRange(sibling.Values);

        if (!child.IsLeaf)
            child.Children.AddRange(sibling.Children);

        node.Keys.RemoveAt(idx);
        node.Values.RemoveAt(idx);
        node.Children.RemoveAt(idx + 1);
    }

    private (TKey, TValue) GetPredecessor(Node node)
    {
        while (!node.IsLeaf)
            node = node.Children[^1];
        return (node.Keys[^1], node.Values[^1]);
    }

    private (TKey, TValue) GetSuccessor(Node node)
    {
        while (!node.IsLeaf)
            node = node.Children[0];
        return (node.Keys[0], node.Values[0]);
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        if (TryGetValue(item.Key, out var val))
            return EqualityComparer<TValue>.Default.Equals(val, item.Value);
        return false;
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        foreach (var kvp in this)
        {
            array[arrayIndex++] = kvp;
        }
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        var list = new List<KeyValuePair<TKey, TValue>>();
        Traverse(_root, (k, v) => list.Add(new KeyValuePair<TKey, TValue>(k, v)));
        return list.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private void Traverse(Node node, Action<TKey, TValue> action)
    {
        int i;
        for (i = 0; i < node.Keys.Count; i++)
        {
            if (!node.IsLeaf)
                Traverse(node.Children[i], action);
            action(node.Keys[i], node.Values[i]);
        }
        if (!node.IsLeaf)
            Traverse(node.Children[i], action);
    }
}