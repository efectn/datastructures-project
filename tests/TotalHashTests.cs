using System;
using System.Collections.Generic;
using datastructures_project.HashTables;
using Xunit;

namespace datastructures_project.Tests
{
    public class HashTableTests
    {
        private IDictionary<string, int> CreateTable(string type)
        {
            return type switch
            {
                "Linear" => new LinearProbingHashTable<string, int>(),
                "Quadratic" => new QuadraticProbingHashTable<string, int>(),
                "Double" => new DoubleHashingHashTable<string, int>(),
                "Separate" => new SeparateChainingHashTable<string, int>(),
                "AVL" => new AVLTreeDictionary<string, int>(), // AVL Tree'yi ekledik
                _ => throw new ArgumentException("Invalid type")
            };
        }

        [Theory]
        [InlineData("Linear")]
        [InlineData("Quadratic")]
        [InlineData("Double")]
        [InlineData("Separate")]
        [InlineData("AVL")]
        public void Add_And_Retrieve_Works(string type)
        {
            var table = CreateTable(type);
            table.Add("one", 1);
            table.Add("two", 2);

            Assert.True(table.ContainsKey("one"));
            Assert.Equal(2, table["two"]);
        }

        [Theory]
        [InlineData("Linear")]
        [InlineData("Quadratic")]
        [InlineData("Double")]
        [InlineData("Separate")]
        [InlineData("AVL")]
        public void Add_DuplicateKey_Throws(string type)
        {
            var table = CreateTable(type);
            table.Add("dup", 42);
            Assert.Throws<ArgumentException>(() => table.Add("dup", 99));
        }

        [Theory]
        [InlineData("Linear")]
        [InlineData("Quadratic")]
        [InlineData("Double")]
        [InlineData("Separate")]
        [InlineData("AVL")]
        public void Indexer_Set_Works(string type)
        {
            var table = CreateTable(type);
            table["key"] = 123;
            Assert.Equal(123, table["key"]);
        }

        [Theory]
        [InlineData("Linear")]
        [InlineData("Quadratic")]
        [InlineData("Double")]
        [InlineData("Separate")]
        [InlineData("AVL")]
        public void Remove_Works(string type)
        {
            var table = CreateTable(type);
            table.Add("k", 1);
            Assert.True(table.Remove("k"));
            Assert.False(table.ContainsKey("k"));
        }

        [Theory]
        [InlineData("Linear")]
        [InlineData("Quadratic")]
        [InlineData("Double")]
        [InlineData("Separate")]
        [InlineData("AVL")]
        public void TryGetValue_Returns_Expected_Results(string type)
        {
            var table = CreateTable(type);
            table.Add("found", 10);
            Assert.True(table.TryGetValue("found", out var val));
            Assert.Equal(10, val);

            Assert.False(table.TryGetValue("missing", out _));
        }

        [Theory]
        [InlineData("Linear")]
        [InlineData("Quadratic")]
        [InlineData("Double")]
        [InlineData("Separate")]
        [InlineData("AVL")]
        public void Keys_And_Values_Work(string type)
        {
            var table = CreateTable(type);
            table.Add("a", 1);
            table.Add("b", 2);

            Assert.Contains("a", table.Keys);
            Assert.Contains(2, table.Values);
        }

        [Theory]
        [InlineData("Linear")]
        [InlineData("Quadratic")]
        [InlineData("Double")]
        [InlineData("Separate")]
        [InlineData("AVL")]
        public void Clear_Removes_All(string type)
        {
            var table = CreateTable(type);
            table.Add("x", 7);
            table.Add("y", 8);
            table.Clear();

            Assert.Empty(table);
        }

        [Theory]
        [InlineData("Linear")]
        [InlineData("Quadratic")]
        [InlineData("Double")]
        [InlineData("Separate")]
        [InlineData("AVL")]
        public void Enumerator_Works(string type)
        {
            var table = CreateTable(type);
            table.Add("a", 1);
            table.Add("b", 2);

            var set = new HashSet<string>();
            foreach (var kv in table)
            {
                set.Add(kv.Key);
            }

            Assert.Contains("a", set);
            Assert.Contains("b", set);
        }
        
        [Theory]
        [InlineData("Linear")]
        [InlineData("Quadratic")]
        [InlineData("Double")]
        [InlineData("Separate")]
        [InlineData("AVL")]
        public void Remove_Does_Not_Break_Search_Chain(string type)
        {
            var table = CreateTable(type);
            table.Add("A", 1);
            table.Add("B", 2);
            table.Add("C", 3);

            Assert.True(table.Remove("A")); // Zincirin başından biri silinir

            Assert.True(table.ContainsKey("B")); // Zincir bozulmamalı
            Assert.Equal(2, table["B"]);
            Assert.True(table.Remove("B"));
            Assert.True(table.ContainsKey("C"));
            Assert.Equal(3, table["C"]);
        }
        
        [Theory]
        [InlineData("Linear")]
        [InlineData("Quadratic")]
        [InlineData("Double")]
        [InlineData("Separate")]
        public void Handles_Collisions_Correctly(string type)
        {
            var table = CreateTable(type);
            var keys = new[] { "Aa", "BB" };

            table.Add(keys[0], 100);
            table.Add(keys[1], 200);

            Assert.Equal(100, table[keys[0]]);
            Assert.Equal(200, table[keys[1]]);
        }
        
        [Theory]
        //[InlineData("Linear")]
        //[InlineData("Quadratic")]
        [InlineData("Double")]
        [InlineData("Separate")]
        [InlineData("AVL")]
        public void Can_Handle_Many_Entries(string type)
        {
            var table = CreateTable(type);
            int count = 1000;
            for (int i = 0; i < count; i++)
                table[$"key{i}"] = i;

            for (int i = 0; i < count; i++)
                Assert.Equal(i, table[$"key{i}"]);

            Assert.Equal(count, table.Count);
        }
        
        [Theory]
        [InlineData("Linear")]
        [InlineData("Quadratic")]
        [InlineData("Double")]
        [InlineData("Separate")]
        [InlineData("AVL")]
        public void Indexer_Updates_Existing_Key(string type)
        {
            var table = CreateTable(type);
            table["x"] = 1;
            table["x"] = 2;

            Assert.Equal(2, table["x"]);
        }
    
        [Theory]
        [InlineData("Linear")]
        [InlineData("Quadratic")]
        [InlineData("Double")]
        [InlineData("Separate")]
        [InlineData("AVL")]
        public void Remove_NonExistent_Returns_False(string type)
        {
            var table = CreateTable(type);
            Assert.False(table.Remove("ghost"));
        }

        [Theory]
        [InlineData("Linear")]
        [InlineData("Quadratic")]
        [InlineData("Double")]
        [InlineData("Separate")]
        [InlineData("AVL")]
        public void ContainsKey_Consistent(string type)
        {
            var table = CreateTable(type);
            table.Add("ping", 123);
            Assert.True(table.ContainsKey("ping"));
            Assert.True(table.ContainsKey("ping"));
        }
        
        [Theory]
        [InlineData("Linear")]
        [InlineData("Quadratic")]
        [InlineData("Double")]
        [InlineData("Separate")]
        [InlineData("AVL")]
        public void Handles_Empty_And_Special_Keys(string type)
        {
            var table = CreateTable(type);
            table[""] = 0;
            table["!@#$%^&*()"] = 42;

            Assert.Equal(0, table[""]);
            Assert.Equal(42, table["!@#$%^&*()"]);
        }
    }
}
