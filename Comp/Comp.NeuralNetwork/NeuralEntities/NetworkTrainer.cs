using Comp.General.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Comp.NeuralNetwork.NeuralEntities
{
    public class NetworkTrainer
    {
        private const int BatchSize = 128;
        protected const double THRESHOLD = 0.001d; //порог ошибки
        private const string EraErrorFileName = @".\eraErrors.txt";

        public async Task Train(NeuralNetwork network, IList<Tuple<double[], double[]>> trainset)
        {
            var iterationErrors = new double[trainset.Count];
            double eraError = default(int);
            do
            {
                for (int i = 0; i < trainset.Count; ++i)
                {
                    var step = default(int);
                    foreach (var item in trainset[i].Item1.Batch(BatchSize))
                    {
                        //прямой проход
                        network.SetInputs(item.ToArray());
                        var outputs = await network.GetOutputs();
                        var expectedOutputs = trainset[i].Item2.Skip(BatchSize * step).Take(BatchSize).ToArray();
                        if (expectedOutputs.Length < BatchSize)
                        {
                            var expectedList = new List<double>(expectedOutputs);
                            expectedList.AddRange(new double[BatchSize - expectedOutputs.Length]);
                            expectedOutputs = expectedList.ToArray();
                        }

                        //вычисление ошибки по итерации
                        double[] errors = new double[expectedOutputs.Length];
                        for (int x = 0; x < errors.Length; ++x)
                        {
                            // расчет ошибки
                            errors[x] = expectedOutputs[x] - outputs[x];
                        }

                        iterationErrors[i] = this.GetIterationError(errors);

                        //обратный проход и коррекция весов

                        var emptyGradients = new double[errors.Length];
                        var emptyErrors = new double[errors.Length];
                        var currentLayer = network.Layers.Last;
                        var gradientSums = currentLayer.Value.BackwardPass(errors, emptyGradients);
                        while (currentLayer.Previous != null)
                        {
                            currentLayer = currentLayer.Previous;
                            emptyErrors = new double[currentLayer.Value.Neurons.Length];
                            gradientSums = currentLayer.Value.BackwardPass(emptyErrors, gradientSums);
                        }

                        step++;

                        Console.WriteLine("Iteration error: {0:F50}", iterationErrors[i]);
                    }
                }

                eraError = this.GetEraError(iterationErrors); //вычисление ошибки по эпохе
                trainset = trainset.Mix().ToList();
                this.SaveWeights(network);
                this.SaveEraError(eraError);
                Console.Clear();
                //debugging
                Console.WriteLine("Era error: {0:F50}", eraError);
                GC.Collect();
            } while (eraError > THRESHOLD);
        }

        private void SaveWeights(NeuralNetwork network)
        {
            var currentLayer = network.Layers.First;
            while (currentLayer != null)
            {
                currentLayer.Value.SaveWeights();
                currentLayer = currentLayer.Next;
            }
        }

        protected virtual double GetIterationError(double[] errors)
        {
            double sum = 0;
            for (int i = 0; i < errors.Length; ++i)
            {
                sum += Math.Pow(errors[i], 2);
            }

            return 0.5d * sum;
        }

        protected virtual double GetEraError(double[] iterationsError)
        {
            double sum = 0;
            for (int i = 0; i < iterationsError.Length; ++i)
            {
                sum += iterationsError[i];
            }

            return sum / iterationsError.Length;
        }

        private void SaveEraError(double eraError)
        {
            using (var streamWriter = new StreamWriter(EraErrorFileName, true))
            {
                streamWriter.WriteLine($"Era error = {eraError}");
            }
        }
    }
}
