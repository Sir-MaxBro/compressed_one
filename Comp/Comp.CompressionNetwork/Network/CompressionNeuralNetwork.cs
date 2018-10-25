using System.Collections.Generic;
using System.Linq;
using Neural = Comp.NeuralNetwork.NeuralEntities;
using Const = Comp.CompressionNetwork.Constants.CompressionNetworkConstants;
using Comp.NeuralNetwork.Memories;
using Comp.General.Extensions;

namespace Comp.CompressionNetwork.Network
{
    public class CompressionNeuralNetwork
    {
        private const int CHARACTERS_COUNT = 128;
        private readonly INeuralNetworkFactory _neuralNetworkFactory;
        private Neural.NeuralNetwork _neuralNetwork;

        public CompressionNeuralNetwork(INeuralNetworkFactory neuralNetworkFactory)
        {
            _neuralNetworkFactory = neuralNetworkFactory;
        }

        public IEnumerable<string> Compress(string source)
        {
            foreach (var batch in source.Batch(Const.InputsCount))
            {
                var inputs = this.ConvertInputs(batch);
                _neuralNetwork.SetInputs(inputs);
                var outputs = _neuralNetwork.GetOutputs();
            }
            return null;
        }

        private void InitializeNetwork()
        {
            var memories = new Queue<ILayerMemory>();
            _neuralNetwork = _neuralNetworkFactory.GetNeuralNetwork(memories, Const.InputsCount, Const.LayersCount, Const.OutputsCount);
        }

        private double[] ConvertInputs(IEnumerable<char> inputs)
        {
            return inputs.Select(i => (double)i / CHARACTERS_COUNT).ToArray();
        }
    }
}
