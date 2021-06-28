using CustomLogger;
using System.Threading.Tasks;

namespace NeuralNetBuilder
{
    public interface IBaseNet : ILoggable
    {
        // int L { get; }
        Task FeedForwardAsync(float[] input);
        float[] Output { get; }
    }
}
