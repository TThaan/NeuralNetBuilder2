using MatrixHelper;
using System;

namespace NeuralNetBuilder.ActivatorFunctions
{
    [Serializable]
    public abstract class ActivationFunction
    {
        public ActivationType ActivationType { get; internal set; } // redundant?

        public abstract float Activation(float weightedInput);
        public abstract float Derivation(float weightedInput);
        public virtual void Activation(IMatrix weightedInput, IMatrix result)
        {
            result.ForEach(weightedInput, x => Activation(x));
        }
        public virtual void Derivation(IMatrix weightedInput, IMatrix result)
        {
            result.ForEach(weightedInput, x => Derivation(x));
        }
    }
}
