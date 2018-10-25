using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Comp.NeuralNetwork.NeuralEntities
{
    public class NetworkTrainer
    {
        protected internal readonly double THRESHOLD = 0.001d; //порог ошибки

        public async Task Train(NeuralNetwork network, IList<Tuple<double[], double[]>> trainset)
        {
            var iterationErrors = new double[trainset.Count];
            double eraError = default(int);
            do
            {
                for (int i = 0; i < trainset.Count; ++i)
                {
                    //прямой проход
                    network.SetInputs(trainset[i].Item1);
                    //network.Inputs = trainset[i].Item1;
                    var outputs = await network.GetOutputs();
                    //вычисление ошибки по итерации
                    double[] errors = new double[trainset[i].Item2.Length];
                    for (int x = 0; x < errors.Length; ++x)
                    {
                        // расчет ошибки
                        errors[x] = trainset[i].Item2[x] - outputs[x];
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
                }
                eraError = this.GetEraError(iterationErrors); //вычисление ошибки по эпохе
                //debugging
                Console.WriteLine(eraError);
            } while (eraError > THRESHOLD);

            this.SaveWeights(network);
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
    }
}
