using System;
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
            Console.WriteLine("Load weights...");
            var weights = new double[neuronCount, inputNeuronCount];
            for (int neuronIndex = 0; neuronIndex < neuronCount; neuronIndex++)
            {
                for (int inputNeuronIndex = 0; inputNeuronIndex < inputNeuronCount; inputNeuronIndex++)
                {
                    // initialize weight
                    var neuronWeight = this.MemoryDocumentElement.ChildNodes.Item(inputNeuronIndex + weights.GetLength(1) * neuronIndex).InnerText.Replace(',', '.');
                    weights[neuronIndex, inputNeuronIndex] = double.Parse(neuronWeight, System.Globalization.CultureInfo.InvariantCulture);
                }
            }

            Console.WriteLine("Load weights completed...");

            return weights;
        }

        public void SaveWeight(double[,] weight)
        {
            Console.WriteLine("Save weights...");

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

            Console.WriteLine("Save weights completed...");
        }

        protected XmlElement MemoryDocumentElement
        {
            get { return _xmlDocument.DocumentElement; }
        }

        private void Initialize(string path)
        {
            if (!File.Exists(path))
            {
                this.CreateFile(path);

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
                for (int i = 0; i < fullNeuronsCount - childNodesCount; i++)
                {
                    var newWeightElement = _xmlDocument.CreateElement("weight");
                    this.MemoryDocumentElement.AppendChild(newWeightElement);
                }
            }
        }

        private void CreateFile(string path)
        {
            using (var fileStream = File.Create(path)) { }
        }
    }
}
