using System;
using System.IO;
using System.Threading.Tasks;
using DataSorter;

namespace DataGenerator
{
    /// <summary>
    /// Class is used to generate sample test dataset.
    /// Usage: DataGenerator <size in GB> eg.: DataGenerator.exe 10
    /// </summary>
    public class Generator
    {
        private const long BYTES_IN_1GB = 1024 * 1024 * 1024;
        private const long AVERAGE_LINE_LENGTH = 19;
        private const long AVERAGE_RECORD_SIZE_IN_BYTES = sizeof(uint) + sizeof(char) * AVERAGE_LINE_LENGTH;
        private static string _outputName = "TestData.txt";
        private static object _lockObject = new object();
        private static object _lock = new object();
        private int _index = 0;

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Give size argument in megabytes eg.: 5");
                Console.ReadLine();
                return;
            }

            try
            {
                var size = uint.Parse(args[0]);
                if (args.Length > 1)
                {
                    _outputName = args[1];
                }
                Console.WriteLine("Generating data it can take a while.");
                var generator = new Generator();
                Benchmark.Check(generator.GenerateDataInChunksAndMerge, size);
                Console.WriteLine("Succesfull operation. File was Generated. Press any key to close.");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine("Something went wrong.");
                Console.WriteLine(e);
                Console.ReadLine();
            }
        }

        /// <summary>
        /// This method is mmemory safe until you set chunk size lower than available memory.
        /// Performance is linked with good parametrization.
        /// </summary>
        /// <param name="givenSize">Give size in gigabytes</param>
        /// <output>Will generate a file of test data with accurcy to about one megabyte</output>

        public void GenerateDataInChunksAndMerge(uint givenSizeInGB)
        {
            long numberOfChunks = givenSizeInGB + 1;
            long chunkSizeInBytes = (givenSizeInGB * BYTES_IN_1GB) / numberOfChunks;
            long numberOfRecordToCreateInChunk = chunkSizeInBytes / AVERAGE_RECORD_SIZE_IN_BYTES;

            Parallel.For(0, numberOfChunks, new ParallelOptions() { MaxDegreeOfParallelism = 4 }, chunkIndex =>
             {
                 using (var ms = new MemoryStream())
                 using (var sw = new StreamWriter(ms))
                 {
                     Parallel.For(0, numberOfRecordToCreateInChunk, i => // Optimize time of object creation.
                     {
                         var record = CreateRecord();

                         lock (_lockObject)
                         {
                             sw.Write(record.ToString());
                         }
                     });

                    // Write the memory stream to a file
                    using (var fs = new FileStream(_outputName + chunkIndex, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 4096))
                    {
                         ms.WriteTo(fs);
                    }
                 }
             });
            MergeFiles(_outputName, numberOfChunks, chunkFormat: _outputName + "{0}");
        }

        private Record CreateRecord()
        {
            lock (_lock)
            {
                long milliseconds = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                uint pseudoRandom = (uint)(milliseconds % (_index + 13));

                if (_index >= Dataset.SPLITTED_SENTENCES.Count)
                {
                    _index = 0;
                }

                var newRecord = new Record() { Name = Dataset.SPLITTED_SENTENCES[_index++], Number = pseudoRandom };
                return newRecord;
            }
        }

        private void MergeFiles(string mergedOutputName, long numberOfChunks, string chunkFormat)
        {
            using (var mergedFs = new FileStream(mergedOutputName, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 65536, useAsync: true))
            using (var mergedSw = new StreamWriter(mergedFs))
            {
                for (long i = 0; i < numberOfChunks; i++)
                {
                    var chunkOutputName = string.Format(chunkFormat, i);
                    using (var fs = new FileStream(chunkOutputName, FileMode.Open, FileAccess.Read, FileShare.None, bufferSize: 65536, useAsync: true))
                    using (var sr = new StreamReader(fs))
                    {
                        while (!sr.EndOfStream)
                        {
                            var line = sr.ReadLine();
                            mergedSw.WriteLine(line);
                        }
                    }
                    File.Delete(chunkOutputName);
                }
            }
        }
    }
}
