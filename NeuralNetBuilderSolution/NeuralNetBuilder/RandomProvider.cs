using System;
using System.Threading;

namespace NeuralNetBuilder
{
    /// <summary>
    /// https://stackoverflow.com/a/7251724
    /// </summary>
    internal static class RandomProvider
    {
        static int seed = Environment.TickCount;

        static ThreadLocal<Random> randomWrapper = new ThreadLocal<Random>
            (() => new Random(Interlocked.Increment(ref seed)));

        internal static Random GetThreadRandom()
        {
            return randomWrapper.Value;
        }
    }
}
