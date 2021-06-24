using MatrixHelper;

namespace NeuralNetBuilder.CostFunctions
{
    public interface ICostFunction
    {
        void Cost(IMatrix output, IMatrix target, IMatrix result);
        void Derivation(IMatrix output, IMatrix target, IMatrix result);
        float GetTotalCost(IMatrix output, IMatrix target);
    }
}
