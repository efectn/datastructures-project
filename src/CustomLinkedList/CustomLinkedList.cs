using System.Collections;
using System.Collections.Generic;

public class Node<T>
{
    public T value;
    public Node<T> next;

    public Node(T value)
    {
        this.value = value;
        this.next = null;
    }
}


public class CustomLinkedList<T> : IEnumerable<T>
{
    public Node<T> head;

    public void AddLast(T value)
    {
        var newNode = new Node<T>(value);
        if (head == null)
        {
            head = newNode;
        }
        else
        {
            var current = head;
            while (current.next != null)
                current = current.next;
            current.next = newNode;
        }
    }

    public bool Remove(Node<T> nodeToRemove)
    {
        if (head == null) return false;

        if (head == nodeToRemove)
        {
            head = head.next;
            return true;
        }

        var current = head;
        while (current.next != null && current.next != nodeToRemove)
        {
            current = current.next;
        }

        if (current.next == nodeToRemove)
        {
            current.next = current.next.next;
            return true;
        }

        return false;
    }

    public IEnumerator<T> GetEnumerator()
    {
        var current = head;
        while (current != null)
        {
            yield return current.value;
            current = current.next;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}