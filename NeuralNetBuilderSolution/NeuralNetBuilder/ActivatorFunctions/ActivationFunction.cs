using MatrixExtensions;
using System;

namespace NeuralNetBuilder.ActivatorFunctions
{
    [Serializable]
    public abstract class ActivationFunction    // Better as ext meths?
    {
        public ActivationType ActivationType { get; internal set; } // redundant?

        public abstract float Activation(float weightedInput);
        public abstract float Derivation(float weightedInput);
        public virtual float[] Activation(float[] weightedInput)
        {
            return weightedInput.ForEach(x => Activation(x));
        }
        public virtual float[] Derivation(float[] weightedInput)
        {
            return weightedInput.ForEach(x => Derivation(x));
        }
    }
}
