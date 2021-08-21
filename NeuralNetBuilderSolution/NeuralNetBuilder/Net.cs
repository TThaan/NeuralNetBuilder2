using CustomLogger;
using MatrixExtensions;
using NeuralNetBuilder.FactoriesAndParameters;
using NeuralNetBuilder.FactoriesAndParameters.JsonConverters;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NeuralNetBuilder
{
    public interface INet : IBaseNet
    {
        ILayer[] Layers { get; set; }
        INet GetCopy();
        // NetStatus NetStatus { get; set; }
        // bool IsInitialized { get; set; }
    }

    [Serializable]
    public class Net : INet
    {
        #region ctor

        internal Net() { }

        #endregion

        #region INet

        [JsonProperty(ItemConverterType = typeof(GenericJsonConverter<Layer>))]
        public ILayer[] Layers { get; set; }
        public INet GetCopy()
        {
            return NetFactory.GetCopy(this);
        }
        // public NetStatus NetStatus { get; set; }
        //public bool IsInitialized { get; set; }

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
