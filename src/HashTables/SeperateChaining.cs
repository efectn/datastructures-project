public class SeparateChainingHashTable
{
    private readonly LinkedList<int>[] table;
    private readonly int size;

    public SeparateChainingHashTable(int size)
    {
        this.size = size;
        table = new LinkedList<int>[size];

        for (int i = 0; i < size; i++)
        {
            table[i] = new LinkedList<int>();
        }
    }

    private int Hash(int key)
    {
        return key % size;
    }

    public void Insert(int key)
    {
        int index = Hash(key);
        if (!table[index].Contains(key))
        {
            table[index].AddLast(key);
        }
    }

    public bool Search(int key)
    {
        int index = Hash(key);
        return table[index].Contains(key);
    }

    public void Delete(int key)
    {
        int index = Hash(key);
        table[index].Remove(key);
    }

    public void PrintTable()
    {
        for (int i = 0; i < size; i++)
        {
            Console.Write($"[{i}]: ");
            foreach (var item in table[i])
            {
                Console.Write($"{item} -> ");
            }
            Console.WriteLine("null");
        }
    }
}