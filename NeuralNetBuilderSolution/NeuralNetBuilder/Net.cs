using CustomLogger;
using MatrixExtensions;
using NeuralNetBuilder.CostFunctions;
using NeuralNetBuilder.FactoriesAndParameters;
using NeuralNetBuilder.FactoriesAndParameters.JsonConverters;
using NeuralNetBuilder.WeightInits;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NeuralNetBuilder
{
    public interface INet : IBaseNet
    {
        ILayer[] Layers { get; set; }
        ILearningNet GetLearningNet(CostType costType);
        INet GetCopy();

        void Initialize(INetParameters netParameters);
        void Reset();
        bool IsInitialized { get; }
    }

    [Serializable]
    public class Net : INet
    {
        #region fields

        private Random rnd = RandomProvider.GetThreadRandom();

        #endregion

        #region IBaseNet

        public async Task FeedForwardAsync(float[] input)
        {
            await Task.Run(() =>
            {
                Layers[0].ProcessInput(input);
                Output = Layers.Last().Output;
            });
        }
        public float[] Output { get; internal set; }

        #endregion

        #region INet

        [JsonProperty(ItemConverterType = typeof(GenericJsonConverter<Layer>))]
        public ILayer[] Layers { get; set; }
        public ILearningNet GetLearningNet(CostType costType)
        {
            ILearningLayer[] layers = new ILearningLayer[Layers.Length];
            layers = Layers.Select(x => LayerFactory.CreateLearningLayer(x))
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

        // Better as copy ctor?
        public INet GetCopy()
        {
            ILayer[] layers = new ILayer[Layers.Length];
            Layers.CopyTo(layers, 0);
            return new Net()
            {
                Layers = layers
            };
        }

        #region helpers

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

        #endregion

        #region Initialization

        public void Initialize(INetParameters netParameters)
        {
            ILayer[] layers = new ILayer[netParameters.LayerParametersCollection.Count];

            for (int i = 0; i < netParameters.LayerParametersCollection.Count; i++)
            {
                layers[i] = LayerFactory.CreateLayer(netParameters.LayerParametersCollection.ElementAt(i));

                if (i > 0)
                {
                    layers[i].Weights = GetRandomWeights(netParameters.LayerParametersCollection.ElementAt(i), netParameters.LayerParametersCollection.ElementAt(i - 1));
                    layers[i].Biases = GetRandomBiases(netParameters.LayerParametersCollection.ElementAt(i));
                    ModifyWeight(layers[i], netParameters.WeightInitType);
                }
            }
            for (int i = 0; i < netParameters.LayerParametersCollection.Count - 1; i++)
            {
                layers[i + 1].ReceptiveField = layers[i];
                layers[i].ProjectiveField = layers[i + 1];
            }

            Layers = layers;
            IsInitialized = true;    // DIC?

        }
        public void Reset()
        {
            throw new NotImplementedException();
        }
        public bool IsInitialized { get; private set; }

        #region helpers

        private float[,] GetRandomWeights(ILayerParameters layerParameters, ILayerParameters receptiveLayerParameters)
        {
            var result = new float[layerParameters.NeuronsPerLayer, receptiveLayerParameters.NeuronsPerLayer];
            result.ForEach(x => GetRandomFloat(layerParameters.WeightMin, layerParameters.WeightMax), result);
            return result;
        }
        private float[] GetRandomBiases(ILayerParameters layerParameters)
        {
            if ((layerParameters.BiasMax - layerParameters.BiasMin) == 0)
                return null;

            var result = new float[layerParameters.NeuronsPerLayer];
            result.ForEach(x => GetRandomFloat(layerParameters.BiasMin, layerParameters.BiasMax), result);
            return result;
        }
        private float GetRandomFloat(float min, float max)
        {
            return (float)(min + (max - min) * rnd.NextDouble());
        }
        private void ModifyWeight(ILayer layer, WeightInitType weightInitType)
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

        #endregion

        #endregion

        #region Converters

        //public static explicit operator LearningNet(Net net)
        //{
        //    
        //    
        //    
        //    
        //    
        //    
        //    

        //    
        //    
        //    
        //    
        //}

        #endregion

        #region ILoggable

        public string LoggableName => "Net";
        public string ToLog(Details details = Details.All)
        {
            string result = LoggableName;

            result += $" (Layers: {Layers.Length})\n";

            if (details == Details.Little)
                return result;

            if (details == Details.Medium)
            {
                foreach (var layer in Layers)
                {
                    result += $"\n{layer.ToLog(Details.Little)}";
                }

                return result;
            }

            foreach (var layer in Layers)
            {
                result += $"\n{layer.ToLog(Details.All)}";
            }

            // result += $"{Output?.ToLog(nameof(Output))}";
            return result;
        }

        #endregion
    }
}
