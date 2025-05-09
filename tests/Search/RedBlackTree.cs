namespace datastructures_project.Tests.Search;

using System;

enum NodeColor { Red, Black }

class Node
{
    public int Value;
    public NodeColor Color;
    public Node Left, Right, Parent;

    public Node(int value)
    {
        Value = value;
        Color = NodeColor.Red;
    }
}

class RedBlackTree
{
    private Node root;

    public void Insert(int value)
    {
        Node newNode = new Node(value);
        root = BSTInsert(root, newNode);
        FixViolation(newNode);
    }

    private Node BSTInsert(Node root, Node node)
    {
        if (root == null)
            return node;

        if (node.Value < root.Value)
        {
            root.Left = BSTInsert(root.Left, node);
            root.Left.Parent = root;
        }
        else if (node.Value > root.Value)
        {
            root.Right = BSTInsert(root.Right, node);
            root.Right.Parent = root;
        }
        return root;
    }

    private void RotateLeft(Node node)
    {
        Node rightChild = node.Right;
        node.Right = rightChild.Left;

        if (rightChild.Left != null)
            rightChild.Left.Parent = node;

        rightChild.Parent = node.Parent;

        if (node.Parent == null)
            root = rightChild;
        else if (node == node.Parent.Left)
            node.Parent.Left = rightChild;
        else
            node.Parent.Right = rightChild;

        rightChild.Left = node;
        node.Parent = rightChild;
    }

    private void RotateRight(Node node)
    {
        Node leftChild = node.Left;
        node.Left = leftChild.Right;

        if (leftChild.Right != null)
            leftChild.Right.Parent = node;

        leftChild.Parent = node.Parent;

        if (node.Parent == null)
            root = leftChild;
        else if (node == node.Parent.Left)
            node.Parent.Left = leftChild;
        else
            node.Parent.Right = leftChild;

        leftChild.Right = node;
        node.Parent = leftChild;
    }

    private void FixViolation(Node node)
    {
        Node parent = null, grandParent = null;

        while (node != root && node.Color == NodeColor.Red && node.Parent.Color == NodeColor.Red)
        {
            parent = node.Parent;
            grandParent = parent.Parent;

            if (parent == grandParent.Left)
            {
                Node uncle = grandParent.Right;

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
                        parent = node.Parent;
                    }

                    RotateRight(grandParent);
                    NodeColor tempColor = parent.Color;
                    parent.Color = grandParent.Color;
                    grandParent.Color = tempColor;
                    node = parent;
                }
            }
            else
            {
                Node uncle = grandParent.Left;

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
                        parent = node.Parent;
                    }

                    RotateLeft(grandParent);
                    NodeColor tempColor = parent.Color;
                    parent.Color = grandParent.Color;
                    grandParent.Color = tempColor;
                    node = parent;
                }
            }
        }

        root.Color = NodeColor.Black;
    }

    public void InOrderTraversal()
    {
        InOrderHelper(root);
        Console.WriteLine();
    }

    private void InOrderHelper(Node node)
    {
        if (node == null) return;

        InOrderHelper(node.Left);
        Console.Write($"{node.Value}({node.Color}) ");
        InOrderHelper(node.Right);
    }
}