using System.IO;
using System.Xml;

namespace Comp.NeuralNetwork.Memories.Implementations
{
    public class XmlMemory : IMemory
    {
        private readonly string _path;
        private XmlDocument _xmlDocument;

        public XmlMemory(string path)
        {
            _path = path;
            this.Initialize(_path);
        }

        public double[,] LoadWeight(int neuronCount, int inputNeuronCount)
        {
            double[,] weights = new double[neuronCount, inputNeuronCount];
            // get memory file
            var memoryElements = this.MemoryDocumentElement;
            for (int neuronIndex = 0; neuronIndex < neuronCount; neuronIndex++)
            {
                for (int inputNeuronIndex = 0; inputNeuronIndex < inputNeuronCount; inputNeuronIndex++)
                {
                    // initialize weight
                    var neuronWeight = memoryElements.ChildNodes.Item(inputNeuronIndex + weights.GetLength(1) * neuronIndex).InnerText.Replace(',', '.');
                    weights[neuronIndex, inputNeuronIndex] = double.Parse(neuronWeight, System.Globalization.CultureInfo.InvariantCulture);
                    System.Console.WriteLine("Initialize weights [{0},{1}] = {2} from {3}", neuronIndex, inputNeuronIndex, neuronWeight, _path);
                }
            }
            return weights;
        }

        public void SaveWeight(double[,] weight)
        {
            // get memory file
            XmlElement memoryElements = this.MemoryDocumentElement;

            int neuronCount = weight.GetLength(0);
            int inputNeuronCount = weight.GetLength(1);

            for (int i = 0; i < neuronCount; ++i)
            {
                for (int j = 0; j < inputNeuronCount; ++j)
                {
                    // set weight
                    memoryElements.ChildNodes.Item(j + inputNeuronCount * i).InnerText = weight[i, j].ToString();
                }
            }
            _xmlDocument.Save(_path);
        }


        protected XmlElement MemoryDocumentElement
        {
            get { return _xmlDocument.DocumentElement; }
        }

        private void Initialize(string path)
        {
            try
            {
                _xmlDocument = new XmlDocument();
                _xmlDocument.Load(path);
            }
            catch (DirectoryNotFoundException ex)
            {
                throw;
            }
        }
    }
}
