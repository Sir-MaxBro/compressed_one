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
                Tuple.Create(new double[]{ 'a', 'a', 'a', 'n', 'n', 'a', 'a', 'a' }, new double[]{ 1, 0, 0, 1, 0, 1, 0, 0 }),
                Tuple.Create(new double[]{ 'q', 'a', 'q', 'a', 'q', 'a', 'b', 'q' }, new double[]{ 1, 0, 1, 0, 1, 0, 1, 1 }),
            };

            var hiddenMemory = new XmlLayerMemory(".\\weights\\hidden.xml");
            var outputMemory = new XmlLayerMemory(".\\weights\\output.xml");

            var memories = new Queue<ILayerMemory>();

            memories.Enqueue(hiddenMemory);
            memories.Enqueue(outputMemory);

            var neuralNetwork = new NeuralEntities.NeuralNetwork(memories, 8, 2, 8);
            var trainer = new NeuralEntities.NetworkTrainer();

            await trainer.Train(neuralNetwork, trainset);
        }
    }
}
