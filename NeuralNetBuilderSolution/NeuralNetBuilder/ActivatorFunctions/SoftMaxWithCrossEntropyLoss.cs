using MatrixHelper;

namespace NeuralNetBuilder.ActivatorFunctions
{
    /// <summary>
    /// Use only on output layer!
    /// </summary>
    public class SoftMaxWithCrossEntropyLoss : SoftMax
    {
        public override void Derivation(IMatrix weightedInput, IMatrix result)
        {
            result.ForEach(weightedInput, x => 1);
        }
    }
}
