
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Utils.FileSort;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    // Sorry... 
    // These are not 'real' tests, rather just a tool to trigger some use-cases.
    // Running the tests doesn't guarentee the code is defect free.

    [TestClass]
    public class UnitTest1
    {
        const int OneKB = 1024;
        const int OneMB = OneKB * 1024;
        const int OneGB = OneMB * 1024;

        const string TestDataBaseFolder = "D:/Junk/FILESORT";

        [TestMethod]
        public void GenerateJunk()
        {
            const int LineSize = 64;
            const int FileSize = OneMB;
            const int WordsPerLine = LineSize / 4;

            var sampleDataFileName = Path.Combine(TestDataBaseFolder, $"SampleFile-{SizeToString(FileSize)}.txt");

            using (var writer = File.CreateText(sampleDataFileName))
            {
                var bytesWritten = 0;

                // Append random text till we reach suggeted file size...
                while(bytesWritten < FileSize)
                {
                    var nextLine = string.Join(" ", RandomWords.Next().Take(WordsPerLine));
                    writer.WriteLine(nextLine);
                    bytesWritten += nextLine.Length + Environment.NewLine.Length;
                }
            }
        }

        [TestMethod]
        public void SplitSortMerge()
        {
            var inputFileName = Path.Combine(TestDataBaseFolder, "Input-1GB.txt");
            var outputFileName = Path.Combine(TestDataBaseFolder, "Output-1GB.txt");
            var inputFileSize = new FileInfo(inputFileName).Length;
            var bufferSize = OneMB * 100;

            var timer = Stopwatch.StartNew();
            FileSorter.SortFile(inputFileName, outputFileName, bufferSize, StringComparison.Ordinal, descending: false);
            timer.Stop();

            Console.WriteLine($"FileSize  : {SizeToString(inputFileSize)}");
            Console.WriteLine($"BufferSize: {SizeToString(bufferSize)}");
            Console.WriteLine($"Elapsed   : {timer.Elapsed}");
        }

        static string SizeToString(long bytes)
        {
            const float K = 1024.0f;
            const float T = 1023.0f;

            float size = bytes;

            return size < T ? $"{size:#,0} bytes"
                : (size /= K) < T ? $"{size:#,0.0} KB"  
                : (size /= K) < T ? $"{size:#,0.0} MB"
                : (size /= K) < T ? $"{size:#,0.0} GB"
                : (size /= K) < T ? $"{size:#,0.0} TB"
                : $"~{size/K:#,0.0} PB";
        }
    }
}
