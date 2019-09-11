using Comp.General.Compression;
using System;
using System.Collections.Generic;

namespace Comp.Compress.Algorithms
{
    public class LZWCompression : ICompression<string, IEnumerable<int>>
    {
        private const byte ALPHABET_COUNT = 128;
        private const byte BYTE_NUMBER = 8;

        public LZWCompression() { }

        public IEnumerable<int> Compress(string source)
        {
            var compressedSource = new List<int>();
            var maxCode = this.GetMaxCode();
            var codingTable = this.GetCodingTable();

            var currentCode = ALPHABET_COUNT;
            var previousNGramma = string.Empty;

            foreach (var @char in source)
            {
                var currentNGramma = previousNGramma + @char;
                var isNGrammaExist = codingTable.ContainsKey(currentNGramma);
                if (isNGrammaExist)
                {
                    previousNGramma = currentNGramma;
                }
                else
                {
                    var existedNGramma = codingTable[previousNGramma];
                    compressedSource.Add(existedNGramma);
                    previousNGramma = @char.ToString();
                    if (currentCode <= maxCode)
                    {
                        codingTable.Add(currentNGramma, currentCode);
                        currentCode++;
                    }
                }
            }

            var lastNGramma = codingTable[previousNGramma];
            compressedSource.Add(lastNGramma);

            return compressedSource;
        }

        public string Decompress(IEnumerable<int> compressedSource)
        {
            var compressedSourceQueue = new Queue<int>(compressedSource);
            var maxCode = this.GetMaxCode();
            var decodingTable = this.GetDecodingTable();

            var currentCode = ALPHABET_COUNT;
            var firstCode = compressedSourceQueue.Dequeue();
            var previousNGramma = decodingTable[firstCode];
            var decompressed = previousNGramma;

            foreach (var code in compressedSourceQueue)
            {
                var existedNGramma = decodingTable.GetValueOrDefault(code);
                if (string.IsNullOrEmpty(existedNGramma))
                {
                    existedNGramma = string.Concat(previousNGramma, previousNGramma[0]);
                }

                decompressed += existedNGramma;
                if (currentCode <= maxCode)
                {
                    var newNGramma = string.Concat(previousNGramma, existedNGramma[0]);
                    decodingTable.Add(currentCode, newNGramma);
                    currentCode++;
                }

                previousNGramma = existedNGramma;
            }

            return decompressed;
        }

        /// <summary>
        /// Serves to get the maximum code
        /// </summary>
        /// <returns>The max code for symbols</returns>
        private int GetMaxCode()
        {
            return (int)(Math.Pow(2, BYTE_NUMBER) - 1);
        }

        private Dictionary<int, string> GetDecodingTable()
        {
            var decodingTable = new Dictionary<int, string>();

            for (int i = 0; i < ALPHABET_COUNT; i++)
            {
                decodingTable.Add(i, ((char)i).ToString());
            }

            return decodingTable;
        }

        private Dictionary<string, int> GetCodingTable()
        {
            var codingTable = new Dictionary<string, int>();

            for (int i = 0; i < ALPHABET_COUNT; i++)
            {
                codingTable.Add(((char)i).ToString(), i);
            }

            return codingTable;
        }
    }
}
