public class DoubleHashingHashTable
{
    private int?[] table;
    private int size;
    private const int DELETED = int.MinValue;

    public DoubleHashingHashTable(int size)
    {
        this.size = size;
        table = new int?[size];
    }

    private int Hash1(int key)
    {
        return key % size;
    }

    private int Hash2(int key)
    {
        return 1 + (key % (size - 1)); // must be non-zero
    }

    public void Insert(int key)
    {
        int index = Hash1(key);
        int step = Hash2(key);
        int i = 0;
        while (table[(index + i * step) % size] is int val && val != DELETED)
        {
            i++;
        }
        table[(index + i * step) % size] = key;
    }

    public bool Search(int key)
    {
        int index = Hash1(key);
        int step = Hash2(key);
        int i = 0;
        while (table[(index + i * step) % size] != null)
        {
            if (table[(index + i * step) % size] == key)
                return true;
            i++;
        }
        return false;
    }

    public void Delete(int key)
    {
        int index = Hash1(key);
        int step = Hash2(key);
        int i = 0;
        while (table[(index + i * step) % size] != null)
        {
            if (table[(index + i * step) % size] == key)
            {
                table[(index + i * step) % size] = DELETED;
                return;
            }
            i++;
        }
    }

    public void PrintTable()
    {
        for (int i = 0; i < size; i++)
        {
            Console.WriteLine($"[{i}]: {(table[i] == null ? "null" : table[i].ToString())}");
        }
    }
}