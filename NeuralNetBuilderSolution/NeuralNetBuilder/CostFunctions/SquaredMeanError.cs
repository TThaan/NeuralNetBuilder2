using MatrixHelper;

namespace NeuralNetBuilder.CostFunctions
{
    public class SquaredMeanError : ICostFunction
    {
        public void Cost(IMatrix output, IMatrix target, IMatrix result) 
        {
            // result = (target - output) * (target - output); //.5f * 
            PerformantOperations.Subtract(target, output, result);
            PerformantOperations.SetHadamardProduct(result, result, result);
        }
        public void Derivation(IMatrix output, IMatrix target, IMatrix result)
        {
            PerformantOperations.Subtract(output, target, result);
        }
        public float GetTotalCost(IMatrix output, IMatrix target)
        {
            float result = 0;

            for (int j = 0; j < output.m; j++)
            {
                result += (target[j] - output[j]) * (target[j] - output[j]);    // .5f * // PerfOps
            }

            return result;
        }
    }
}
