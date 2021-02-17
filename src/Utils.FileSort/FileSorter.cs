using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace Utils.FileSort
{
    // FileSize     : 1.0 GB
    // BufferSize   : 100 MB
    // Split & Sort : 00:00:23.1957404
    // Merge        : 00:00:07.6137010
    // Total        : 00:00:30.9096024

    public static class FileSorter
    {
        const int OneKB = 1024;
        const int OneMB = OneKB * 1024;
        const int TenMB = OneMB * 10;
        const int TwoFiftySixMB = OneMB * 256;

        public static void SortFile(string inputFileName, string outputFileName, int bufferSizeInBytes = TenMB, StringComparison stringComparison = StringComparison.Ordinal, bool descending = false)
        {
            if (null == inputFileName) throw new ArgumentNullException(nameof(inputFileName));
            if (null == outputFileName) throw new ArgumentNullException(nameof(outputFileName));

            // Should we trust the caller, or should we impose a range?
            if (bufferSizeInBytes < OneMB || bufferSizeInBytes > TwoFiftySixMB) throw new ArgumentException("Please use a bufferSize between 1 MB and 256 MB.");

            // Ensure input file exists.
            if (!File.Exists(inputFileName)) throw new FileNotFoundException($"Input file not found: {inputFileName}");

            // Ensure output folder exists.
            // Any issues creating output folder, let it throw now, don't waste time processing input.
            var outputDir = Path.GetDirectoryName(outputFileName);
            if (!Directory.Exists(outputDir)) Directory.CreateDirectory(outputDir);

            // An IComparer for given ascending|descending string comparison.
            var sortOrder = CreateComparer(stringComparison, descending);

            // How large is the input file?
            var inputFileSizeInBytes = new FileInfo(inputFileName).Length;

            if (inputFileSizeInBytes < bufferSizeInBytes)
            {
                // Input file is smaller than suggested buffer size.
                // Just read/sort/save; do not bother to split/sort/merge.
                var lines = File.ReadAllLines(inputFileName);
                Array.Sort(lines, sortOrder);
                File.WriteAllLines(outputFileName, lines);
            }
            else
            {
                // A temp folder for storing the split files. 
                var tempFolderName = Path.Combine(Path.GetTempPath(), $"fsrt-{Guid.NewGuid()}");
                Directory.CreateDirectory(tempFolderName);

                try
                {
                    // Split, sort and merge.
                    SplitAndSort(inputFileName, tempFolderName, bufferSizeInBytes, sortOrder);
                    Merge(tempFolderName, outputFileName, sortOrder);
                }
                finally
                {
                    // Purge the temp folder.
                    Directory.Delete(tempFolderName, recursive: true);
                }
            }
        }

        static void SplitAndSort(string inputFileName, string tempFolderName, int bufferSizeInBytes, IComparer<string> sortOrder)
        {
            if (null == inputFileName) throw new ArgumentNullException(nameof(inputFileName));
            if (null == tempFolderName) throw new ArgumentNullException(nameof(tempFolderName));
            if (null == sortOrder) throw new ArgumentNullException(nameof(sortOrder));

            using (var reader = File.OpenText(inputFileName))
            {
                var fileIndex = 0;
                var lines = new List<string>();
                var bytesRead = 0;

                while (true)
                {
                    var nextLine = reader.ReadLine();

                    if (null != nextLine)
                    {
                        lines.Add(nextLine);
                        bytesRead += nextLine.Length;
                    }

                    // Flush if reached end or exceeded the buffer size.
                    var shouldFlush = (null == nextLine && bytesRead > 0) || (bytesRead >= bufferSizeInBytes);
                    if (shouldFlush)
                    {
                        // Sort the lines.
                        lines.Sort(sortOrder);

                        // Save the sorted lines.
                        fileIndex += 1;
                        var sortedFileName = Path.Combine(tempFolderName, $"sorted-{fileIndex}.tmp");
                        File.WriteAllLines(sortedFileName, lines);

                        // Reset.
                        lines.Clear();
                        bytesRead = 0;
                    }

                    // Exit if reached end of the input file.
                    if (null == nextLine) break;
                }
            }
        }

        static void Merge(string tempFolderName, string outputFileName, IComparer<string> sortOrder)
        {
            if (null == tempFolderName) throw new ArgumentNullException(nameof(tempFolderName));
            if (null == outputFileName) throw new ArgumentNullException(nameof(outputFileName));
            if (null == sortOrder) throw new ArgumentNullException(nameof(sortOrder));

            // Open a LineReader on each input file.
            var readers = Directory.GetFiles(tempFolderName).Select(fileName => new LineReader(fileName)).ToArray();

            try
            {
                using (var writer = File.CreateText(outputFileName))
                {
                    // Consume the next 'lowest' text from the readers, append to output.
                    string nextLine;
                    while (null != (nextLine = ConsumeLowest())) writer.WriteLine(nextLine);
                }
            }
            finally
            {
                // Let go the readers...
                foreach (var reader in readers) reader?.Dispose();
            }

            string ConsumeLowest()
            {
                int lowestReader = -1;
                string lowestText = null;

                for (int i = 0; i < readers.Length; i++)
                {
                    // Peek next line, don't consume yet.
                    var line = readers[i].Peek();

                    // If this reader has not reached EOF... 
                    if (null != line)
                    {
                        // Is this line the lowest text?
                        if (-1 == lowestReader || sortOrder.Compare(line, lowestText) < 0)
                        {
                            lowestReader = i;
                            lowestText = line;
                        }
                    }
                }

                // Consume() the lowest line; Returns NULL if reached the end.
                return -1 == lowestReader ? null : readers[lowestReader].Consume();
            }
        }
   
        static IComparer<string> CreateComparer(StringComparison sortOrder, bool descending)
        {
            int OrdernalDesc(string lhs, string rhs) => StringComparer.Ordinal.Compare(rhs, lhs);
            int OrdinalIgnoreCaseDesc(string lhs, string rhs) => StringComparer.OrdinalIgnoreCase.Compare(rhs, lhs);
            int CurrentCultureDesc(string lhs, string rhs) => StringComparer.CurrentCulture.Compare(rhs, lhs);
            int CurrentCultureIgnoreCaseDesc(string lhs, string rhs) => StringComparer.CurrentCultureIgnoreCase.Compare(rhs, lhs);
            int InvariantCultureDesc(string lhs, string rhs) => StringComparer.InvariantCulture.Compare(rhs, lhs);
            int InvariantCultureIgnoreCaseDesc(string lhs, string rhs) => StringComparer.InvariantCultureIgnoreCase.Compare(rhs, lhs);

            if (descending)
            {
                switch (sortOrder)
                {
                    case StringComparison.Ordinal: return Comparer<string>.Create(OrdernalDesc);
                    case StringComparison.OrdinalIgnoreCase: return Comparer<string>.Create(OrdinalIgnoreCaseDesc);
                    case StringComparison.CurrentCulture: return Comparer<string>.Create(CurrentCultureDesc);
                    case StringComparison.CurrentCultureIgnoreCase: return Comparer<string>.Create(CurrentCultureIgnoreCaseDesc);
                    case StringComparison.InvariantCulture: return Comparer<string>.Create(InvariantCultureDesc);
                    case StringComparison.InvariantCultureIgnoreCase: return Comparer<string>.Create(InvariantCultureIgnoreCaseDesc);
                    default: throw new Exception($"Invalid sort order (StringComparison): {sortOrder}");
                }
            }
            else
            {
                switch(sortOrder)
                {
                    case StringComparison.Ordinal: return StringComparer.Ordinal;
                    case StringComparison.OrdinalIgnoreCase: return StringComparer.OrdinalIgnoreCase;
                    case StringComparison.CurrentCulture: return StringComparer.CurrentCulture;
                    case StringComparison.CurrentCultureIgnoreCase: return StringComparer.CurrentCultureIgnoreCase;
                    case StringComparison.InvariantCulture: return StringComparer.InvariantCulture;
                    case StringComparison.InvariantCultureIgnoreCase: return StringComparer.InvariantCultureIgnoreCase;
                    default: throw new Exception($"Invalid sort order (StringComparison): {sortOrder}");
                }
            }
        }

        sealed class LineReader : IDisposable
        {
            StreamReader Reader = null;
            string CurrentLine = null;

            internal LineReader(string fileName)
            {
                Reader = File.OpenText(fileName);
                CurrentLine = Reader.ReadLine();
            }

            // Returns current line, or NULL if reached EOF.
            internal string Peek() => CurrentLine;

            // Returns current line. Advances the reader to next line.
            internal string Consume()
            {
                if (null == CurrentLine) throw new Exception("Reached end of file. Please Peek() before you Consume()");

                var lastLine = CurrentLine;
                CurrentLine = Reader.ReadLine();
                return lastLine;
            }

            public void Dispose()
            {
                try {
                    Interlocked.Exchange(ref Reader, null)?.Dispose();
                }
                catch { }
            }
        }
    }
}

/*
        static void Merge(string tempFolderName, string outputFileName, IComparer<string> sortOrder)
        {
            if (null == tempFolderName) throw new ArgumentNullException(nameof(tempFolderName));
            if (null == outputFileName) throw new ArgumentNullException(nameof(outputFileName));
            if (null == sortOrder) throw new ArgumentNullException(nameof(sortOrder));

            // Get list of files.
            // All files in this folder will be merged.
            var fileNames = Directory.GetFiles(tempFolderName);

            // A reader for each file, and a queue to hold few lines from each file. 
            StreamReader[]  readers = new StreamReader[fileNames.Length];
            Queue<string>[] queues = new Queue<string>[fileNames.Length];

            try
            {
                // Open input files, create an empty queue for each file.
                for(int i=0; i<fileNames.Length; i++)
                {
                    readers[i] = File.OpenText(fileNames[i]);
                    queues[i] = new Queue<string>();
                }

                // Append the the lowest text from each file.
                using (var writer = File.CreateText(outputFileName))
                {
                    string nextLine;
                    while (null != (nextLine = RefillQueuesAndDequeueLowest())) writer.WriteLine(nextLine);
                    
                    // using() should do this. Just in case...
                    writer.Flush();
                }
            }
            finally
            {
                // Safe-close the input files.
                foreach (var reader in readers) try { reader?.Close(); } catch { }
            }

            string RefillQueuesAndDequeueLowest()
            {
                int lowest = -1;
                string lowestText = null;

                for (int i = 0; i < queues.Length; i++)
                {
                    // Next queue...
                    var q = queues[i];

                    // If this queue is already dropped...
                    if (null == q) continue;

                    // If queue is empty...
                    if (0 == q.Count)
                    {
                        // Refill this queue with more lines...
                        var r = readers[i];
                        string nextLine;
                        while (q.Count < 64 && null != (nextLine = r.ReadLine())) q.Enqueue(nextLine);

                        // If reached end of this file...
                        if (0 == q.Count)
                        {
                            queues[i] = null;
                            continue;
                        }
                    }

                    // Does this queue represent next lowest text?
                    var lower = -1 == lowest || sortOrder.Compare(q.Peek(), lowestText) < 0;
                    if (lower)
                    {
                        lowest = i;
                        lowestText = q.Peek();
                    }
                }

                // Dequeue and return next lowest text. 
                // Returns NULL if reached the end.
                return -1 == lowest ? null : queues[lowest].Dequeue();
            }
        }
*/