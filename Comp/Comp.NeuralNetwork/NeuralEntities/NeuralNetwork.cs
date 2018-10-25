using Comp.NeuralNetwork.Memories;
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
        private readonly Queue<ILayerMemory> _memories;
        private readonly object lockObject = new object();

        private double[] _inputs;
        private LinkedList<Layer> _layers;

        public NeuralNetwork(Queue<ILayerMemory> memories, int inputCount)
            : this(memories, inputCount, DEFAULT_LAYER_COUNT, DEFAULT_OUTPUT_COUNT) { }

        public NeuralNetwork(Queue<ILayerMemory> memories, int inputCount, int layerCount)
            : this(memories, inputCount, layerCount, DEFAULT_OUTPUT_COUNT) { }

        public NeuralNetwork(Queue<ILayerMemory> memories, int inputCount, int layerCount, int outputCount)
        {
            if (memories.Count != layerCount)
            {
                layerCount = memories.Count;
            }

            //this.SetInputs(inputs);
            _memories = memories;
            _layerCount = layerCount;
            _outputCount = outputCount;

            this.InitializeNetwork(inputCount);
        }

        public double[] Inputs
        {
            get { return _inputs; }
        }

        public void SetInputs(double[] inputs)
        {
            _inputs = inputs;
            if (_layers != null)
            {
                _layers.First.Value.SetInputs(_inputs);
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

        /// <summary>
        /// Get neuron count for the next layer
        /// </summary>
        /// <param name="neuronCount">The count of neurons.</param>
        /// <returns>Return count of neurons for the next layer.</returns>
        public virtual int GetIncrementedNeuronCount(int neuronCount)
        {
            return neuronCount * 2;
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

        private void InitializeNetwork(int inputCount)
        {
            var layersStorage = new LinkedList<Layer>();
            var currentLayer = this.SetInputLayer(inputCount, layersStorage);

            var previousNeuronCount = currentLayer.Value.NeuronsCount;
            for (int layerIndex = 1; layerIndex < _layerCount - 1; layerIndex++)
            {
                var memory = this.GetLayerMemory();
                var neuronCount = this.GetIncrementedNeuronCount(previousNeuronCount);
                var nextLayer = new Layer(neuronCount, previousNeuronCount, memory);
                previousNeuronCount = neuronCount;
                currentLayer = layersStorage.AddAfter(currentLayer, nextLayer);
            }

            this.SetOutputLayer(_outputCount, layersStorage);

            lock (lockObject)
            {
                _layers = layersStorage;
            }
        }

        private LinkedListNode<Layer> SetInputLayer(int inputCount, LinkedList<Layer> layersStorage)
        {
            var inputMemory = this.GetLayerMemory();
            var neuronCount = this.GetIncrementedNeuronCount(inputCount);
            var inputLayer = new Layer(neuronCount, inputCount, inputMemory);
            return layersStorage?.AddFirst(inputLayer);
        }

        private LinkedListNode<Layer> SetOutputLayer(int outputCount, LinkedList<Layer> layersStorage)
        {
            var outputMemory = this.GetLayerMemory();
            var previousNeuronCount = layersStorage.Last.Value.NeuronsCount;
            var outputLayer = new Layer(outputCount, previousNeuronCount, outputMemory);
            return layersStorage?.AddLast(outputLayer);
        }

        private ILayerMemory GetLayerMemory()
        {
            return _memories?.Dequeue();
        }
    }
}
