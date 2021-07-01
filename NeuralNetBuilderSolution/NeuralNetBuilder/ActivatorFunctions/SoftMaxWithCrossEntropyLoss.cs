using MatrixExtensions;

namespace NeuralNetBuilder.ActivatorFunctions
{
    /// <summary>
    /// Use only on output layer!
    /// </summary>
    public class SoftMaxWithCrossEntropyLoss : SoftMax
    {
        public SoftMaxWithCrossEntropyLoss() => ActivationType = ActivationType.SoftMaxWithCrossEntropyLoss;

        public override void Derivation(float[] weightedInput, float[] result)
        {
            weightedInput.ForEach(x => 1, result);
        }
    }
}
