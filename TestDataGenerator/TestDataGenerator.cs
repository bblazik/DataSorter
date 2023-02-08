using System.IO;
using Xunit;
using DataGenerator;
using System.Text.RegularExpressions;

namespace TestDataGenerator
{
    public class TestDataGenerator
    {
        private static string OUTPUT_NAME = "TestData.txt";

        Generator _generator = new Generator();

        [Fact]
        public void CheckIfFileIsCreatedAndWithCorrectSize()
        {
            var timeSpan = Benchmark.Check(_generator.GenerateDataInChunksAndMerge, 1u);

            Assert.True(timeSpan.TotalSeconds < 180);
            Assert.True(File.Exists(OUTPUT_NAME));

            var createdFileSizeInMegaBytes = new FileInfo(OUTPUT_NAME).Length / (1024 * 1024);
            Assert.True(1024 * 0.9 < createdFileSizeInMegaBytes && createdFileSizeInMegaBytes < 1024 * 1.1);
        }
        [Fact]
        public void CheckIfFileFormatOfFirstLineIsCorrect()
        {
            if (File.Exists(OUTPUT_NAME))
            {
                using (StreamReader reader = new StreamReader(OUTPUT_NAME))
                {
                    var firstLine = reader.ReadLine();
                    var regex = new Regex(@"\d+\. .+");
                    Assert.True(regex.Match(firstLine).Success);
                }
            }
        }
    }

}
