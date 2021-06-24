using CustomLogger;
using MatrixHelper;
using System.Threading.Tasks;

namespace NeuralNetBuilder
{
    public interface IBaseNet : ILoggable
    {
        // int L { get; }
        Task FeedForwardAsync(IMatrix input);
        IMatrix Output { get; }
    }
}
