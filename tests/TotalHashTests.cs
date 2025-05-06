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
                _ => throw new ArgumentException("Invalid type")
            };
        }

        [Theory]
        [InlineData("Linear")]
        [InlineData("Quadratic")]
        [InlineData("Double")]
        [InlineData("Separate")]
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
    }
}