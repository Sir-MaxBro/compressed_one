using System;

namespace Comp.NeuralNetwork.NeuralEntities
{
    internal class Neuron
    {
        private double[] _weights;
        private double[] _inputs;

        public Neuron(double[] weights)
            : this(null, weights) { }

        public Neuron(double[] inputs, double[] weights)
        {
            _inputs = inputs;
            _weights = weights;
        }

        /// <summary>
        /// Weights of neuron
        /// </summary>
        public double[] Weights
        {
            get => _weights;
            set => _weights = value;
        }

        /// <summary>
        /// Input values of neuron
        /// </summary>
        public double[] Inputs
        {
            get => _inputs;
            set => _inputs = value;
        }

        /// <summary>
        /// Get output value of neuron
        /// </summary>
        /// <returns></returns>
        public double GetOutput()
        {
            return this.Activator(_inputs, _weights);
        }

        /// <summary>
        /// Activator
        /// </summary>
        /// <param name="inputs"></param>
        /// <param name="weights"></param>
        /// <returns></returns>
        private double Activator(double[] inputs, double[] weights)
        {
            double sum = 0;
            for (int i = 0; i < inputs.Length; i++)
            {
                sum += inputs[i] * weights[i];
            }
            return Math.Pow(1 + Math.Exp(-sum), -1);
        }

        public double Derivativator()
        {
            var outsignal = this.GetOutput();
            return outsignal * (1.0d - outsignal); // формула производной для текущей функции активации
        }

        public double Gradientor(double error, double gradientSum)
        {
            var dif = this.Derivativator();
            return error * dif + gradientSum * dif; // gradientSum - это сумма градиентов следующего слоя
        }
    }
}
