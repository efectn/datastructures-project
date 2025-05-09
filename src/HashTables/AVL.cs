using System;
using System.Collections;
using System.Collections.Generic;

public class AVLTreeDictionary<TKey, TValue> : IDictionary<TKey, TValue> where TKey : IComparable<TKey>
{
    private class Node
    {
        public TKey Key;
        public TValue Value;
        public Node Left, Right;
        public int Height;

        public Node(TKey key, TValue value)
        {
            Key = key;
            Value = value;
            Height = 1;
        }
    }

    private Node root;
    private int count;

    public int Count => count;
    public bool IsReadOnly => false;

    // ICollection için anahtar ve değer koleksiyonları
    public ICollection<TKey> Keys
    {
        get
        {
            List<TKey> keys = new List<TKey>();
            InOrderTraversal(root, n => keys.Add(n.Key));
            return keys;
        }
    }

    public ICollection<TValue> Values
    {
        get
        {
            List<TValue> values = new List<TValue>();
            InOrderTraversal(root, n => values.Add(n.Value));
            return values;
        }
    }

    // Ekleme işlemi
    public void Add(TKey key, TValue value)
    {
        root = Insert(root, key, value);
    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {
        Add(item.Key, item.Value);
    }

    // Elemanı silme
    public bool Remove(TKey key)
    {
        bool existed = ContainsKey(key);
        if (existed)
        {
            root = Remove(root, key);
            count--;
        }
        return existed;
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        return Remove(item.Key);
    }

    public bool ContainsKey(TKey key)
    {
        return Find(root, key) != null;
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        var node = Find(root, key);
        if (node != null)
        {
            value = node.Value;
            return true;
        }
        value = default;
        return false;
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        var node = Find(root, item.Key);
        return node != null && EqualityComparer<TValue>.Default.Equals(node.Value, item.Value);
    }

    public TValue this[TKey key]
    {
        get
        {
            var node = Find(root, key);
            if (node == null) throw new KeyNotFoundException();
            return node.Value;
        }
        set
        {
            root = Insert(root, key, value);
        }
    }

    public void Clear()
    {
        root = null;
        count = 0;
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        foreach (var kv in this)
        {
            array[arrayIndex++] = kv;
        }
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        List<KeyValuePair<TKey, TValue>> list = new List<KeyValuePair<TKey, TValue>>();
        InOrderTraversal(root, n => list.Add(new KeyValuePair<TKey, TValue>(n.Key, n.Value)));
        return list.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    // AVL Yardımcı Fonksiyonlar
    private Node Find(Node node, TKey key)
    {
        if (node == null) return null;
        int cmp = key.CompareTo(node.Key);
        if (cmp < 0) return Find(node.Left, key);
        if (cmp > 0) return Find(node.Right, key);
        return node;
    }

    private Node Insert(Node node, TKey key, TValue value)
    {
        if (node == null)
        {
            count++;
            return new Node(key, value);
        }

        int cmp = key.CompareTo(node.Key);
        if (cmp < 0)
            node.Left = Insert(node.Left, key, value);
        else if (cmp > 0)
            node.Right = Insert(node.Right, key, value);
        else
            node.Value = value; // Anahtar zaten varsa, değeri güncelle

        return Balance(node);
    }

    private Node Remove(Node node, TKey key)
    {
        if (node == null) return null;

        int cmp = key.CompareTo(node.Key);
        if (cmp < 0)
            node.Left = Remove(node.Left, key);
        else if (cmp > 0)
            node.Right = Remove(node.Right, key);
        else
        {
            if (node.Left == null) return node.Right;
            if (node.Right == null) return node.Left;

            Node min = GetMin(node.Right);
            node.Key = min.Key;
            node.Value = min.Value;
            node.Right = Remove(node.Right, min.Key);
        }

        return Balance(node);
    }

    private Node GetMin(Node node)
    {
        while (node.Left != null) node = node.Left;
        return node;
    }

    private int Height(Node node) => node?.Height ?? 0;

    private int BalanceFactor(Node node) => Height(node.Right) - Height(node.Left);

    private void UpdateHeight(Node node)
    {
        node.Height = Math.Max(Height(node.Left), Height(node.Right)) + 1;
    }

    private Node RotateRight(Node y)
    {
        Node x = y.Left;
        y.Left = x.Right;
        x.Right = y;
        UpdateHeight(y);
        UpdateHeight(x);
        return x;
    }

    private Node RotateLeft(Node x)
    {
        Node y = x.Right;
        x.Right = y.Left;
        y.Left = x;
        UpdateHeight(x);
        UpdateHeight(y);
        return y;
    }

    private Node Balance(Node node)
    {
        UpdateHeight(node);
        int bf = BalanceFactor(node);
        if (bf == 2)
        {
            if (BalanceFactor(node.Right) < 0)
                node.Right = RotateRight(node.Right);
            return RotateLeft(node);
        }
        if (bf == -2)
        {
            if (BalanceFactor(node.Left) > 0)
                node.Left = RotateLeft(node.Left);
            return RotateRight(node);
        }
        return node;
    }

    private void InOrderTraversal(Node node, Action<Node> action)
    {
        if (node == null) return;
        InOrderTraversal(node.Left, action);
        action(node);
        InOrderTraversal(node.Right, action);
    }
}
