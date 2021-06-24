using System;

namespace NeuralNetBuilder.ActivatorFunctions
{
    [Serializable]
    public class ReLU : ActivationFunction
    {
        public override float Activation(float weightedInput)
        {
            return weightedInput >= 0 ? weightedInput : 0;
        }
        public override float Derivation(float weightedInput)
        {
            return weightedInput >= 0 ? 1 : 0;
        }
    }
}
