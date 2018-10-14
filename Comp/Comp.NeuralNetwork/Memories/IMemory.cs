namespace Comp.NeuralNetwork.Memories
{
    public interface IMemory
    {
        /// <summary>
        /// Save the neurons weight
        /// </summary>
        /// <param name="weight">The weight.</param>
        void SaveWeight(double[,] weight);

        /// <summary>
        /// Load neurons weight
        /// </summary>
        /// <returns>The weights.</returns>
        double[,] LoadWeight(int neuronCount, int inputNeuronCount);
    }
}
