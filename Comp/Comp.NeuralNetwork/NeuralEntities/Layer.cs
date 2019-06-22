using Comp.NeuralNetwork.Memories;
using System;

namespace Comp.NeuralNetwork.NeuralEntities
{
    internal class Layer
    {
        #region Fields

        protected const double LEARNING_GRATE = 0.1d;

        private readonly Neuron[] _neurons;
        private readonly ILayerMemory _memory;
        protected readonly int _currentNeuronCount;
        protected readonly int _previousNeuronCount;

        #endregion

        public Layer(int neuronCount, int previousNeuronCount, ILayerMemory memory)
        {
            _currentNeuronCount = neuronCount;
            _previousNeuronCount = previousNeuronCount;
            _neurons = new Neuron[neuronCount];
            _memory = memory;

            this.InitializeWeights();
        }

        public Neuron[] Neurons
        {
            get => _neurons;
        }

        public int NeuronsCount => this.Neurons != null ? this.Neurons.Length : default(int);

        public void SetInputs(double[] inputs)
        {
            if (_currentNeuronCount > 0 && inputs.Length != _previousNeuronCount)
            {
                throw new ArgumentException("The inputs count must be equals to count of neuron inputs.");
            }

            foreach (var neuron in _neurons)
            {
                neuron.Inputs = inputs;
            }
        }

        public void SaveWeights()
        {
            var resultWeights = new double[_currentNeuronCount, _previousNeuronCount];
            for (int i = 0; i < _currentNeuronCount; ++i)
            {
                for (int j = 0; j < _previousNeuronCount; ++j)
                {
                    resultWeights[i, j] = _neurons[i].Weights[j];
                }
            }

            _memory.SaveWeight(resultWeights);
        }

        public double[] BackwardPass(double[] errors, double[] gradients)
        {
            var gradientSums = new double[_previousNeuronCount];
            for (int previousNeuronIndex = 0; previousNeuronIndex < _previousNeuronCount; previousNeuronIndex++) // вычисление градиентных сумм выходного слоя
            {
                double gradientSum = default(double);
                for (int neuronIndex = 0; neuronIndex < _currentNeuronCount; neuronIndex++)
                {
                    var currentNeuron = _neurons[neuronIndex];
                    var gradientor = currentNeuron.Gradientor(errors[neuronIndex], gradients[neuronIndex]);
                    gradientSum += currentNeuron.Weights[previousNeuronIndex] * gradientor;
                    currentNeuron.Weights[previousNeuronIndex] += currentNeuron.Inputs[previousNeuronIndex] * LEARNING_GRATE * gradientor; // коррекция весов         
                }
                gradientSums[previousNeuronIndex] = gradientSum;
            }

            return gradientSums;
        }

        private void InitializeWeights()
        {
            double[,] weightsSource = _memory.LoadWeight(_currentNeuronCount, _previousNeuronCount);
            for (int i = 0; i < _currentNeuronCount; i++)
            {
                double[] weights = new double[_previousNeuronCount];
                for (int j = 0; j < _previousNeuronCount; j++)
                {
                    weights[j] = weightsSource[i, j];
                }

                // initialize neurouns
                _neurons[i] = new Neuron(weights);
            }
        }
    }
}
