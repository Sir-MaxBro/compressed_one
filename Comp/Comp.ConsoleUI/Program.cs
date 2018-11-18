using Comp.Compress.Algorithms;
using NeuralEntities = Comp.NeuralNetwork.NeuralEntities;
using System;
using System.Collections.Generic;
using Comp.NeuralNetwork.Memories.Implementations;
using Comp.NeuralNetwork.Memories;
using System.Threading.Tasks;
using Comp.CompressionNetwork.Network;

namespace Comp.ConsoleUI
{
    class Program
    {
        static void Main(string[] args)
        {
            var program = new Program();
            program.TrainNetwork();

            Console.ReadLine();
        }

        private async Task TrainNetwork()
        {
            var trainset = new List<Tuple<double[], double[]>>
            {
                Tuple.Create(new double[]{ 'a', 'a', 'a', 'n', 'n', 'a', 'a', 'a', 'n', 'n', 'a', 'a' }, new double[]{ 1, 0, 0, 1, 0, 1, 0, 0, 1, 0, 1, 1 }),
                Tuple.Create(new double[]{ 'q', 'a', 'q', 'a', 'q', 'a', 'b', 'q', 'q', 'a', 'b', 'q' }, new double[]{ 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0 }),
                Tuple.Create(new double[]{ 'b', 'v', 'a', 'a', 'a', 'a', 'b', 'v', 'b', 'v', 'w', 'e' }, new double[]{ 1, 0, 1, 0, 0, 0, 1, 0, 1, 0, 1, 1 }),
                Tuple.Create(new double[]{ 'a', 'b', 'v', 'f', 'r', 'e', 'w', 'q', 't', 'y', 'u', 'i' }, new double[]{ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }),
                Tuple.Create(new double[]{ 'm', 'm', 'm', 'm', 'm', ' ', 'm', 'm', 'm', 'm', 'm', 'm' }, new double[]{ 1, 0, 1, 0, 1, 1, 1, 0, 1, 0, 1, 0 }),
                Tuple.Create(new double[]{ 'r', 'a', 'r', 'w', 'r', 'a', 'r', 'w', 'r', 'a', 'r', 'w' }, new double[]{ 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0 }),
            };

            var neuralNetwork = new CompressionNeuralNetwork();

            //var s = await neuralNetwork.Compress("TOBEORNOTTOBEORTOBEORNOT");
            var trainer = new NeuralEntities.NetworkTrainer();

            await trainer.Train(neuralNetwork.NeuralNetwork, trainset);
        }
    }
}
