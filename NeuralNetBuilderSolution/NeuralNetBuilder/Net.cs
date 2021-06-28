using MatrixExtensions;
using NeuralNetBuilder.FactoriesAndParameters;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NeuralNetBuilder
{
    public interface INet : IBaseNet
    {
        ILayer[] Layers { get; set; }
        INet GetCopy();
        NetStatus NetStatus { get; set; }
        // bool IsInitialized { get; set; }
    }

    [Serializable]
    public class Net : INet
    {
        #region ctor

        internal Net() { }

        #endregion

        #region INet
        
        public ILayer[] Layers { get; set; }
        public INet GetCopy()
        {
            return NetFactory.GetCopy(this);
        }
        public NetStatus NetStatus { get; set; }
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
        public string ToLog()
        {
            string result = LoggableName;

            result += $" (Layers: {Layers.Length})\n";

            foreach (var layer in Layers)
            {
                result += $"\n{layer.ToLog()}";
            }

            result += $"{Output?.ToLog(nameof(Output))}";
            return result;
        }

        #endregion
    }
}
