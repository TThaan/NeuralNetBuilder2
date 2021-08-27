namespace NeuralNetBuilder
{
    public interface IInit
    {
        void Initialize(params object[] parameters);
        void Reset();
        bool IsInitialized { get; }
    }
}
