using System;

namespace NeuralNetBuilder
{
    public static class Helpers
    {
        internal static void ThrowFormattedException(Exception e)
        {
            throw new ArgumentException($"{e.GetType().Name}:\nDetails: {e.Message}");
        }
        internal static void ThrowFormattedArgumentException(string message)
        {
            throw new ArgumentException($"ArgumentException:\nDetails: {message}");
        }
    }
}
