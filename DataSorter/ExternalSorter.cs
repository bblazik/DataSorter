using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DataGenerator;

namespace DataSorter
{
    public class ExternalSorter
    {
        private const int AVERAGE_LINE_LENGTH = 19;
        private const long AVERAGE_RECORD_SIZE_IN_BYTES = sizeof(uint) + sizeof(char) * AVERAGE_LINE_LENGTH;
        private static long _chunkSizeInBytes;
        private static long _numberOfRecordToCreateInChunk;
        private static string _inputFile = "TestData.txt";
        private static string _outputFile = "output.txt";

        static void Main(string[] args)
        {
            try
            {
                if (args.Length == 2)
                {
                    _inputFile = args[0];
                    _outputFile = args[1];
                }
                Console.WriteLine($"Sort process started. InputFile: {_inputFile}, OutputFile: {_outputFile}");
                Run();
                Console.WriteLine($"Succesful finish. Output saved to: {_outputFile}. Press any key to close.");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine("Something went wrong.");
                Console.WriteLine(e);
                Console.ReadLine();
            }
        }

        public static void Run()
        {
            var sizeOfFileInBytes = new FileInfo(_inputFile).Length;
            var sizeOfFileInGB = (int)Math.Round((double)sizeOfFileInBytes / (1024 * 1024 * 1024));
            var numberOfChunks = sizeOfFileInGB + 2;
            _chunkSizeInBytes = sizeOfFileInBytes / numberOfChunks;
            _numberOfRecordToCreateInChunk = _chunkSizeInBytes / AVERAGE_RECORD_SIZE_IN_BYTES; 

            List<string> chunkFiles = Benchmark.Check(DivideDataIntoChunks, _inputFile);
            Benchmark.Check(SortAndWriteChunksToFile, chunkFiles);
            Benchmark.Check(MergeChunksIntoFile, chunkFiles);
        }

        private static List<string> DivideDataIntoChunks(string inputFile)
        {
            var chunkFiles = new List<string>();
            var chunkNumber = 0;

            using (var input = new FileStream(inputFile, FileMode.Open, FileAccess.Read))
            using (var reader = new StreamReader(input))
            {
                string line;
                var lines = new List<string>();
                while ((line = reader.ReadLine()) != null)
                {
                    lines.Add(line);

                    // If the current chunk has reached the specified size, write it to a file and clear memory.
                    if (lines.Count >= _numberOfRecordToCreateInChunk)
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

        private static void SortAndWriteChunksToFile(List<string> chunkFiles)
        {
            Parallel.ForEach(chunkFiles,new ParallelOptions() {MaxDegreeOfParallelism = 4}, chunkFile => 
            {
                var records = TryParseRecordsSafe(chunkFile);
                records.Sort();
                WriteRecordsToChunkFile(chunkFile, records);
            });
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
                    sw.Write(record.ToString());
                }
            }
        }

        private static void MergeChunksIntoFile(List<string> chunkFiles)
        {
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
            using (var output = new FileStream(_outputFile, FileMode.Create, FileAccess.Write))
            using (var writer = new StreamWriter(output))
            {
                while (heap.Size > 0)
                {
                    // Remove the smallest record from the heap
                    var record = heap.ExtractMin();
                    writer.Write(record.ToString());

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
            // Clean up chunks
            foreach (var chunkFile in chunkFiles)
            {
                File.Delete(chunkFile);
            }
        }
    }
}
