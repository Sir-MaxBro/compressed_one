using Comp.Compress.Algorithms;
using NeuralEntities = Comp.NeuralNetwork.NeuralEntities;
using System;
using System.Collections.Generic;
using Comp.NeuralNetwork.Memories.Implementations;
using Comp.NeuralNetwork.Memories;
using System.Threading.Tasks;

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
                Tuple.Create(new double[]{ 1, 0, 1, 0, 1, 0, 1, 0 }, new double[]{ 1, 0 }),
                Tuple.Create(new double[]{ 0, 1, 0, 1, 0, 1, 0, 1 }, new double[]{ 0, 1 }),
                Tuple.Create(new double[]{ 0, 0, 0, 0, 0, 0, 0, 0 }, new double[]{ 0, 0 }),
                Tuple.Create(new double[]{ 1, 1, 1, 1, 1, 1, 1, 1 }, new double[]{ 1, 1 }),
                Tuple.Create(new double[]{ 1, 0, 1, 0, 1, 1, 1, 0 }, new double[]{ 1, 0 }),
            };

            var hiddenMemory = new XmlMemory(".\\weights\\hidden.xml");
            var outputMemory = new XmlMemory(".\\weights\\output.xml");

            var memories = new Queue<IMemory>();

            memories.Enqueue(hiddenMemory);
            memories.Enqueue(outputMemory);

            var emptyInputs = new double[8];
            var neuralNetwork = new NeuralEntities.NeuralNetwork(memories, emptyInputs, 2, 2);
            var trainer = new NeuralEntities.NetworkTrainer();

            await trainer.Train(neuralNetwork, trainset);
        }
    }
}
