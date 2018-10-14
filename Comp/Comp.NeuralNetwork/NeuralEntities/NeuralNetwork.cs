using Comp.NeuralNetwork.Memories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Comp.NeuralNetwork.NeuralEntities
{
    public class NeuralNetwork
    {
        private const int DEFAULT_LAYER_COUNT = 2;
        private const int DEFAULT_OUTPUT_COUNT = 2;

        private readonly int _layerCount;
        private readonly int _outputCount;
        private readonly int _inputCount;
        private readonly Queue<IMemory> _memories;

        private double[] _inputs;
        private LinkedList<Layer> _layers;

        public NeuralNetwork(Queue<IMemory> memories, double[] inputs)
            : this(memories, inputs, DEFAULT_LAYER_COUNT, DEFAULT_OUTPUT_COUNT) { }

        public NeuralNetwork(Queue<IMemory> memories, double[] inputs, int layerCount)
            : this(memories, inputs, layerCount, DEFAULT_OUTPUT_COUNT) { }

        public NeuralNetwork(Queue<IMemory> memories, double[] inputs, int layerCount, int outputCount)
        {
            if (memories.Count != layerCount)
            {
                throw new ArgumentException();
            }

            _inputs = inputs;
            _inputCount = inputs.Length;
            _layerCount = layerCount;
            _outputCount = outputCount;
            _memories = memories;

            this.InitializeNetwork();
        }

        public double[] Inputs
        {
            get { return _inputs; }
            internal set
            {
                if (_inputs != null && _layers != null)
                {
                    _layers.First.Value.SetInputs(value);
                    _inputs = value;
                }
            }
        }

        internal LinkedList<Layer> Layers
        {
            get => _layers;
        }

        public async Task<double[]> GetOutputs()
        {
            double[] outputs = new double[_outputCount];
            Neuron[] outputNeurons = await this.StartNetwork();

            for (int i = 0; i < _outputCount; i++)
            {
                outputs[i] = outputNeurons[i].GetOutput();
            }

            return outputs;
        }

        private async Task<Neuron[]> StartNetwork()
        {
            var currentLayer = _layers.First;
            while (currentLayer.Next != null)
            {
                await this.Recognize(currentLayer.Value, currentLayer.Next.Value);
                currentLayer = currentLayer.Next;
            }
            return currentLayer.Value.Neurons;
        }

        private Task Recognize(Layer currentLayer, Layer nextLayer)
        {
            return Task.Run(() =>
                {
                    var neurons = currentLayer.Neurons;
                    double[] nextInputs = new double[neurons.Length];
                    for (int i = 0; i < neurons.Length; ++i)
                    {
                        nextInputs[i] = neurons[i].GetOutput();
                    }
                    nextLayer.SetInputs(nextInputs);
                });
        }

        private void InitializeNetwork()
        {
            var layers = new LinkedList<Layer>();

            var inputLayer = this.GetInputLayer();
            var currentLayer = layers.AddFirst(inputLayer);

            var previousNeuronCount = this.GetIncrementedNeuronCount(_inputCount);
            for (int layerIndex = 1; layerIndex < _layerCount - 1; layerIndex++)
            {
                var neuronCount = this.GetIncrementedNeuronCount(previousNeuronCount);
                var memory = _memories.Dequeue();

                var layer = new Layer(neuronCount, previousNeuronCount, memory);
                previousNeuronCount = neuronCount;
                currentLayer = layers.AddAfter(currentLayer, layer);
            }

            var outputMemory = _memories.Dequeue();
            var outputLayer = new Layer(_outputCount, previousNeuronCount, outputMemory);

            layers.AddLast(outputLayer);
            _layers = layers;
        }

        private Layer GetInputLayer()
        {
            var neuronCount = this.GetIncrementedNeuronCount(_inputCount);
            var inputMemory = _memories.Dequeue();

            var inputLayer = new Layer(neuronCount, _inputCount, inputMemory);
            inputLayer.SetInputs(_inputs);

            return inputLayer;
        }

        private int GetIncrementedNeuronCount(int neuronCount)
        {
            return neuronCount * 2;
        }
    }
}
