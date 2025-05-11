namespace datastructures_project.HashTables;

public interface ILinearQuadraticDoubleHashing<TKey, TValue> : IDictionary<TKey, TValue>
{
    bool IsInCollide(TKey key);
    List<int> GetTombstones();
    IEnumerator<KeyValuePair<int, KeyValuePair<TKey, TValue>>> GetEnumeratorWithIndex();
}

public interface ISeparateChaining<TKey, TValue> : IDictionary<TKey, TValue>
{
    LinkedList<KeyValuePair<TKey, TValue>>[] GetBuckets();
}