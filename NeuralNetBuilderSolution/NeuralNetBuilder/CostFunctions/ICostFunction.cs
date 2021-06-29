namespace NeuralNetBuilder.CostFunctions
{
    public interface ICostFunction
    {
        void Cost(float[] output, float[] target, float[] result);
        void Derivation(float[] output, float[] target, float[] result);
        float GetTotalCost(float[] output, float[] target);
    }
}
