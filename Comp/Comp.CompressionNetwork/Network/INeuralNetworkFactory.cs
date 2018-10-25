using Comp.NeuralNetwork.Memories;
using System.Collections.Generic;
using Neural = Comp.NeuralNetwork.NeuralEntities;

namespace Comp.CompressionNetwork.Network
{
    public interface INeuralNetworkFactory
    {
        Neural.NeuralNetwork GetNeuralNetwork(Queue<ILayerMemory> memories, int inputCount);

        Neural.NeuralNetwork GetNeuralNetwork(Queue<ILayerMemory> memories, int inputCount, int layerCount);

        Neural.NeuralNetwork GetNeuralNetwork(Queue<ILayerMemory> memories, int inputCount, int layerCount, int outputCount);
    }
}
