using MatrixExtensions;
using NeuralNet_Core.ActivatorFunctions;

namespace NeuralNet_Core.FactoriesAndParameters
{
    internal class LayerFactory
    {
        #region public/internal

        internal static ILayer CreateLayer(ILayerParameters layerParameters)
        {
            ILayer result = new Layer()
            {
                Id = layerParameters.Id,
                N = layerParameters.NeuronsPerLayer,
                ActivationFunction = GetActivationFunction(layerParameters.ActivationType),
                Input = new float[layerParameters.NeuronsPerLayer],
                Output = new float[layerParameters.NeuronsPerLayer]
            };

            return result;
        }
        internal static ILearningLayer CreateLearningLayer(ILayer layer)
        {
            ILearningLayer result = new LearningLayer()//layers.Weights.GetCopy(), layers.Biases.GetCopy()
            {
                Id = layer.Id ,
                N = layer.N,
                ActivationFunction= layer.ActivationFunction,
                Input = layer.Input.GetCopy(),
                Output = layer.Output.GetCopy(),
                Weights = layer.Weights?.GetCopy(),
                Biases = layer.Biases?.GetCopy(),
                DCDA = new float[layer.N],
                DADZ = new float[layer.N],
                Delta = new float[layer.N],
                WeightsChange = layer.Weights == null ? null : new float[layer.Weights.GetLength(0), layer.Weights.GetLength(1)],
                BiasesChange = layer.Biases == null ? null : new float[layer.N]
            };

            return result;
        }

        #endregion

        #region helpers

        private static ActivationFunction GetActivationFunction(ActivationType activationType)
        {
            switch (activationType)
            {
                case ActivationType.LeakyReLU:
                    return new LeakyReLU();
                case ActivationType.NullActivator:
                    return new NullActivator();
                case ActivationType.ReLU:
                    return new ReLU();
                case ActivationType.Sigmoid:
                    return new Sigmoid();
                case ActivationType.SoftMax:
                    return new SoftMax();
                case ActivationType.SoftMaxWithCrossEntropyLoss:
                    return new SoftMaxWithCrossEntropyLoss();
                case ActivationType.Tanh:
                    return new Tanh();
                case ActivationType.None:
                    return default;
                default:
                    return default;
            }
        }

        #endregion
    }
}
