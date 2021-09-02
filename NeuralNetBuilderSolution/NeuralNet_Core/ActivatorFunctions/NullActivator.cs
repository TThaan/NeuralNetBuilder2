using System;

namespace NeuralNet_Core.ActivatorFunctions
{
    [Serializable]
    public class NullActivator : ActivationFunction
    {
        public NullActivator() => ActivationType = ActivationType.NullActivator;

        public override float Activation(float weightedInput)
        {
            return weightedInput;
        }
        public override float Derivation(float weightedInput)
        {
            return 1;
        }
    }
}
