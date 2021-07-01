using MatrixExtensions;
using System;
using System.Linq;

namespace NeuralNetBuilder.ActivatorFunctions
{
    [Serializable]
    public class SoftMax : ActivationFunction
    {
        public SoftMax() => ActivationType = ActivationType.SoftMax;

        public override float Activation(float weightedInput)
        {
            return (float)Math.Exp(weightedInput);
        }
        public override float Derivation(float weightedInput)
        {
            return weightedInput * (1 - weightedInput);
        }

        public override void Activation(float[] weightedInput, float[] result)
        {
            weightedInput.ForEach(x => Activation(x), result);
            
            float sum = result.Sum();
            if (sum != 0) result.ForEach(x => x / sum, result);
        }
        public override void Derivation(float[] weightedInput, float[] result)
        {
            weightedInput.ForEach(x => Derivation(x), result);
        }
    }
}
