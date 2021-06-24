namespace NeuralNetBuilder.WeightInits
{
    public interface IWeightInit
    {
        void InitializeWeights(ILayer layer);
    }
}
