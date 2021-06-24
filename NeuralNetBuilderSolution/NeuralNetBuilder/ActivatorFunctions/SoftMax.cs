using MatrixHelper;
using System;
using System.Linq;

namespace NeuralNetBuilder.ActivatorFunctions
{
    [Serializable]
    public class SoftMax : ActivationFunction
    {
        public override float Activation(float weightedInput)
        {
            return (float)Math.Exp(weightedInput);
        }
        public override float Derivation(float weightedInput)
        {
            return weightedInput * (1 - weightedInput);
        }

        public override void Activation(IMatrix weightedInput, IMatrix result)
        {
            result.ForEach(weightedInput, x => Activation(x));
            float sum = result.Sum();
            if (sum != 0) result.ForEach(x => x / sum);
        }
        public override void Derivation(IMatrix weightedInput, IMatrix result)
        {
            result.ForEach(weightedInput, x => Derivation(x));
        }
    }
}
