using MatrixExtensions;
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

        public override float[] Activation(float[] weightedInput)
        {
            var result = weightedInput.ForEach(x => Activation(x));
            
            float sum = result.Sum();
            if (sum != 0) result = result.ForEach(x => x / sum);

            return result;
        }
        public override float[] Derivation(float[] weightedInput)
        {
            return weightedInput.ForEach(x => Derivation(x));
        }
    }
}
