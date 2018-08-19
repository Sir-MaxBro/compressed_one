namespace Comp.NeuralNetwork.Memory
{
    public interface IMemory
    {
        /// <summary>
        /// Save the neurons weight
        /// </summary>
        /// <param name="weight"></param>
        void Save(double[,] weight);

        /// <summary>
        /// Load neurons weight
        /// </summary>
        /// <returns></returns>
        double[,] Load();
    }
}
