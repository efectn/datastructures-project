namespace datastructures_project.HashTables;

public class LinearProbingHashTable
{
    /*static void Main()
    {
        var hashTable = new LinearProbingHashTable(10);

        hashTable.Insert(10);
        hashTable.Insert(20);
        hashTable.Insert(30);
        hashTable.Insert(41);

        Console.WriteLine("Hash Table with Linear Probing:");
        hashTable.PrintTable();
        /*
        Console.WriteLine($"Search 20: {hashTable.Search(20)}");
        Console.WriteLine("Deleting 20...");
        hashTable.Delete(20);
        Console.WriteLine($"Search 20 after deletion: {hashTable.Search(20)}");
        
        hashTable.PrintTable();
    }*/
    
    private int?[] table;
    private int size;
    private const int DELETED = int.MinValue; // Special marker for deleted slots

    public LinearProbingHashTable(int size)
    {
        this.size = size;
        table = new int?[size];
    }

    private int Hash(int key)
    {
        return key % size;
    }

    public void Insert(int key)
    {
        int index = Hash(key);
        int start = index;

        while (table[index] != null && table[index] != DELETED)
        {
            index = (index + 1) % size;
            if (index == start)
            {
                Console.WriteLine("Hash table is full!");
                return;
            }
        }

        table[index] = key;
    }

    public bool Search(int key)
    {
        int index = Hash(key);
        int start = index;

        while (table[index] != null)
        {
            if (table[index] != DELETED && table[index] == key)
                return true;

            index = (index + 1) % size;
            if (index == start)
                break;
        }

        return false;
    }

    public void Delete(int key)
    {
        int index = Hash(key);
        int start = index;

        while (table[index] != null)
        {
            if (table[index] != DELETED && table[index] == key)
            {
                table[index] = DELETED;
                return;
            }

            index = (index + 1) % size;
            if (index == start)
                break;
        }

        Console.WriteLine("Key not found to delete.");
    }

    public void PrintTable()
    {
        for (int i = 0; i < size; i++)
        {
            Console.Write($"[{i}]: ");
            if (table[i] == null)
                Console.WriteLine("Empty");
            else if (table[i] == DELETED)
                Console.WriteLine("Deleted");
            else
                Console.WriteLine(table[i]);
        }
        
        
    }
}