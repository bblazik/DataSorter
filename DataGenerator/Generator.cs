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
    class Generator
    {
        private const string OUTPUT_NAME = "TestData.txt";
        private const long FIXED_SIZE = 1024 * 1024 * 1024;
        private const long AVERAGE_LINE_SIZE = 18;
        private const long FIXED_RECORD_SIZE = sizeof(uint) + sizeof(char) * AVERAGE_LINE_SIZE;

        public int _index { get; set; } = 0;
        object lockObject = new object();
        private object _lock = new object();

        static void Main(string[] args)
        {
            if(args.Length == 0)
            {
                Console.WriteLine("Give size argument in megabytes eg.: 5");
                Console.ReadLine();
                return;
            }

            try 
            {
                uint size = uint.Parse(args[0]);
                long chunksize2 = (long)Math.Floor(Math.Log10(size) * 100);
                Console.WriteLine("Generating data it can take a while.");
                var generator = new Generator();
                Benchmark<uint>.Check(generator.GenerateDataInChunksAndMerge, size);
                Console.WriteLine("Succesfull operation. File was Generated. Press any key to close.");
                Console.ReadLine();
            } 
            catch(Exception e)
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
        /// <param name="givenSize">Give size in megabytes</param>
        /// <output>Will generate a file of test data with accurcy to about one megabyte</output>

        public void GenerateDataInChunksAndMerge(uint givenSize)
        {
            long numberOfRecordsToGenerate = (givenSize * FIXED_SIZE) / FIXED_RECORD_SIZE;
            long chunkSize = (numberOfRecordsToGenerate/64);
            long numberOfChunks = numberOfRecordsToGenerate / chunkSize;

            Parallel.For(0, numberOfChunks, new ParallelOptions { MaxDegreeOfParallelism = 4 }, chunkIndex =>
            //for (int chunkIndex = 0; chunkIndex < numberOfChunks; chunkIndex++) // We can't do faster than I/O anyways.
            {    
                var startIndex = chunkIndex * chunkSize;
                var endIndex = Math.Min(startIndex + chunkSize, numberOfRecordsToGenerate);
                using (MemoryStream ms = new MemoryStream())
                using (StreamWriter sw = new StreamWriter(ms))
                {
                    Parallel.For(startIndex, endIndex, i => // Optimize time of object creation.
                    {
                        var record = CreateRecord();
                        
                        lock (lockObject)
                        {
                            sw.Write(record.ToString());
                        }
                    });

                    // Write the memory stream to a file
                    using (FileStream fs = new FileStream(OUTPUT_NAME + chunkIndex, FileMode.Create, FileAccess.Write))
                    {
                        ms.WriteTo(fs);
                    }
                }
            });
            MergeFiles(OUTPUT_NAME, numberOfChunks, chunkFormat: OUTPUT_NAME + "{0}");
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
            using (FileStream mergedFs = new FileStream(mergedOutputName, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true))
            using (StreamWriter mergedSw = new StreamWriter(mergedFs))
            {
                for (long i = 0; i < numberOfChunks; i++)
                {
                    string chunkOutputName = string.Format(chunkFormat, i);
                    using (FileStream fs = new FileStream(chunkOutputName, FileMode.Open, FileAccess.Read, FileShare.None, bufferSize: 4096, useAsync: true))
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        while (!sr.EndOfStream)
                        {
                            string line = sr.ReadLine();
                            mergedSw.WriteLine(line);
                        }
                    }
                    File.Delete(chunkOutputName);
                }
            }
        }
    }
}
