using Comp.NeuralNetwork.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Comp.NeuralNetwork.Layers
{
    internal abstract class Layer
    {
        #region Fields

        protected int _currentNeuronCount;
        protected int _previousNeuronCount;
        protected const double LEARNING_GRATE = 0.1d;
        private Neuron[] _neurons;

        #endregion

        protected Layer(int neuronCount, int previousNeuronCount)
        {

        }

        public Neuron[] Neurons
        {
            get => _neurons;
            set => _neurons = value;
        }
    }
}
