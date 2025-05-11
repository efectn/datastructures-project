using System;
using System.Collections;
using System.Collections.Generic;

namespace datastructures_project.HashTables
{
    enum NodeColor { Red, Black }

    class Node<TKey, TValue> where TKey : IComparable<TKey>
    {
        public TKey Key;
        public TValue Value;
        public NodeColor Color;
        public Node<TKey, TValue> Left, Right, Parent;

        public Node(TKey key, TValue value)
        {
            Key = key;
            Value = value;
            Color = NodeColor.Red;
        }
    }

    public class RedBlackTree<TKey, TValue> : IDictionary<TKey, TValue> where TKey : IComparable<TKey>
    {
        private Node<TKey, TValue> root;
        private int count = 0;

        public TValue this[TKey key]
        {
            get
            {
                var node = FindNode(root, key);
                if (node == null) throw new KeyNotFoundException();
                return node.Value;
            }
            set
            {
                var node = FindNode(root, key);
                if (node != null) node.Value = value;
                else AddInternal(key, value);
            }
        }

        public ICollection<TKey> Keys => new List<TKey>(InOrderKeys(root));
        public ICollection<TValue> Values => new List<TValue>(InOrderValues(root));
        public int Count => count;
        public bool IsReadOnly => false;

        public void Add(TKey key, TValue value)
        {
            if (ContainsKey(key))
                throw new ArgumentException("Key already exists.");

            var newNode = new Node<TKey, TValue>(key, value);
            root = BSTInsert(root, newNode);
            FixViolation(newNode);
            count++;
        }
        
        private void AddInternal(TKey key, TValue value)
        {
            var newNode = new Node<TKey, TValue>(key, value);
            root = BSTInsert(root, newNode);
            FixViolation(newNode);
            count++;
        }

        public bool Remove(TKey key)
        {
            var nodeToDelete = FindNode(root, key);
            if (nodeToDelete == null) return false;

            DeleteNode(nodeToDelete);
            count--;
            return true;
        }

        private void DeleteNode(Node<TKey, TValue> node)
        {
            Node<TKey, TValue> y = node;
            NodeColor yOriginalColor = y.Color;
            Node<TKey, TValue> x;
            Node<TKey, TValue> xParent;

            if (node.Left == null)
            {
                x = node.Right;
                xParent = node.Parent;
                Transplant(node, node.Right);
            }
            else if (node.Right == null)
            {
                x = node.Left;
                xParent = node.Parent;
                Transplant(node, node.Left);
            }
            else
            {
                y = Minimum(node.Right);
                yOriginalColor = y.Color;
                x = y.Right;
                xParent = y.Parent;

                if (y.Parent == node)
                {
                    if (x != null) x.Parent = y;
                    xParent = y;
                }
                else
                {
                    Transplant(y, y.Right);
                    y.Right = node.Right;
                    y.Right.Parent = y;
                }

                Transplant(node, y);
                y.Left = node.Left;
                y.Left.Parent = y;
                y.Color = node.Color;
            }

            if (yOriginalColor == NodeColor.Black)
                FixDelete(x, xParent);
        }

        private void Transplant(Node<TKey, TValue> u, Node<TKey, TValue> v)
        {
            if (u.Parent == null)
                root = v;
            else if (u == u.Parent.Left)
                u.Parent.Left = v;
            else
                u.Parent.Right = v;

            if (v != null)
                v.Parent = u.Parent;
        }

        private Node<TKey, TValue> Minimum(Node<TKey, TValue> node)
        {
            while (node.Left != null)
                node = node.Left;
            return node;
        }

        private void FixDelete(Node<TKey, TValue> x, Node<TKey, TValue> parent)
        {
            while (x != root && (x == null || x.Color == NodeColor.Black))
            {
                if (x == parent.Left)
                {
                    var w = parent.Right;
                    if (w.Color == NodeColor.Red)
                    {
                        w.Color = NodeColor.Black;
                        parent.Color = NodeColor.Red;
                        RotateLeft(parent);
                        w = parent.Right;
                    }

                    if ((w.Left == null || w.Left.Color == NodeColor.Black) &&
                        (w.Right == null || w.Right.Color == NodeColor.Black))
                    {
                        w.Color = NodeColor.Red;
                        x = parent;
                        parent = x.Parent;
                    }
                    else
                    {
                        if (w.Right == null || w.Right.Color == NodeColor.Black)
                        {
                            if (w.Left != null) w.Left.Color = NodeColor.Black;
                            w.Color = NodeColor.Red;
                            RotateRight(w);
                            w = parent.Right;
                        }

                        w.Color = parent.Color;
                        parent.Color = NodeColor.Black;
                        if (w.Right != null) w.Right.Color = NodeColor.Black;
                        RotateLeft(parent);
                        x = root;
                    }
                }
                else
                {
                    var w = parent.Left;
                    if (w.Color == NodeColor.Red)
                    {
                        w.Color = NodeColor.Black;
                        parent.Color = NodeColor.Red;
                        RotateRight(parent);
                        w = parent.Left;
                    }

                    if ((w.Right == null || w.Right.Color == NodeColor.Black) &&
                        (w.Left == null || w.Left.Color == NodeColor.Black))
                    {
                        w.Color = NodeColor.Red;
                        x = parent;
                        parent = x.Parent;
                    }
                    else
                    {
                        if (w.Left == null || w.Left.Color == NodeColor.Black)
                        {
                            if (w.Right != null) w.Right.Color = NodeColor.Black;
                            w.Color = NodeColor.Red;
                            RotateLeft(w);
                            w = parent.Left;
                        }

                        w.Color = parent.Color;
                        parent.Color = NodeColor.Black;
                        if (w.Left != null) w.Left.Color = NodeColor.Black;
                        RotateRight(parent);
                        x = root;
                    }
                }
            }

            if (x != null)
                x.Color = NodeColor.Black;
        }

        public bool ContainsKey(TKey key) => FindNode(root, key) != null;

        public bool TryGetValue(TKey key, out TValue value)
        {
            var node = FindNode(root, key);
            if (node != null)
            {
                value = node.Value;
                return true;
            }
            value = default;
            return false;
        }

        public void Add(KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);

        public void Clear()
        {
            root = null;
            count = 0;
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            var node = FindNode(root, item.Key);
            return node != null && EqualityComparer<TValue>.Default.Equals(node.Value, item.Value);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            foreach (var kvp in this)
                array[arrayIndex++] = kvp;
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            if (Contains(item))
                return Remove(item.Key);
            return false;
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach (var kvp in InOrderTraversal(root))
                yield return kvp;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private Node<TKey, TValue> FindNode(Node<TKey, TValue> node, TKey key)
        {
            while (node != null)
            {
                int cmp = key.CompareTo(node.Key);
                if (cmp < 0) node = node.Left;
                else if (cmp > 0) node = node.Right;
                else return node;
            }
            return null;
        }

        private IEnumerable<TKey> InOrderKeys(Node<TKey, TValue> node)
        {
            if (node == null) yield break;
            foreach (var key in InOrderKeys(node.Left)) yield return key;
            yield return node.Key;
            foreach (var key in InOrderKeys(node.Right)) yield return key;
        }

        private IEnumerable<TValue> InOrderValues(Node<TKey, TValue> node)
        {
            if (node == null) yield break;
            foreach (var value in InOrderValues(node.Left)) yield return value;
            yield return node.Value;
            foreach (var value in InOrderValues(node.Right)) yield return value;
        }

        private IEnumerable<KeyValuePair<TKey, TValue>> InOrderTraversal(Node<TKey, TValue> node)
        {
            if (node == null) yield break;
            foreach (var kvp in InOrderTraversal(node.Left)) yield return kvp;
            yield return new KeyValuePair<TKey, TValue>(node.Key, node.Value);
            foreach (var kvp in InOrderTraversal(node.Right)) yield return kvp;
        }

        private Node<TKey, TValue> BSTInsert(Node<TKey, TValue> root, Node<TKey, TValue> node)
        {
            if (root == null) return node;

            if (node.Key.CompareTo(root.Key) < 0)
            {
                root.Left = BSTInsert(root.Left, node);
                root.Left.Parent = root;
            }
            else if (node.Key.CompareTo(root.Key) > 0)
            {
                root.Right = BSTInsert(root.Right, node);
                root.Right.Parent = root;
            }
            else
            {
                root.Value = node.Value;
            }
            return root;
        }

        private void RotateLeft(Node<TKey, TValue> node)
        {
            var right = node.Right;
            node.Right = right.Left;
            if (right.Left != null) right.Left.Parent = node;
            right.Parent = node.Parent;

            if (node.Parent == null) root = right;
            else if (node == node.Parent.Left) node.Parent.Left = right;
            else node.Parent.Right = right;

            right.Left = node;
            node.Parent = right;
        }

        private void RotateRight(Node<TKey, TValue> node)
        {
            var left = node.Left;
            node.Left = left.Right;
            if (left.Right != null) left.Right.Parent = node;
            left.Parent = node.Parent;

            if (node.Parent == null) root = left;
            else if (node == node.Parent.Left) node.Parent.Left = left;
            else node.Parent.Right = left;

            left.Right = node;
            node.Parent = left;
        }

        private void FixViolation(Node<TKey, TValue> node)
        {
            while (node != root && node.Color == NodeColor.Red && node.Parent.Color == NodeColor.Red)
            {
                var parent = node.Parent;
                var grandParent = parent.Parent;

                if (parent == grandParent.Left)
                {
                    var uncle = grandParent.Right;
                    if (uncle != null && uncle.Color == NodeColor.Red)
                    {
                        grandParent.Color = NodeColor.Red;
                        parent.Color = NodeColor.Black;
                        uncle.Color = NodeColor.Black;
                        node = grandParent;
                    }
                    else
                    {
                        if (node == parent.Right)
                        {
                            RotateLeft(parent);
                            node = parent;
                        }
                        RotateRight(grandParent);
                        var temp = parent.Color;
                        parent.Color = grandParent.Color;
                        grandParent.Color = temp;
                        node = parent;
                    }
                }
                else
                {
                    var uncle = grandParent.Left;
                    if (uncle != null && uncle.Color == NodeColor.Red)
                    {
                        grandParent.Color = NodeColor.Red;
                        parent.Color = NodeColor.Black;
                        uncle.Color = NodeColor.Black;
                        node = grandParent;
                    }
                    else
                    {
                        if (node == parent.Left)
                        {
                            RotateRight(parent);
                            node = parent;
                        }
                        RotateLeft(grandParent);
                        var temp = parent.Color;
                        parent.Color = grandParent.Color;
                        grandParent.Color = temp;
                        node = parent;
                    }
                }
            }
            root.Color = NodeColor.Black;
        }
    }
}
