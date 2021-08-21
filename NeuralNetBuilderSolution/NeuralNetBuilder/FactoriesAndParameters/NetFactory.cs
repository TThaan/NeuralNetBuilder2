using MatrixExtensions;
using NeuralNetBuilder.CostFunctions;
using NeuralNetBuilder.WeightInits;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NeuralNetBuilder.FactoriesAndParameters
{
    internal class NetFactory
    {
        #region fields

        private static Random rnd = RandomProvider.GetThreadRandom();

        #endregion

        #region public/internal

        internal static async Task<INet> CreateNetAsync(INetParameters netParameters)
        {
            return await Task.Run(() =>
            {
                return CreateNet(netParameters);
            });
        }
        internal static INet CreateNet(INetParameters netParameters)
        {
            INet rawNet = new Net() { 
                //NetStatus = NetStatus.Undefined 
            };

            ILayer[] layers = new ILayer[netParameters.LayerParametersCollection.Count];
            
            for (int i = 0; i < netParameters.LayerParametersCollection.Count; i++)
            {
                layers[i] = LayerFactory.GetLayer(netParameters.LayerParametersCollection.ElementAt(i));

                if (i > 0)
                {
                    layers[i].Weights = GetWeights(netParameters.LayerParametersCollection.ElementAt(i), netParameters.LayerParametersCollection.ElementAt(i - 1));
                    layers[i].Biases = GetBiases(netParameters.LayerParametersCollection.ElementAt(i));
                    InitializeWeight(layers[i], netParameters.WeightInitType);
                }
            }
            for (int i = 0; i < netParameters.LayerParametersCollection.Count - 1; i++)
            {
                layers[i+1].ReceptiveField = layers[i];
                layers[i].ProjectiveField = layers[i + 1];
            }

            rawNet.Layers = layers;
            //rawNet.NetStatus = NetStatus.Initialized;    // DIC?

            return rawNet;
        }
        internal static ILearningNet GetLearningNet(INet net, CostType costType)
        {
            ILearningLayer[] layers = new ILearningLayer[net.Layers.Length];
            layers = net.Layers.Select(x => LayerFactory.GetLearningLayer(x))
                .ToArray();

            for (int i = 0; i < layers.Length - 1; i++)
            {
                layers[i + 1].ReceptiveField = layers[i];
                layers[i].ProjectiveField = layers[i + 1];
            }

            ILearningNet result = new LearningNet()
            {
                Layers = layers,
                CostFunction = GetCostFunction(costType)
            };

            return result;
        }
        internal static INet GetNet(ILearningNet originalNet)
        {
            ILayer[] layers = new ILayer[originalNet.Layers.Length];
            originalNet.Layers.CopyTo(layers, 0);
            return new Net()
            {
                Layers = layers
            };
        }
        internal static INet GetCopy(INet originalNet)
        {
            ILayer[] layers = new ILayer[originalNet.Layers.Length];
            originalNet.Layers.CopyTo(layers, 0);
            return new Net()
            {
                Layers = layers
            };
        }

        #endregion

        #region helpers

        private static float[,] GetWeights(ILayerParameters layerParameters, ILayerParameters receptiveLayerParameters)
        {
            int m = layerParameters.NeuronsPerLayer;
            int n = receptiveLayerParameters.NeuronsPerLayer;
            float weightMin = layerParameters.WeightMin;
            float weightMax = layerParameters.WeightMax;

            var result = new float[m, n];
            result.ForEach(x => GetRandomFloat(weightMin, weightMax), result);
            return result;
        }
        private static float[] GetBiases(ILayerParameters layerParameters)
        {
            if ((layerParameters.BiasMax - layerParameters.BiasMin) == 0)
                return null;

            int m = layerParameters.NeuronsPerLayer;
            float biasMin = layerParameters.BiasMin;
            float biasMax = layerParameters.BiasMax;

            var result = new float[m];
            result.ForEach(x => GetRandomFloat(biasMin, biasMax), result);
            return result;
        }
        private static float GetRandomFloat(float min, float max)
        {
            return (float)(min + (max - min) * rnd.NextDouble());
        }
        private static void InitializeWeight(ILayer layer, WeightInitType weightInitType)
        {
            IWeightInit weightInit;

            switch (weightInitType)
            {
                case WeightInitType.Xavier:
                    weightInit = new Xavier();
                    break;
                default:
                    weightInit = default;
                    // throw new ArgumentException("No default CostFunction defined!");
                    break;
            }

            if (weightInit != null)
            {
                weightInit.InitializeWeights(layer);
            }
        }
        private static ICostFunction GetCostFunction(CostType costType)
        {
            switch (costType)
            {
                case CostType.SquaredMeanError:
                    return new SquaredMeanError();
                default:
                    return default;
                    // throw new ArgumentException("No default CostFunction defined!");
            }
        }

        #endregion
    }
}
