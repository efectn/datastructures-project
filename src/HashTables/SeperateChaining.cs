namespace datastructures_project.HashTables;

public class SeparateChainingHashTable
{
    private readonly LinkedList<int>[] table;
    private readonly int size;

    /*static void Main()
    {
        var hashTable = new SeparateChainingHashTable(5);
        hashTable.Insert(10);
        hashTable.Insert(20);
        hashTable.Insert(30);
        hashTable.Insert(41);
        Console.WriteLine("Hash Table with Separate Chaining:");
        hashTable.PrintTable(); 
        
        /*Console.WriteLine($"Search 20: {hashTable.Search(20)}"); 
        hashTable.Delete(20); Console.WriteLine($"Search 20 after deletion: {hashTable.Search(20)}"); 
        hashTable.PrintTable();
    }*/
    

    
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
