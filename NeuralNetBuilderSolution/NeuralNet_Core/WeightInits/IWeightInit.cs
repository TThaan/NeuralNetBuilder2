namespace NeuralNet_Core.WeightInits
{
    public interface IWeightInit
    {
        void InitializeWeights(ILayer layer);
    }
}
