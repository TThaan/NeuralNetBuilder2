using MatrixHelper;
using NeuralNetBuilder.ActivatorFunctions;

namespace NeuralNetBuilder.FactoriesAndParameters
{
    internal class LayerFactory
    {
        #region public/internal

        internal static ILayer GetRawLayer()
        {
            return new Layer();
        }
        internal static ILayer GetLayer(ILayerParameters layerParameters)
        {
            ILayer result = new Layer()
            {
                Id = layerParameters.Id,
                N = layerParameters.NeuronsPerLayer,
                ActivationFunction = GetActivationFunction(layerParameters.ActivationType),
                Input = new Matrix(layerParameters.NeuronsPerLayer, $"Layer {layerParameters.Id}.{nameof(result.Input)}"),
                Output = new Matrix(layerParameters.NeuronsPerLayer, $"Layer {layerParameters.Id}.{nameof(result.Output)}")
            };

            return result;
        }
        internal static ILearningLayer GetLearningLayer(ILayer layer)
        {
            ILearningLayer result = new LearningLayer()//layers.Weights.GetCopy(), layers.Biases.GetCopy()
            {
                Id = layer.Id ,
                N = layer.N,
                ActivationFunction= layer.ActivationFunction,
                Input = layer.Input.GetCopy($"Layer {layer.Id}.{nameof(result.Input)}"),
                Output = layer.Output.GetCopy($"Layer {layer.Id}.{nameof(result.Output)}"),
                Weights = layer.Weights.GetCopy($"LearningLayer {layer.Id}.{nameof(result.Weights)}"),
                Biases = layer.Biases.GetCopy($"LearningLayer {layer.Id}.{nameof(result.Biases)}"),
                DCDA = new Matrix(layer.N, $"LearningLayer {layer.Id}.{nameof(result.DCDA)}"),
                DADZ = new Matrix(layer.N, $"LearningLayer {layer.Id}.{nameof(result.DADZ)}"),
                Delta = new Matrix(layer.N, $"LearningLayer {layer.Id}.{nameof(result.Delta)}"),
                WeightsChange = layer.Weights == null ? null : new Matrix(layer.Weights.m, layer.Weights.n, $"LearningLayer {layer.Id}.{nameof(result.WeightsChange)}"),
                BiasesChange = layer.Biases == null ? null : new Matrix(layer.N, $"LearningLayer {layer.Id}.{nameof(result.BiasesChange)}")
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
