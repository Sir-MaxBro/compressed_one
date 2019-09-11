using Comp.General.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DataGenerator
{
    class Program
    {
        private const int BATCH_SIZE = 128;
        private const string Suffix = "_compressed";
        private const string FileExtension = ".txt";

        static void Main(string[] args)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(@".\data\");
            var files = directoryInfo.GetFiles().Where(f => !f.Name.Contains(Suffix)); ;
            foreach (var file in files)
            {
                Console.WriteLine($"Compression started file {file.Name}...");

                var source = string.Empty;
                using (var streamReader = new StreamReader(file.FullName))
                {
                    source = streamReader.ReadToEnd();
                }

                var compressedSource = Compress(source);

                using (var fileStream = File.Create(directoryInfo.FullName + "\\" + file.Name.Replace(FileExtension, "") + Suffix + FileExtension))
                {
                    using (var streamWriter = new StreamWriter(fileStream))
                    {
                        streamWriter.Write(compressedSource);
                    }
                }

                Console.WriteLine($"Compression finished file {file.Name}...");
            }

            Console.ReadKey();
        }

        private static string Compress(string source)
        {
            var resultMap = new Dictionary<string, int>();

            foreach (List<char> item in source.Batch(BATCH_SIZE).ToList())
            {
                for (int i = 0; i < item.Count; i++)
                {
                    for (int j = 0; j < item.Count - i; j++)
                    {
                        var st = new string(item.Skip(i).Take(j + 1).ToArray());
                        if (!resultMap.ContainsKey(st))
                        {
                            resultMap.Add(st, 1);
                        }
                        else
                        {
                            resultMap[st] += 1;
                        }
                    }
                }
            }

            var rmap = resultMap.Where(rm => rm.Key.Length != 1).GroupBy(rm => rm.Value).OrderByDescending(rm => rm.Key).ToList();

            var resultSource = new StringBuilder(source);
            foreach (var map in rmap)
            {
                var sortedMap = map.OrderByDescending(m => m.Key.Length);
                foreach (var item in sortedMap)
                {
                    var replacedStr = $"{item.Key.Length}" + new string('0', item.Key.Length - 1);
                    resultSource.Replace(item.Key, replacedStr);
                }
            }

            foreach (var item in resultSource.ToString())
            {
                if (char.IsLetter(item))
                {
                    resultSource.Replace(item, '1');
                }
            }

            return resultSource.ToString();
        }
    }
}
