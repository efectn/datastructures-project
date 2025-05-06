namespace datastructures_project.HashTables;

public class Comparison
{ 
    // This class demonstrates how to use and compare four different hash table techniques:
// - Linear Probing
// - Separate Chaining
// - Quadratic Probing
// - Double Hashing
//
// To run this code, call: Comparison.Run(); from your Program.cs file inside Main().
// Example:
//     static void Main() {
//         Comparison.Run();
//     }
    public static void Run()
    {
        Console.WriteLine("Linear Probing:");
        var linear = new LinearProbingHashTable(10);
        linear.Insert(10); linear.Insert(20); linear.Insert(30); linear.Insert(41);
        linear.PrintTable();

        Console.WriteLine("\nSeparate Chaining:");
        var chaining = new SeparateChainingHashTable(5);
        chaining.Insert(10); chaining.Insert(20); chaining.Insert(30); chaining.Insert(41);
        chaining.PrintTable();

        Console.WriteLine("\nQuadratic Probing:");
        var quadratic = new QuadraticProbingHashTable(10);
        quadratic.Insert(10); quadratic.Insert(20); quadratic.Insert(30); quadratic.Insert(41);
        quadratic.PrintTable();

        Console.WriteLine("\nDouble Hashing:");
        var doubleHash = new DoubleHashingHashTable(10);
        doubleHash.Insert(10); doubleHash.Insert(20); doubleHash.Insert(30); doubleHash.Insert(41);
        doubleHash.PrintTable();
    }
}