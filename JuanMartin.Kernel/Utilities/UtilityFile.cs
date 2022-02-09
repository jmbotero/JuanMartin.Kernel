using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace JuanMartin.Kernel.Utilities
{
    public class UtilityFile
    {
        public const char CarriageReturn = '\r';
        public const char LineFeed = '\n';

        public static bool WriteArrayToFile<T>(string fileName, T[] source,bool overwrite=true)
        {
            try
            {
                if (overwrite && File.Exists(fileName))
                    File.Delete(fileName);

                File.WriteAllLines(fileName, Array.ConvertAll(source, x => x.ToString()));
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static IEnumerable<string> ReadCsvToList(string fileName, char delimiter, char qualifier)
        {
            var word = new StringBuilder();

            using var stream = File.OpenRead(fileName);
            using var reader = new StreamReader(stream);
            while (reader.Peek() != -1)
            {
                var readChar = (char)reader.Read();

                if (readChar == delimiter || reader.EndOfStream)
                {
                    var w = word.ToString();
                    word.Clear();
                    yield return w;
                }
                else if (readChar != qualifier)
                    word.Append(readChar);
            }
        }

        public static string[] ReadCsvToArray(string fileName, char delimiter, char qualifier)
        {
            using var reader = new StreamReader(fileName, Encoding.UTF8);
            return reader.ReadToEnd().Replace(qualifier.ToString(), string.Empty).Split(new[] { delimiter });
        }

        public static string[][] ReadTextToTwoDimensionalArray(string fileName, char delimiter =  ',')
        {
            using var reader = new StreamReader(fileName, Encoding.UTF8);
            var lines = (IEnumerable<string>)reader.ReadToEnd().Split(new char[] { CarriageReturn, LineFeed }, StringSplitOptions.RemoveEmptyEntries);

            return lines.Select(line => line.Split(new[] { delimiter }, StringSplitOptions.RemoveEmptyEntries).ToArray()).ToArray();
        }

        public static int[][] ReadTextToTwoDimensionalNumericArray(string fileName, char delimiter = ',')
        {
            using var reader = new StreamReader(fileName, Encoding.UTF8);
            var lines = (IEnumerable<string>)reader.ReadToEnd().Split(new char[] { CarriageReturn, LineFeed }, StringSplitOptions.RemoveEmptyEntries);

            return lines.Select(line => line.Split(new[] { delimiter }, StringSplitOptions.RemoveEmptyEntries).Select(v => Convert.ToInt32(v)).ToArray()).ToArray();
        }

        public static int[][] ReadTextToTwoDimensionalNumericArrayWithNullElements(string fileName, char delimiter = ',', string  nullIndicator="-")
        {
            static int ProcessValue(string value, string  indicator)
            {
                if (value == indicator)
                    return 0;
                else
                    return Convert.ToInt32(value);
            };

            using var reader = new StreamReader(fileName, Encoding.UTF8);
            var lines = (IEnumerable<string>)reader.ReadToEnd().Split(new char[] { CarriageReturn, LineFeed }, StringSplitOptions.RemoveEmptyEntries);

            return lines.Select(line => line.Split(new[] { delimiter }, StringSplitOptions.RemoveEmptyEntries).Select(v => ProcessValue(v, nullIndicator)).ToArray()).ToArray();
        }

        public static IEnumerable<string> ReadTextToStringEnumerable(string fileName)
        {
            string line = string.Empty;
            var contents = new List<string>();

            using (var reader = new StreamReader(fileName, Encoding.UTF8))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    contents.Add(line);
                }
            }
            return contents.AsEnumerable();
        }
        public static List<string> ListZipFileContents(string zipFileName)
        {
            var contents = new List<string>();

            ZipArchive zip = ZipFile.Open(zipFileName, ZipArchiveMode.Read);

            foreach (var entry in zip.Entries.ToList())
            {

                var filename = entry.FullName;
                if (!contents.Contains(filename))
                    contents.Add(filename);
            }
            return contents;
        }

        public static string GetTextContentOfFileInZip(string zipFileName, string fileFullName)
        {
            var text = "";

            using (FileStream zipToOpen = new FileStream(zipFileName, FileMode.Open))
            {
                using ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update);
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    if (entry.FullName == fileFullName)
                    {
                        using StreamReader reader = new StreamReader(entry.Open());

                        text = reader.ReadToEnd();
                    }
                }
            }

            return text;
        }

        public static void UpddateTextContentOfFileInZip(string zipFileName, string fileFullName, string text)
        {
            if (string.IsNullOrEmpty(zipFileName))
            {
                throw new ArgumentException($"'{nameof(zipFileName)}' cannot be null or empty.", nameof(zipFileName));
            }

            if (string.IsNullOrEmpty(fileFullName))
            {
                throw new ArgumentException($"'{nameof(fileFullName)}' cannot be null or empty.", nameof(fileFullName));
            }

            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentException($"'{nameof(text)}' cannot be null or empty.", nameof(text));
            }

            using FileStream zipToOpen = new FileStream(zipFileName, FileMode.Open);
            using ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update);
            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                if (entry.FullName == fileFullName)
                {
                    using StreamWriter writer = new StreamWriter(entry.Open());
                    writer.Write(text);
                    break;
                }
            }
        }
    }
}
