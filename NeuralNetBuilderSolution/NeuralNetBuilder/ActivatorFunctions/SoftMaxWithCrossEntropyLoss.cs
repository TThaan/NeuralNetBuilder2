using MatrixExtensions;

namespace NeuralNetBuilder.ActivatorFunctions
{
    /// <summary>
    /// Use only on output layer!
    /// </summary>
    public class SoftMaxWithCrossEntropyLoss : SoftMax
    {
        public override float[] Derivation(float[] weightedInput)
        {
            return weightedInput.ForEach(x => 1);
        }
    }
}
