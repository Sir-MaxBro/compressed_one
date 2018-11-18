using System.IO;
using System.Xml;

namespace Comp.NeuralNetwork.Memories.Implementations
{
    public class XmlLayerMemory : ILayerMemory
    {
        private readonly string _path;
        private readonly XmlDocument _xmlDocument = new XmlDocument();

        public XmlLayerMemory(string path)
        {
            _path = path;
            this.Initialize(_path);
        }

        public double[,] LoadWeight(int neuronCount, int inputNeuronCount)
        {
            var weights = new double[neuronCount, inputNeuronCount];
            for (int neuronIndex = 0; neuronIndex < neuronCount; neuronIndex++)
            {
                for (int inputNeuronIndex = 0; inputNeuronIndex < inputNeuronCount; inputNeuronIndex++)
                {
                    // initialize weight
                    var neuronWeight = this.MemoryDocumentElement.ChildNodes.Item(inputNeuronIndex + weights.GetLength(1) * neuronIndex).InnerText.Replace(',', '.');
                    weights[neuronIndex, inputNeuronIndex] = double.Parse(neuronWeight, System.Globalization.CultureInfo.InvariantCulture);
                    System.Console.WriteLine("Initialize weights [{0},{1}] = {2} from {3}", neuronIndex, inputNeuronIndex, neuronWeight, _path);
                }
            }

            return weights;
        }

        public void SaveWeight(double[,] weight)
        {
            var neuronCount = weight.GetLength(0);
            var inputNeuronCount = weight.GetLength(1);

            this.InitializeWeightElements(neuronCount, inputNeuronCount);

            for (int i = 0; i < neuronCount; i++)
            {
                for (int j = 0; j < inputNeuronCount; j++)
                {
                    // set weight
                    this.MemoryDocumentElement.ChildNodes.Item(j + inputNeuronCount * i).InnerText = weight[i, j].ToString();
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
            if (!File.Exists(path))
            {
                File.Create(path);

                var xmlDeclaration = _xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);
                _xmlDocument.InsertBefore(xmlDeclaration, _xmlDocument.DocumentElement);

                var rootElement = _xmlDocument.CreateElement(string.Empty, "weights", string.Empty);
                _xmlDocument.AppendChild(rootElement);

                _xmlDocument.Save(path);
            }

            _xmlDocument.Load(path);
        }

        private void InitializeWeightElements(int neuronCount, int inputNeuronCount)
        {
            var fullNeuronsCount = neuronCount * inputNeuronCount;
            var childNodesCount = this.MemoryDocumentElement.ChildNodes.Count;

            if (childNodesCount != fullNeuronsCount)
            {
                for (int i = childNodesCount; i < fullNeuronsCount - childNodesCount; i++)
                {
                    var newWeightElement = _xmlDocument.CreateElement("weight");
                    this.MemoryDocumentElement.AppendChild(newWeightElement);
                }
            }
        }
    }
}
