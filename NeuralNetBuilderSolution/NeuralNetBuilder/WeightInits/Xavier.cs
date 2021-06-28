using MatrixExtensions;
using System;

namespace NeuralNetBuilder.WeightInits
{
    public class Xavier : IWeightInit
    {
        public void InitializeWeights(ILayer layer)
        {
            if (layer.ActivationFunction.ActivationType == ActivationType.ReLU ||
                layer.ActivationFunction.ActivationType == ActivationType.LeakyReLU)
            {
                layer.Weights = layer.Weights.ForEach(x => ForRelu(x, layer.N));
            }
            else
            {
                layer.Weights = layer.Weights.ForEach(x => Standard(x, layer.N));
            }
        }

        #region helpers

        float ForRelu(float weight, int n)
        {
            return weight * (float)Math.Sqrt(2f / n);
        }
        float Standard(float weight, int n)
        {
            return weight * (float)Math.Sqrt(1f / n);
        }

        #endregion
    }
}
