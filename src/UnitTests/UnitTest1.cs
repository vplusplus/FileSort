using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Utils.FileSort;

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
            const int FileSize = OneMB * 10;
            const int WordsPerLine = LineSize / 4;

            var sampleDataFileName = Path.Combine(TestDataBaseFolder, "SampleFile.txt");

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
        public void SplitSortMergeTest()
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
            if (bytes < 1024) return $"{bytes:#,0} bytes";

            var kb = bytes / 1024;
            if (kb < 1024) return $"{kb:#,0.0} KB";

            var mb = kb / 1024;
            if (mb < 1024) return $"{mb:#,0.0} MB";

            var gb = mb / 1024.0;
            return $"{gb:#,0.0} GB";
        }
    }
}
