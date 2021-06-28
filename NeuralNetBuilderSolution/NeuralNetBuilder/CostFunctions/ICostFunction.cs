namespace NeuralNetBuilder.CostFunctions
{
    public interface ICostFunction
    {
        float[] Cost(float[] output, float[] target);
        float[] Derivation(float[] output, float[] target);
        float GetTotalCost(float[] output, float[] target);
    }
}
