using System;

namespace NeuralNetBuilder.ActivatorFunctions
{
    [Serializable]
    public class Sigmoid : ActivationFunction
    {
        public Sigmoid() => ActivationType = ActivationType.Sigmoid;

        public override float Activation(float weightedInput)
        {
            return 1 / (1 + (float)Math.Exp(-weightedInput));
        }
        public override float Derivation(float weightedInput)
        {
            return Activation(weightedInput) * (1 - Activation(weightedInput));
        }
    }
}
