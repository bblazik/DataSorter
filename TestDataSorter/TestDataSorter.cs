using System;
using Xunit;
using DataSorter;
using System.IO;
using System.Collections.Generic;
using Record = DataSorter.Record;
using System.Linq;

namespace TestDataSorter
{
    public class TestDataSorter
    {
        private static readonly string inputFile = "TestData.txt";
        private static readonly string outputFile = "output.txt";
        
        [Fact]
        public void CheckIfSortsFileWithoutAnyIssues()
        {
            if (File.Exists(inputFile))
            {
                ExternalSorter.Run(); // Invisible brackets of DoesNotThrow :)
            }
        }

        [Fact]
        public void CheckIfFileIsSortedCorrectly()
        {
            if (File.Exists(outputFile))
            {
                var listOfRecords = ReadFirstNRowsFromFile(outputFile, 1000);
                Assert.True(listOfRecords.SequenceEqual(listOfRecords.OrderBy(x => x)));
            }
        }

        private static List<Record> ReadFirstNRowsFromFile(string filePath, int n)
        {
            var firstNRows = new List<Record>();
            try
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    for (int i = 0; i < n; i++)
                    {
                        if (!sr.EndOfStream)
                        {
                            firstNRows.Add(Record.ParseRecord(sr.ReadLine()));
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: " + e.Message);
            }
            return firstNRows;
        }
    }
}
