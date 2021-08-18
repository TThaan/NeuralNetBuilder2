using System;

namespace NeuralNetBuilder
{
    public static class Helpers
    {
        public static string GetFormattedExceptionMessage(Exception e)
        {
            return $"{e.GetType().Name}:\nDetails: {e.Message}";
        }
        public static void ThrowFormattedException(Exception e)
        {
            throw new ArgumentException(GetFormattedExceptionMessage(e));
        }
        public static void ThrowFormattedArgumentException(string message)
        {
            throw new ArgumentException($"ArgumentException:\nDetails: {message}");
        }
    }
}
