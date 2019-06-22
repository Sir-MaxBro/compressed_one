using Comp.Compress.Algorithms;
using NeuralEntities = Comp.NeuralNetwork.NeuralEntities;
using System;
using System.Collections.Generic;
using Comp.NeuralNetwork.Memories.Implementations;
using Comp.NeuralNetwork.Memories;
using System.Threading.Tasks;
using Comp.CompressionNetwork.Network;
using System.Linq;
using System.IO;

namespace Comp.ConsoleUI
{
    class Program
    {
        private const int ALPHABET_COUNT = 128;
        private const int BATCH_SIZE = 128;
        private const string Suffix = "_compressed";
        private const string FileExtension = ".txt";

        static void Main(string[] args)
        {
            var program = new Program();
            program.TrainNetwork();

            Console.ReadLine();
        }

        private async Task TrainNetwork()
        {
            var trainset = GetTrainset();

            var neuralNetwork = new CompressionNeuralNetwork();

            var trainer = new NeuralEntities.NetworkTrainer();

            await trainer.Train(neuralNetwork.NeuralNetwork, trainset);
        }

        private static List<Tuple<double[], double[]>> GetTrainset()
        {
            var trainset = new List<Tuple<double[], double[]>>();

            DirectoryInfo directoryInfo = new DirectoryInfo(@".\data\");
            var allFiles = directoryInfo.GetFiles();
            var files = allFiles.Where(f => !f.Name.Contains(Suffix)); ;
            foreach (var file in files)
            {
                var source = string.Empty;
                var compressedSource = string.Empty;

                using (var streamReader = new StreamReader(file.FullName))
                {
                    source = streamReader.ReadToEnd();
                }

                var compresssedFile = allFiles.Where(x => x.Name == file.Name.Replace(FileExtension, "") + Suffix + FileExtension).FirstOrDefault();
                if (compresssedFile != null)
                {
                    using (var streamReader = new StreamReader(compresssedFile.FullName))
                    {
                        compressedSource = streamReader.ReadToEnd();
                    }
                }

                trainset.Add(new Tuple<double[], double[]>(source.ToCharArray().Select(x => (double)x).ToArray(), compressedSource.ToCharArray().Select(i => (double)i / ALPHABET_COUNT).ToArray()));
            }

            return trainset;
        }
    }
}
