public class QuadraticProbingHashTable
{
    private int?[] table;
    private int size;
    private const int DELETED = int.MinValue;

    public QuadraticProbingHashTable(int size)
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
        int i = 0;
        while (table[(index + i * i) % size] is int val && val != DELETED)
        {
            i++;
        }
        table[(index + i * i) % size] = key;
    }

    public bool Search(int key)
    {
        int index = Hash(key);
        int i = 0;
        while (table[(index + i * i) % size] != null)
        {
            if (table[(index + i * i) % size] == key)
                return true;
            i++;
        }
        return false;
    }

    public void Delete(int key)
    {
        int index = Hash(key);
        int i = 0;
        while (table[(index + i * i) % size] != null)
        {
            if (table[(index + i * i) % size] == key)
            {
                table[(index + i * i) % size] = DELETED;
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