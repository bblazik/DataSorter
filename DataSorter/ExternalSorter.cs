using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HPCsharp;
using DStruct;
using DataGenerator;

namespace DataSorter
{
    class ExternalSorter
    {
        private const int AVERAGE_LINE_SIZE = 18;
        private const int CHUNK_SIZE = 1024 * 1024 * 4; // 4 MB
        private const int ESTIMATED_LINES_IN_CHUNK = CHUNK_SIZE / AVERAGE_LINE_SIZE;

        static void Main(string[] args)
        {
            string inputFile = "TestData.txt";
            string outputFile = "output.txt";

            // Load and divide the file into chunks

            List<string> chunkFiles = Banchmark<string, int, List<string>>.Check(DivideDataIntoChunks,inputFile, CHUNK_SIZE);

            // Sort and write each chunk to a separate file
            Banchmark<List<string>, int, object>.Check(SortAndWriteChunksToFile, chunkFiles, CHUNK_SIZE);

            // Merge the sorted chunks into a single file
            Banchmark<List<string>, int, object>.Check(MergeChunksIntoFile, chunkFiles, outputFile);

            Console.WriteLine($"Succesful finish. Output saved to: {outputFile}. Press any key to close.");
            Console.ReadLine();
        }

        private static List<string> DivideDataIntoChunks(string inputFile, int chunkSize)
        {
            List<string> chunkFiles = new List<string>();
            int chunkNumber = 0;

            using (var input = new FileStream(inputFile, FileMode.Open, FileAccess.Read))
            using (var reader = new StreamReader(input))
            {
                string line;
                List<string> lines = new List<string>();
                while ((line = reader.ReadLine()) != null)
                {
                    lines.Add(line);

                    // If the current chunk has reached the specified size, write it to a file and clear memory.
                    if (lines.Count >= ESTIMATED_LINES_IN_CHUNK)
                    {
                        chunkFiles.Add(WriteChunkToFile(lines, chunkNumber));
                        chunkNumber++;
                        lines.Clear(); 
                    }
                }

                // Write the final chunk to a file
                if (lines.Count > 0)
                {
                    chunkFiles.Add(WriteChunkToFile(lines, chunkNumber));
                }
            }

            return chunkFiles;
        }

        private static string WriteChunkToFile(List<string> lines, int chunkNumber)
        {
            string chunkFile = $"chunk_{chunkNumber}.txt";

            using (var output = new FileStream(chunkFile, FileMode.Create, FileAccess.Write))
            using (var writer = new StreamWriter(output))
            {
                foreach (string line in lines)
                {
                    writer.WriteLine(line);
                }
            }

            return chunkFile;
        }

        private static void SortAndWriteChunksToFile(List<string> chunkFiles, int chunkSize)
        {
            //Parallel.ForEach(chunkFiles, chunkFile => //todo with max limit of paralelism 
            foreach(var chunkFile in chunkFiles)
            {
                //var records = TryParseRecordsSafe(chunkFile);
                var records = File.ReadAllLines(chunkFile).Select(r => Record.ParseRecord(r)).ToList();
                records.Sort();
                WriteRecordsToChunkFile(chunkFile, records);
            }//);
        }

        private static List<Record> TryParseRecordsSafe(string chunkFile)
        {
            var records = new List<Record>();
            var lines = File.ReadAllLines(chunkFile);
            foreach (var line in lines)
            {
                try
                {
                    records.Add(Record.ParseRecord(line));
                }
                catch (Exception e)
                {
                    Console.WriteLine("one of the record was not parsed correctly");
                    continue; // Can be any approach 
                }
            }

            return records;
        }

        private static void WriteRecordsToChunkFile(string chunkFile, List<Record> records)
        {
            using (StreamWriter sw = new StreamWriter(chunkFile))
            {
                foreach (Record record in records)
                {
                    sw.WriteLine(record.ToString());
                }
            }
        }

        private static void MergeChunksIntoFile(List<string> chunkFiles, string outputFile)
        {
            // Create a list of StreamReaders, one for each chunk file
            List<StreamReader> chunkReaders = chunkFiles.Select(file => new StreamReader(file)).ToList();

            // Initialize a min heap with the first line from each chunk
            var heap = new MinHeap<Record>(chunkReaders.Count);
            for (int i = 0; i < chunkReaders.Count; i++)
            {
                var line = chunkReaders[i].ReadLine();
                if (line != null)
                {
                    var record = Record.ParseRecord(line);
                    record.ChunkIndex = i;
                    heap.Add(record);
                }
            }

            // Write the sorted records to the output file
            using (var output = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
            using (var writer = new StreamWriter(output))
            {
                while (heap.Size > 0)
                {
                    // Remove the smallest record from the heap
                    var record = heap.ExtractMin();
                    writer.WriteLine(record.ToString());

                    // Add the next line from the chunk that the record was read from to the heap
                    var chunkIndex = record.ChunkIndex;
                    var line = chunkReaders[chunkIndex].ReadLine();
                    if (line != null)
                    {
                        record = Record.ParseRecord(line);
                        record.ChunkIndex = chunkIndex;
                        heap.Add(record);
                    }
                }
            }
            // Close all the chunk readers
            foreach (var reader in chunkReaders)
            {
                reader.Close();
            }

            foreach (var chunkFile in chunkFiles)
            {
                File.Delete(chunkFile);
            }
        }
    }
}
