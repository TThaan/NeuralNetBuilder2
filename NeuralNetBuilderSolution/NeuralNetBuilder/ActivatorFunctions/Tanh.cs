using System;

namespace NeuralNetBuilder.ActivatorFunctions
{
    [Serializable]
    public class Tanh : ActivationFunction
    {
        public Tanh() => ActivationType = ActivationType.Tanh;

        public override float Activation(float weightedInput)
        {
            return (float)((Math.Exp(weightedInput) - Math.Exp(-weightedInput))
                / (Math.Exp(weightedInput) + Math.Exp(-weightedInput)));
        }
        public override float Derivation(float weightedInput)
        {
            return 1 - (Activation(weightedInput) * Activation(weightedInput));
        }
    }
}
