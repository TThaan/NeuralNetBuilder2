using CustomLogger;
using System.Threading.Tasks;

namespace NeuralNet_Core
{
    public interface IBaseNet : ILoggable
    {
        // int L { get; }
        Task FeedForwardAsync(float[] input);
        float[] Output { get; }
        //ILayer[] Layers { get; }
    }
}
