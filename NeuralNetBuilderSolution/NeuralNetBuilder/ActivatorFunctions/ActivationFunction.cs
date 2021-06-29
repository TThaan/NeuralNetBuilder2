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
        public virtual void Activation(float[] weightedInput, float[] result)
        {
            result.ForEach(weightedInput, x => Activation(x));
        }
        public virtual void Derivation(float[] weightedInput, float[] result)
        {
            result.ForEach(weightedInput, x => Derivation(x));
        }
    }
}
