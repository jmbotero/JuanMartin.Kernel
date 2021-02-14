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

        public static bool WriteArrayToFile<T>(string file_name, T[] source,bool overwrite=true)
        {
            try
            {
                if (overwrite && File.Exists(file_name))
                    File.Delete(file_name);

                File.WriteAllLines(file_name, Array.ConvertAll(source, x => x.ToString()));
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

            using (var stream = File.OpenRead(fileName))
            using (var reader = new StreamReader(stream))
            {
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
        }

        public static string[] ReadCsvToArray(string fileName, char delimiter, char qualifier)
        {
            using (var reader = new StreamReader(fileName, Encoding.UTF8))
            {
                return reader.ReadToEnd().Replace(qualifier.ToString(), string.Empty).Split(new[] { delimiter });
            }
        }

        public static string[][] ReadTextToTwoDimensionalArray(string fileName, char delimiter)
        {
            using (var reader = new StreamReader(fileName, Encoding.UTF8))
            {
                var lines = (IEnumerable<string>)reader.ReadToEnd().Split(new char[] { CarriageReturn, LineFeed }, StringSplitOptions.RemoveEmptyEntries);

                return lines.Select(line => line.Split(new[] { delimiter }, StringSplitOptions.RemoveEmptyEntries).ToArray()).ToArray();
            }
        }

        public static int [][] ReadTextToTwoDimensionalNumericArray(string fileName, char delimiter)
        {
            using (var reader = new StreamReader(fileName, Encoding.UTF8))
            {
                var lines = (IEnumerable<string>)reader.ReadToEnd().Split(new char[] { CarriageReturn, LineFeed }, StringSplitOptions.RemoveEmptyEntries);

                return lines.Select(line => line.Split(new[] { delimiter }, StringSplitOptions.RemoveEmptyEntries).Select(v => Convert.ToInt32(v)).ToArray()).ToArray();
            }
        }

        public static List<string> ListZipFileContents(string zip_name)
        {
            var contents = new List<string>();

            ZipArchive zip = ZipFile.Open(zip_name, ZipArchiveMode.Read);

            foreach (var entry in zip.Entries.ToList())
            {

                var filename = entry.FullName;
                if (!contents.Contains(filename))
                    contents.Add(filename);
            }
            return contents;
        }

        public static string GetTextContentOfFileInZip(string zip_name, string file_full_name)
        {
            var text = "";

            using (FileStream zipToOpen = new FileStream(zip_name, FileMode.Open))
            {
                using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
                {
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        if (entry.FullName == file_full_name)
                        {
                            using (StreamReader reader = new StreamReader(entry.Open()))
                            {

                                text = reader.ReadToEnd();
                            }
                        }
                    }
                }
            }

            return text;
        }

        public static void UpddateTextContentOfFileInZip(string zip_name, string file_full_name, string text, string new_zip_name = "")
        {
            using (FileStream zipToOpen = new FileStream(zip_name, FileMode.Open))
            {
                using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
                {
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        if (entry.FullName == file_full_name)
                        {
                            using (StreamWriter writer = new StreamWriter(entry.Open()))
                            {
                                writer.Write(text);
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}
