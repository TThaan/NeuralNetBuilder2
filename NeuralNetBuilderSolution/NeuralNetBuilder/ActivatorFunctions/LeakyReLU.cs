using System;

namespace NeuralNetBuilder.ActivatorFunctions
{
    [Serializable]
    public class LeakyReLU : ActivationFunction
    {
        public override float Activation(float weightedInput)
        {
            return weightedInput >= 0 ? weightedInput : weightedInput/100;
        }
        public override float Derivation(float weightedInput)
        {
            return weightedInput >= 0 ? 1 : 1/100;
        }
    }
}
