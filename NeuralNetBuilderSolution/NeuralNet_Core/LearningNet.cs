using CustomLogger;
using NeuralNet_Core.CostFunctions;
using NeuralNet_Core.FactoriesAndParameters.JsonConverters;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;

namespace NeuralNet_Core
{
    public interface ILearningNet : IBaseNet
    {
        ILearningLayer[] Layers { get; }
        ICostFunction CostFunction { get; }
        float CurrentTotalCost { get; }
        Task PropagateBackAsync(float[] target);
        Task AdjustWeightsAndBiasesAsync(float learningRate);
    }

    public class LearningNet : ILearningNet
    {
        #region ctor & fields

        internal LearningNet() { }

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

        #endregion

        #region ILearningNet

        [JsonProperty(ItemConverterType = typeof(GenericJsonConverter<LearningLayer>))]
        public ILearningLayer[] Layers { get; internal set; }
        public float[] Output { get; internal set; }
        public ICostFunction CostFunction { get; internal set; }
        public float CurrentTotalCost { get; internal set; }
        public async Task PropagateBackAsync(float[] target)
        {
            await Task.Run(() =>
            {
                Layers.Last().ProcessDelta(target, CostFunction);
                CurrentTotalCost = CostFunction.GetTotalCost(Layers.Last().Output, target);
            });
        }
        public async Task AdjustWeightsAndBiasesAsync(float learningRate)
        {
            await Task.Run(() =>
            {
                Layers.ElementAt(1).AdaptWeightsAndBiases(learningRate);
            });
        }

        #endregion

        #region Converters

        public static explicit operator Net(LearningNet learningNet)
        {
            ILayer[] layers = new ILayer[learningNet.Layers.Length];
            learningNet.Layers.CopyTo(layers, 0);

            return new Net()
            {
                Layers = layers,
                IsInitialized = true
            };
        }

        #endregion

        #region ILoggable

        public string LoggableName => "LearningNet";
        public string ToLog(Details details = Details.All)
        {
            string result = LoggableName;

            result += $" (Layers: {Layers.Length}";
            result += $", {nameof(CostFunction)}: {CostFunction.GetType().Name}";
            // result += $", {nameof(CurrentTotalCost)}: {CurrentTotalCost})\n";

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
