using System.Collections.Generic;
using System.Linq;
using Neural = Comp.NeuralNetwork.NeuralEntities;
using Comp.NeuralNetwork.Memories;
using Comp.General.Extensions;
using Const = Comp.CompressionNetwork.Constants.CompressionNetworkConstants;
using Comp.NeuralNetwork.Memories.Implementations;
using System;
using System.Threading.Tasks;
using System.Text;

namespace Comp.CompressionNetwork.Network
{
    public class CompressionNeuralNetwork
    {
        private const int CHARACTERS_COUNT = 128;
        private const int START_FREQUENCY_VALUE = 1;
        //private readonly INeuralNetworkFactory _neuralNetworkFactory;
        private Neural.NeuralNetwork _neuralNetwork;

        public CompressionNeuralNetwork(/*INeuralNetworkFactory neuralNetworkFactory*/)
        {
            //_neuralNetworkFactory = neuralNetworkFactory;

            this.InitializeNetwork();
        }

        public Neural.NeuralNetwork NeuralNetwork => _neuralNetwork;

        public async Task<CompressResult> Compress(string source)
        {
            var mainFrequencyMap = new Dictionary<string, int>();
            foreach (var batch in source.Batch(Const.InputsCount))
            {
                var inputs = this.ConvertInputs(batch);
                _neuralNetwork.SetInputs(inputs);
                var frequencies = await _neuralNetwork.GetOutputs();
                var currentFrequencyMap = this.GetFrequencyMap(batch, frequencies);
                this.MergeMaps(mainFrequencyMap, currentFrequencyMap);
            }

            return this.GetCompressResult(source, mainFrequencyMap);
        }

        private CompressResult GetCompressResult(string source, Dictionary<string, int> mainFrequencyMap)
        {
            var compressMap = this.GetCompressMap(mainFrequencyMap);

            var result = new StringBuilder(source);
            foreach (var item in compressMap)
            {
                result.Replace(item.Key, item.Value.ToString() + "#");
            }

            return new CompressResult
            {
                CompressMap = compressMap,
                Result = result.ToString()
            };
        }

        private Dictionary<string, int> GetCompressMap(Dictionary<string, int> mainFrequencyMap)
        {
            var code = 1;
            var compressMap = new Dictionary<string, int>();

            foreach (var item in mainFrequencyMap.Where(f => !string.IsNullOrEmpty(f.Key)).OrderByDescending(f => f.Value).ToList())
            {
                compressMap.Add(item.Key, code);
                code++;
            }

            return compressMap;
        }

        private void MergeMaps(Dictionary<string, int> mainFrequencyMap, Dictionary<string, int> currentFrequencyMap)
        {
            foreach (var frequency in currentFrequencyMap)
            {
                if (mainFrequencyMap.ContainsKey(frequency.Key))
                {
                    mainFrequencyMap[frequency.Key] += frequency.Value;
                }
                else
                {
                    mainFrequencyMap.Add(frequency.Key, frequency.Value);
                }
            }
        }

        private Dictionary<string, int> GetFrequencyMap(IEnumerable<char> source, double[] sourceFrequencies)
        {
            int step = default(int);
            var frequencyMap = new Dictionary<string, int>();
            var frequencies = this.GetFrequencies(sourceFrequencies);

            foreach (var currentFrequency in frequencies)
            {
                var value = new string(source.Skip(step).Take(currentFrequency).ToArray());
                var frequency = frequencyMap.GetValueOrDefault(value);
                if (frequency == default(int))
                {
                    frequencyMap.Add(value, START_FREQUENCY_VALUE);
                }
                else
                {
                    frequencyMap[value] = ++frequency;
                }

                step += currentFrequency;
            }

            return frequencyMap;
        }

        private void InitializeNetwork()
        {
            var memories = new Queue<ILayerMemory>();

            var hiddenMemory = new XmlLayerMemory(".\\weights\\hidden.xml");
            var outputMemory = new XmlLayerMemory(".\\weights\\output.xml");

            memories.Enqueue(hiddenMemory);
            memories.Enqueue(outputMemory);

            //_neuralNetwork = _neuralNetworkFactory.GetNeuralNetwork(memories, Const.InputsCount, Const.LayersCount, Const.OutputsCount);
            _neuralNetwork = new Neural.NeuralNetwork(memories, Const.InputsCount, Const.LayersCount, Const.OutputsCount);
        }

        private double[] ConvertInputs(IEnumerable<char> inputs)
        {
            return inputs.Select(i => (double)i / CHARACTERS_COUNT).ToArray();
        }

        private List<int> GetFrequencies(double[] sourceFrequencies)
        {
            return sourceFrequencies.Select(f => (int)Math.Round(f * CHARACTERS_COUNT)).ToList();
        }
    }
}
